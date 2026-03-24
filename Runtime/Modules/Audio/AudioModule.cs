using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.IoC;
using Framework.Module.Resource;
using UnityEngine;

namespace Framework.Module.Audio
{
	/// <summary>
	/// 音频模块，实现枚举与整型频道播放（支持循环参数与缓入）、并发、动态频道、音量、暂停/恢复与停止（支持缓出）。
	/// </summary>
	[Dependencies(typeof(IResourceModule))]
	internal sealed class AudioModule : Module, IAudioModule
	{
		/// <summary>
		/// 初始固定频道数量（含 BG/UI/Effect 等）
		/// </summary>
		const int InitialChannelCount =5;
		/// <summary>
		/// 动态频道起始 Id（避免与固定频道冲突）
		/// </summary>
		const int DynamicChannelStart =11;
		/// <summary>
		/// 最大动态频道数量（限制无限增长）
		/// </summary>
		const int MaxDynamicChannels =32;
		/// <summary>
		/// 默认音量（新建频道初始化使用）
		/// </summary>
		float volume =1f;
		/// <summary>
		///资源缓存（剪辑名到 AudioClip）
		/// </summary>
		readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>(128);
		/// <summary>
		///频道缓存（频道 Id 到频道实例）
		/// </summary>
		readonly Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>(64);
		/// <summary>
		/// 主音源异步播放中的频道集合（用于同帧二次调用时转为并发播放）
		/// </summary>
		readonly HashSet<int> mainPending = new HashSet<int>();
		/// <summary>
		/// 渐变任务列表（用于缓入缓出）
		/// </summary>
		readonly List<FadeTask> fades = new List<FadeTask>(32);
		/// <summary>
		///资源加载器
		/// </summary>
		IResourceLoader resourceLoader;
		/// <summary>
		/// 根节点对象
		/// </summary>
		GameObject root;

		/// <summary>
		/// 渐变任务结构
		/// </summary>
		class FadeTask
		{
			public AudioSource Source;
			public float From;
			public float To;
			public float Duration;
			public float Elapsed;
			public Action<AudioSource> OnComplete;
		}

		/// <summary>
		/// 注入资源加载器
		/// </summary>
		[Inject]
		public void SetResourcesLoader(IResourceLoader resourceLoader)
		{
			if (resourceLoader == null)
			{
				throw new NullReferenceException("resourceLoader can not be null");
			}
			this.resourceLoader = resourceLoader;
		}

		/// <summary>
		/// 构造函数，初始化根节点与固定频道
		/// </summary>
		public AudioModule()
		{
			CreateRoot();
			InitFixedChannels(InitialChannelCount);
		}

		/// <summary>
		/// 每帧更新（处理音量渐变）
		/// </summary>
		internal override void OnUpdate()
		{
			if (fades.Count ==0)
			{
				return;
			}
			var dt = Time.deltaTime;
			for (var i = fades.Count -1; i >=0; i--)
			{
				var f = fades[i];
				if (f.Source == null)
				{
					fades.RemoveAt(i);
					continue;
				}
				f.Elapsed += dt;
				var t = f.Duration <=0f ?1f : Mathf.Clamp01(f.Elapsed / f.Duration);
				f.Source.volume = Mathf.Lerp(f.From, f.To, t);
				if (t >=1f)
				{
					var cb = f.OnComplete;
					fades.RemoveAt(i);
					if (cb != null)
					{
						cb(f.Source);
					}
				}
			}
		}

		/// <summary>
		/// 创建根节点
		/// </summary>
		void CreateRoot()
		{
			root = GameObject.Find("[AudioRoot]");
			if (root == null)
			{
				root = new GameObject("[AudioRoot]");
				UnityEngine.Object.DontDestroyOnLoad(root);
			}
		}

		/// <summary>
		/// 初始化固定频道
		/// </summary>
		void InitFixedChannels(int count)
		{
			for (var i =0; i < count; i++)
			{
				GetChannel(i);
			}
			AudioChannel bg;
			if (channels.TryGetValue((int)ChannelType.BG, out bg))
			{
				bg.Loop = true;
			}
		}

		/// <summary>
		/// 获取频道（不存在则创建）
		/// </summary>
		AudioChannel GetChannel(int id)
		{
			AudioChannel channel;
			if (channels.TryGetValue(id, out channel))
			{
				return channel;
			}
			channel = new AudioChannel(id, false);
			channel.Volume = volume;
			channels[id] = channel;
			return channel;
		}

		/// <summary>
		/// 获取动态频道（复用或创建）
		/// </summary>
		AudioChannel GetDynamicChannel()
		{
			for (var i = DynamicChannelStart; i < DynamicChannelStart + MaxDynamicChannels; i++)
			{
				AudioChannel ch;
				if (channels.TryGetValue(i, out ch))
				{
					if (!ch.Playing)
					{
						return ch;
					}
				}
				else
				{
					return GetChannel(i);
				}
			}
			return channels[DynamicChannelStart];
		}

		/// <summary>
		/// 异步获取音频资源
		/// </summary>
		ValueTask<AudioClip> GetClipAsync(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new ValueTask<AudioClip>((AudioClip)null);
			}
			AudioClip cached;
			if (clips.TryGetValue(name, out cached))
			{
				return new ValueTask<AudioClip>(cached);
			}
			return LoadClipAsync(name);
		}

		/// <summary>
		/// 加载并缓存音频资源
		/// </summary>
		async ValueTask<AudioClip> LoadClipAsync(string name)
		{
			var asset = await resourceLoader.GetAsync<AudioClip>(name);
			if (asset != null && !clips.ContainsKey(name))
			{
				clips.Add(name, asset);
			}
			return asset;
		}

		/// <summary>
		/// 设置频道速度更新模式
		/// </summary>
		public void SetVelocityUpdateMode(ChannelType channel, bool dynamicMode)
		{
			var ch = GetChannel((int)channel);
			ch.IgnoreTimeScale(dynamicMode);
		}
		public void SetVelocityUpdateMode(int channelId, bool dynamicMode)
		{
			GetChannel(channelId).IgnoreTimeScale(dynamicMode);
		}

		/// <summary>
		/// 异步播放频道主音源
		/// </summary>
		public async ValueTask PlayAsync(ChannelType channel, string clipName, bool? loop = null, float fadeInSeconds =0f)
		{
			var id = (int)channel;
			await InternalPlayAsync(id, clipName, false, loop, fadeInSeconds);
		}

		/// <summary>
		/// 异步并发播放频道（不替换主音源）
		/// </summary>
		public async ValueTask PlayConcurrentAsync(ChannelType channel, string clipName, bool? loop = null, float fadeInSeconds =0f)
		{
			var id = (int)channel;
			await InternalPlayAsync(id, clipName, true, loop, fadeInSeconds);
		}
		/// <summary>
		/// 异步播放整型频道主音源
		/// </summary>
		public ValueTask PlayAsync(int channelId, string clipName, bool? loop = null, float fadeInSeconds =0f)
		{
			return InternalPlayAsync(channelId, clipName, false, loop, fadeInSeconds);
		}

		/// <summary>
		/// 异步并发播放整型频道（不替换主音源）
		/// </summary>
		public ValueTask PlayConcurrentAsync(int channelId, string clipName, bool? loop = null, float fadeInSeconds =0f)
		{
			return InternalPlayAsync(channelId, clipName, true, loop, fadeInSeconds);
		}

		/// <summary>
		/// 异步内部播放实现（支持主音源或并发），可选设置循环与缓入，带 pending保护
		/// </summary>
		async ValueTask InternalPlayAsync(
			int channelId,
			string clipName,
			bool concurrent,
			bool? loopOverride,
			float fadeInSeconds
		)
		{
			if (string.IsNullOrEmpty(clipName))
			{
				return;
			}
			if (!concurrent)
			{
				mainPending.Add(channelId);
			}
			try
			{
				var clip = await GetClipAsync(clipName);
				if (clip == null)
				{
					return;
				}
				var ch = GetChannel(channelId);
				if (loopOverride.HasValue)
				{
					ch.Loop = loopOverride.Value;
				}
				var src = concurrent ? ch.PlayConcurrent(clip) : ch.PlayClip(clip);
				if (src == null)
				{
					return;
				}
				var target = ch.Volume;
				if (fadeInSeconds >0f)
				{
					var from =0f;
					var to = target;
					src.volume = from;
					fades.Add(new FadeTask
					{
						Source = src,
						From = from,
						To = to,
						Duration = fadeInSeconds,
						Elapsed =0f,
						OnComplete = null,
					});
				}
				else
				{
					src.volume = target;
				}
			}
			finally
			{
				if (!concurrent)
				{
					mainPending.Remove(channelId);
				}
			}
		}

		/// <summary>
		/// 异步动态频道播放（用于临时音效）
		/// </summary>
		public async ValueTask PlayDynamicAsync(string clipName)
		{
			var clip = await GetClipAsync(clipName);
			if (clip == null)
			{
				return;
			}
			GetDynamicChannel().PlayClip(clip);
		}

		/// <summary>
		/// 设置频道音量
		/// </summary>
		public void SetVolume(ChannelType channel, float value)
		{
			value = Mathf.Clamp01(value);
			var ch = GetChannel((int)channel);
			ch.Volume = value;
			volume = value;
		}
		public void SetVolume(int channelId, float volume)
		{
			volume = Mathf.Clamp01(volume);
			GetChannel(channelId).Volume = volume;
			this.volume = volume;
		}

		/// <summary>
		/// 设置所有频道音量
		/// </summary>
		public void SetVolumeAll(float value)
		{
			value = Mathf.Clamp01(value);
			foreach (var kv in channels)
			{
				kv.Value.Volume = value;
			}
			volume = value;
		}

		/// <summary>
		/// 设置除指定频道外其他频道音量
		/// </summary>
		public void SetVolumeExcept(ChannelType excludedChannel, float value)
		{
			value = Mathf.Clamp01(value);
			var excludedId = (int)excludedChannel;
			foreach (var kv in channels)
			{
				if (kv.Key == excludedId)
				{
					continue;
				}
				kv.Value.Volume = value;
			}
		}

		/// <summary>
		/// 停止频道全部播放（可选缓出）
		/// </summary>
		public void StopChannel(ChannelType channel, float fadeOutSeconds =0f)
		{
			AudioChannel ch;
			if (!channels.TryGetValue((int)channel, out ch))
			{
				return;
			}
			StopChannelInternal(ch, fadeOutSeconds);
		}
		/// <summary>
		/// 停止整型频道全部播放（可选缓出）
		/// </summary>
		public void StopChannel(int channelId, float fadeOutSeconds =0f)
		{
			AudioChannel ch;
			if (!channels.TryGetValue(channelId, out ch))
			{
				return;
			}
			StopChannelInternal(ch, fadeOutSeconds);
		}

		/// <summary>
		/// 内部停止实现（可选缓出）
		/// </summary>
		void StopChannelInternal(AudioChannel ch, float fadeOutSeconds)
		{
			if (ch == null)
			{
				return;
			}
			if (fadeOutSeconds <=0f)
			{
				ch.StopAll();
				return;
			}
			var restore = ch.Volume;
			ch.ForEachActiveSource(src =>
			{
				var from = src.volume;
				fades.Add(new FadeTask
				{
					Source = src,
					From = from,
					To =0f,
					Duration = fadeOutSeconds,
					Elapsed =0f,
					OnComplete = s =>
					{
						s.Stop();
						s.volume = restore;
					},
				});
			});
		}

		/// <summary>
		/// 停止频道中指定音频剪辑
		/// </summary>
		public void StopClip(ChannelType channel, string clipName)
		{
			AudioChannel ch;
			if (!channels.TryGetValue((int)channel, out ch))
			{
				return;
			}
			ch.StopClip(clipName);
		}
		public void StopClip(int channelId, string clipName)
		{
			AudioChannel ch;
			if (!channels.TryGetValue(channelId, out ch))
			{
				return;
			}
			ch.StopClip(clipName);
		}

		/// <summary>
		/// 暂停枚举频道全部播放
		/// </summary>
		public void PauseChannel(ChannelType channel)
		{
			AudioChannel ch;
			if (!channels.TryGetValue((int)channel, out ch))
			{
				return;
			}
			ch.PauseAll();
		}
		/// <summary>
		/// 暂停整型频道全部播放
		/// </summary>
		public void PauseChannel(int channelId)
		{
			AudioChannel ch;
			if (!channels.TryGetValue(channelId, out ch))
			{
				return;
			}
			ch.PauseAll();
		}
		/// <summary>
		/// 恢复枚举频道全部播放
		/// </summary>
		public void ResumeChannel(ChannelType channel)
		{
			AudioChannel ch;
			if (!channels.TryGetValue((int)channel, out ch))
			{
				return;
			}
			ch.ResumeAll();
		}
		/// <summary>
		/// 恢复整型频道全部播放
		/// </summary>
		public void ResumeChannel(int channelId)
		{
			AudioChannel ch;
			if (!channels.TryGetValue(channelId, out ch))
			{
				return;
			}
			ch.ResumeAll();
		}

		/// <summary>
		/// 停止所有频道播放
		/// </summary>
		public void StopAll()
		{
			foreach (var kv in channels)
			{
				kv.Value.StopAll();
			}
		}

		/// <summary>
		/// 模块卸载回收资源
		/// </summary>
		internal override void OnTearDown()
		{
			resourceLoader.Release();
			base.OnTearDown();
		}
	}
}