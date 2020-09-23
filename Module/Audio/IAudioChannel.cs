using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Audio
{
    public interface IAudioChannel
    {
        int ChannelId { get; }
        float Volume { get; set; }
        bool Loop { get; set; }
        bool Playing { get; }
        void PlayClip(AudioClip clip);
        void IgnoreTimeScale(bool ignore);
    }
}