using UnityEngine;

namespace Framework.Module.Audio
{
    public class AudioChannel
    {
        public int ChannelId { get; private set; }
        AudioSource source;
        float volume;
        bool loop;
        GameObject gameObject;

        public AudioChannel(int channelId, bool loop)
        {
            gameObject = GameObject.Find("[AudioChannel]");
            if (gameObject == null)
            {
                gameObject = new GameObject("[AudioChannel]");
                Object.DontDestroyOnLoad(gameObject);
            }

            source = gameObject.AddComponent<AudioSource>();

            ChannelId = channelId;
            this.loop = loop;
        }

        public float Volume
        {
            set
            {
                source.volume = value;
                volume = value;
            }
            get
            {
                return volume;
            }
        }

        public bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loop = value;
                source.loop = loop;
            }
        }

        public bool Playing
        {
            get
            {
                return source.isPlaying;
            }
        }

        public void PlayClip(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }
            source.clip = clip;
            source.Play();
        }

        public void IgnoreTimeScale(bool ignore)
        {
            if (ignore)
            {
                source.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
            }
            else
            {
                source.velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
            }
        }
    }
}

