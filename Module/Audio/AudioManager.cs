using Framework.Module.Resource;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Module.Audio
{
    public class AudioManager : ModuleBase, IAudioManager
    {
        float volume = 1f;

        ConcurrentDictionary<string, AudioClip> Clips = new ConcurrentDictionary<string, AudioClip>();
        ConcurrentDictionary<int, IAudioChannel> Channels = new ConcurrentDictionary<int, IAudioChannel>();

        public override void OnInit()
        {
            InitChannel(5);
            base.OnInit();
        }

        void InitChannel(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GetChannel(i);
            }
        }

        IAudioChannel GetChannel(int channelId)
        {
            if (Channels.ContainsKey(channelId))
            {
                return Channels[channelId];
            }

            AudioChannel channel = new AudioChannel(channelId, false);
            if (channel.ChannelId == 0)
            {
                channel.Loop = true;
            }
            Channels.TryAdd(channelId, channel);
            return channel;
        }

        void ChangeChannelVolume(int channelId, float volume)
        {
            if (!Channels.ContainsKey(channelId))
            {
                return;
            }
            Channels[channelId].Volume = volume;
        }

        async Task<AudioClip> GetAudioClip(string clipName)
        {
            if (Clips.ContainsKey(clipName))
            {
                return Clips[clipName];
            }

            IAsset asset = await ModuleManager.GetModule<IResourceManager>().LoadAsync<AudioClip>(clipName);
            Clips.TryAdd(clipName, asset.asset as AudioClip);
            asset.Retain();
            return asset.asset as AudioClip;
        }

        public void SetChannelIgnoreTimeScale(int channelId, bool ingnore)
        {
            IAudioChannel channel = GetChannel(channelId);
            channel.IgnoreTimeScale(ingnore);
        }

        public async void PlayAudio(int channelId, string clipName)
        {
            AudioClip clip = await GetAudioClip(clipName);
            IAudioChannel channel = GetChannel(channelId);
            channel.PlayClip(clip);
            //channel.Volume = volume;
        }

        public async void PlayAudioNotReplace(string clipName)
        {
            AudioClip clip = await GetAudioClip(clipName);
            foreach (var channel in Channels)
            {
                if (channel.Value.Playing)
                {
                    continue;
                }

                if(channel.Key <= 10)
                {
                    continue;
                }

                channel.Value.PlayClip(clip);
                return;
            }

            int maxChannel = 11;
            foreach(var id in Channels.Keys)
            {
                if(id > maxChannel)
                {
                    maxChannel = id;
                }
            }

            IAudioChannel newChannel = GetChannel(maxChannel + 1);
            newChannel.PlayClip(clip);
            newChannel.Volume = volume;
        }

        public void PlayAudio(ChannelType channelType, string clipName)
        {
            int channelId = (int)channelType;
            PlayAudio(channelId, clipName);
        }

        public void ChangeVolume(int channelId, float volume)
        {
            if (!Channels.ContainsKey(channelId))
            {
                return;
            }
            GetChannel(channelId).Volume = volume;
            this.volume = volume;
        }

        public void ChangeVolume(ChannelType channelType, float volume)
        {
            int channelId = (int)channelType;
            ChangeVolume(channelId, volume);
            this.volume = volume;
        }

        public void ChangeBgVolume(float volume)
        {
            ChangeVolume(ChannelType.BG, volume);
        }

        public void ChangeExceptBgVolume(float volume)
        {
            foreach (var key in Channels.Keys)
            {
                if (key == (int)ChannelType.BG)
                {
                    continue;
                }
                Channels[key].Volume = volume;
            }
        }

        public void ChangeAllVolume(float volume)
        {
            foreach (var key in Channels.Keys)
            {
                Channels[key].Volume = volume;
            }

            this.volume = volume;
        }
    }
}