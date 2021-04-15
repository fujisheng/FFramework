using Framework.Service.Resource;

namespace Framework.Service.Audio
{
    public interface IAudioService
    {
        void SetResourcesLoader(IResourceLoader resourceLoader);
        void SetChannelIgnoreTimeScale(int channelId, bool ignore);
        void PlayAudio(int channelId, string clipName);
        void PlayAudio(ChannelType channelType, string clipName);
        void PlayAudioNotReplace(string clipName);
        void ChangeVolume(int channelId, float volume);
        void ChangeVolume(ChannelType channelType, float volume);
        void ChangeBgVolume(float volume);
        void ChangeExceptBgVolume(float volume);
        void ChangeAllVolume(float volume);
    }
}