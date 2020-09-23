using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FrameAnimation : MonoBehaviour
{
    public List<Sprite> spriteList = new List<Sprite>();
    public float intervals = 0.1f;
    public bool loop = true;
    public bool playOnAwake = true;

    float currentTime = 0;

    Image image;

    private void Awake()
    {
        image = transform.GetComponent<Image>();
        if (playOnAwake)
        {
            Play();
        }
    }

    async void Play()
    {
        for(int i = 0; i < spriteList.Count; i++)
        {
            if(image == null)
            {
                return;
            }

            image.sprite = spriteList[i];
            await Task.Delay((int)(intervals * 1000));
        }

        Play();
    }
}
