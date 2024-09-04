using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public AudioClip Clip;
    public AudioMixerGroup Mixer;
    public string Name;
    public bool Loop;
    public bool FadeIn;
    public float FadeInTime;
    public bool FadeOut;
    public float FadeOutTime;
    [Range(0f, 1f)]
    public float Volume = 1;
    [Range(-3f, 3f)]
    public float Pitch = 1;
    [HideInInspector] public AudioSource Source;
    [HideInInspector] public Coroutine FadeInCoroutine, FadeOutCoroutine;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public Sound[] Sounds;
    
     private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        
        AddSources();
    }

     void AddSources()
     {
         foreach (var sound in Sounds)
         {
             var source = gameObject.AddComponent<AudioSource>();
             source.clip = sound.Clip;
             source.outputAudioMixerGroup = sound.Mixer;
             source.loop = sound.Loop;
             source.volume = sound.FadeIn ? 0 : sound.Volume;
             source.pitch = sound.Pitch;
             sound.Source = source;
         }
     }

    public void PlaySound(string clipName)
    {
        var sound = Array.Find(Sounds, x => x.Name == clipName);
        if (sound.FadeInCoroutine != null)
        {
            StopCoroutine(sound.FadeInCoroutine);
            sound.FadeInCoroutine = null;
        }
        if (sound.FadeOutCoroutine != null)
        {
            StopCoroutine(sound.FadeOutCoroutine);
            sound.FadeOutCoroutine = null;
        }

        if (sound.FadeIn)
            sound.FadeInCoroutine = StartCoroutine(FadeIn(sound));
        else
            sound.Source.volume = sound.Volume;
        sound.Source.Play();
    }

    IEnumerator FadeIn(Sound sound)
    {
        float lerpPosition = sound.Source.volume / sound.Volume;
        while (lerpPosition < 1)
        {
            lerpPosition += Time.unscaledDeltaTime / sound.FadeInTime;
            sound.Source.volume = Mathf.Lerp(0f, sound.Volume, lerpPosition);
            yield return null;
        }
        sound.FadeInCoroutine = null;
    }

    public void StopSound(string clipName)
    {
        var sound = Array.Find(Sounds, x => x.Name == clipName);
        if (sound.FadeInCoroutine != null)
        {
            StopCoroutine(sound.FadeInCoroutine);
            sound.FadeInCoroutine = null;
        }
        if (sound.FadeOutCoroutine != null)
        {
            StopCoroutine(sound.FadeOutCoroutine);
            sound.FadeOutCoroutine = null;
        }
        if (sound.FadeOut)
            sound.FadeOutCoroutine = StartCoroutine(FadeOut(sound));
        else sound.Source.Stop();
    }

    IEnumerator FadeOut(Sound sound)
    {
        float lerpPosition = sound.Source.volume / sound.Volume;
        while (lerpPosition > 0)
        {
            lerpPosition -= Time.unscaledDeltaTime / sound.FadeOutTime;
            sound.Source.volume = Mathf.Lerp(0, sound.Volume, lerpPosition);
            yield return null;
        }
        sound.Source.Stop();
        sound.FadeOutCoroutine = null;
    }
}
