using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework.Module.Audio
{
	/// <summary>
	/// 单个音频频道，支持主音源与并发播放音源池，并可绑定 AudioMixerGroup。
	/// </summary>
	public class AudioChannel
	{
		/// <summary>
		///频道 Id
		/// </summary>
		public int ChannelId { get; private set; }
		/// <summary>
		/// 是否循环（主音源专用）
		/// </summary>
		bool loop;
		/// <summary>
		/// 根节点承载对象
		/// </summary>
		GameObject root;
		/// <summary>
		/// 本频道挂载对象
		/// </summary>
		GameObject owner;
		/// <summary>
		/// 主音源（用于背景或常驻音）
		/// </summary>
		AudioSource mainSource;
		/// <summary>
		/// 并发音源池
		/// </summary>
		List<AudioSource> concurrentSources;
		/// <summary>
		/// 最大并发音源数量
		/// </summary>
		const int MaxConcurrentSources =8;
		/// <summary>
		/// 所属混音组
		/// </summary>
		AudioMixerGroup mixerGroup;

		/// <summary>
		///兼容旧用法构造（自动创建根节点）
		/// </summary>
		public AudioChannel(int channelId, bool loop) : this(channelId, loop, GetOrCreateRoot(), null)
		{
		}

		/// <summary>
		/// 构造函数，初始化主音源与池
		/// </summary>
		public AudioChannel(int channelId, bool loop, GameObject root, AudioMixerGroup mixerGroup)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}
			ChannelId = channelId;
			this.loop = loop;
			this.root = root;
			this.mixerGroup = mixerGroup;
			concurrentSources = new List<AudioSource>(MaxConcurrentSources);

			var childName = string.Format("[AudioChannel_{0}]", channelId);
			owner = new GameObject(childName);
			owner.transform.SetParent(root.transform, false);

			mainSource = owner.AddComponent<AudioSource>();
			ConfigureSource(mainSource, loop);
		}

		/// <summary>
		/// 获取或创建根节点
		/// </summary>
		static GameObject GetOrCreateRoot()
		{
			var root = GameObject.Find("[AudioRoot]");
			if (root == null)
			{
				root = new GameObject("[AudioRoot]");
				UnityEngine.Object.DontDestroyOnLoad(root);
			}
			return root;
		}

		/// <summary>
		/// 配置音源公共属性
		/// </summary>
		void ConfigureSource(AudioSource source, bool loopFlag)
		{
			source.playOnAwake = false;
			source.loop = loopFlag;
			if (mixerGroup != null)
			{
				source.outputAudioMixerGroup = mixerGroup;
			}
		}

		/// <summary>
		/// 获取可用并发音源
		/// </summary>
		AudioSource GetConcurrentSource()
		{
			for (var i =0; i < concurrentSources.Count; i++)
			{
				var src = concurrentSources[i];
				if (!src.isPlaying)
				{
					return src;
				}
			}
			if (concurrentSources.Count < MaxConcurrentSources)
			{
				var newSrc = owner.AddComponent<AudioSource>();
				ConfigureSource(newSrc, false);
				newSrc.volume = Volume;
				concurrentSources.Add(newSrc);
				return newSrc;
			}
			return concurrentSources[0];
		}

		/// <summary>
		///频道音量（主音源与并发源）
		/// </summary>
		public float Volume
		{
			set
			{
				mainSource.volume = value;
				for (var i =0; i < concurrentSources.Count; i++)
				{
					concurrentSources[i].volume = value;
				}
			}
			get
			{
				return mainSource.volume;
			}
		}

		/// <summary>
		/// 是否循环（主音源）
		/// </summary>
		public bool Loop
		{
			get
			{
				return loop;
			}
			set
			{
				loop = value;
				mainSource.loop = loop;
			}
		}

		/// <summary>
		/// 是否有任一音源在播放
		/// </summary>
		public bool Playing
		{
			get
			{
				if (mainSource.isPlaying)
				{
					return true;
				}
				for (var i =0; i < concurrentSources.Count; i++)
				{
					if (concurrentSources[i].isPlaying)
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// 播放主音源音频并返回音源
		/// </summary>
		public AudioSource PlayClip(AudioClip clip)
		{
			if (clip == null)
			{
				return null;
			}
			mainSource.clip = clip;
			mainSource.Play();
			return mainSource;
		}

		/// <summary>
		/// 并发播放一个音频（不影响主音源），返回使用的音源
		/// </summary>
		public AudioSource PlayConcurrent(AudioClip clip)
		{
			if (clip == null)
			{
				return null;
			}
			var src = GetConcurrentSource();
			src.clip = clip;
			src.loop = false;
			src.Play();
			return src;
		}

		/// <summary>
		/// 设置速度更新模式（全部音源）
		/// </summary>
		public void IgnoreTimeScale(bool ignore)
		{
			mainSource.velocityUpdateMode = ignore ? AudioVelocityUpdateMode.Dynamic : AudioVelocityUpdateMode.Fixed;
			for (var i =0; i < concurrentSources.Count; i++)
			{
				concurrentSources[i].velocityUpdateMode = ignore ? AudioVelocityUpdateMode.Dynamic : AudioVelocityUpdateMode.Fixed;
			}
		}

		/// <summary>
		/// 停止全部音源
		/// </summary>
		public void StopAll()
		{
			mainSource.Stop();
			for (var i =0; i < concurrentSources.Count; i++)
			{
				concurrentSources[i].Stop();
			}
		}

		/// <summary>
		/// 暂停全部音源
		/// </summary>
		public void PauseAll()
		{
			if (mainSource.isPlaying)
			{
				mainSource.Pause();
			}
			for (var i =0; i < concurrentSources.Count; i++)
			{
				var src = concurrentSources[i];
				if (src.isPlaying)
				{
					src.Pause();
				}
			}
		}

		/// <summary>
		/// 恢复全部音源
		/// </summary>
		public void ResumeAll()
		{
			mainSource.UnPause();
			for (var i =0; i < concurrentSources.Count; i++)
			{
				concurrentSources[i].UnPause();
			}
		}

		/// <summary>
		/// 遍历处于播放状态的音源
		/// </summary>
		public void ForEachActiveSource(Action<AudioSource> action)
		{
			if (action == null)
			{
				return;
			}
			if (mainSource.isPlaying)
			{
				action(mainSource);
			}
			for (var i =0; i < concurrentSources.Count; i++)
			{
				var src = concurrentSources[i];
				if (src.isPlaying)
				{
					action(src);
				}
			}
		}

		/// <summary>
		/// 停止指定剪辑
		/// </summary>
		public void StopClip(string clipName)
		{
			if (string.IsNullOrEmpty(clipName))
			{
				return;
			}
			if (mainSource.clip != null && mainSource.clip.name == clipName)
			{
				mainSource.Stop();
			}
			for (var i =0; i < concurrentSources.Count; i++)
			{
				var src = concurrentSources[i];
				if (src.clip != null && src.clip.name == clipName)
				{
					src.Stop();
				}
			}
		}
	}
}

