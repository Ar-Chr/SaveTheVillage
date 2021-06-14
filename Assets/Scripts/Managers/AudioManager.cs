using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public void Play(Sound sound)
    {
        sound.source = gameObject.AddComponent<AudioSource>();
        sound.source.clip = sound.clip;

        sound.source.loop = sound.loop;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;

        sound.source.Play();
        if (!sound.loop)
            Destroy(sound.source, sound.clip.length);
    }
}
