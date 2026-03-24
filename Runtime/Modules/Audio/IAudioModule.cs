using System.Threading.Tasks;
using Framework.Module.Resource;

namespace Framework.Module.Audio
{
	/// <summary>
	/// 音频模块接口，提供频道播放（支持循环与缓入）、并发播放、动态临时播放、音量、暂停/恢复与停止（支持缓出）。
	/// </summary>
	public interface IAudioModule
	{
		/// <summary>
		/// 设置资源加载器
		/// </summary>
		void SetResourcesLoader(IResourceLoader resourceLoader);
		/// <summary>
		/// 设置枚举频道速度更新模式（动态或固定）
		/// </summary>
		void SetVelocityUpdateMode(ChannelType channel, bool dynamicMode);
		/// <summary>
		/// 设置整型频道速度更新模式（动态或固定）
		/// </summary>
		void SetVelocityUpdateMode(int channelId, bool dynamicMode);
		/// <summary>
		/// 异步播放枚举频道主音源；loop 为可选，null 表示不改变；fadeInSeconds 缓入秒数，默认0
		/// </summary>
		ValueTask PlayAsync(ChannelType channel, string clipName, bool? loop = null, float fadeInSeconds =0f);
		/// <summary>
		/// 异步播放整型频道主音源；loop 为可选，null 表示不改变；fadeInSeconds 缓入秒数，默认0
		/// </summary>
		ValueTask PlayAsync(int channelId, string clipName, bool? loop = null, float fadeInSeconds =0f);
		/// <summary>
		/// 异步并发播放枚举频道（不替换主音源）；loop 为可选；fadeInSeconds 缓入秒数，默认0
		/// </summary>
		ValueTask PlayConcurrentAsync(ChannelType channel, string clipName, bool? loop = null, float fadeInSeconds =0f);
		/// <summary>
		/// 异步并发播放整型频道（不替换主音源）；loop 为可选；fadeInSeconds 缓入秒数，默认0
		/// </summary>
		ValueTask PlayConcurrentAsync(int channelId, string clipName, bool? loop = null, float fadeInSeconds =0f);
		/// <summary>
		/// 异步动态频道播放（临时音效）
		/// </summary>
		ValueTask PlayDynamicAsync(string clipName);
		/// <summary>
		/// 设置枚举频道音量（0-1）
		/// </summary>
		void SetVolume(ChannelType channel, float volume);
		/// <summary>
		/// 设置整型频道音量（0-1）
		/// </summary>
		void SetVolume(int channelId, float volume);
		/// <summary>
		/// 设置所有频道音量（0-1）
		/// </summary>
		void SetVolumeAll(float volume);
		/// <summary>
		/// 设置除指定枚举频道外其他频道音量（0-1）
		/// </summary>
		void SetVolumeExcept(ChannelType excludedChannel, float volume);
		/// <summary>
		/// 停止枚举频道全部播放；fadeOutSeconds 缓出秒数，默认0
		/// </summary>
		void StopChannel(ChannelType channel, float fadeOutSeconds =0f);
		/// <summary>
		/// 停止整型频道全部播放；fadeOutSeconds 缓出秒数，默认0
		/// </summary>
		void StopChannel(int channelId, float fadeOutSeconds =0f);
		/// <summary>
		/// 停止枚举频道中指定音频剪辑
		/// </summary>
		void StopClip(ChannelType channel, string clipName);
		/// <summary>
		/// 停止整型频道中指定音频剪辑
		/// </summary>
		void StopClip(int channelId, string clipName);
		/// <summary>
		/// 暂停枚举频道全部播放
		/// </summary>
		void PauseChannel(ChannelType channel);
		/// <summary>
		/// 暂停整型频道全部播放
		/// </summary>
		void PauseChannel(int channelId);
		/// <summary>
		/// 恢复枚举频道全部播放
		/// </summary>
		void ResumeChannel(ChannelType channel);
		/// <summary>
		/// 恢复整型频道全部播放
		/// </summary>
		void ResumeChannel(int channelId);
		/// <summary>
		/// 停止所有频道播放
		/// </summary>
		void StopAll();
	}
}