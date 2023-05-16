using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField] private List<DataSound> _dataSounds = new List<DataSound>();
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(string clipName)
    {
        var audioClip = GetAudioClip(clipName);
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    public void PlaySoundEffect()
    {       
        if (_audioSource.clip != null)
            _audioSource.Play();
    }

    private AudioClip GetAudioClip(string clipName)
    {
        AudioClip clip = null;

        foreach (var sound in _dataSounds.Where(sound => sound.name == clipName))
        {
            clip = sound.audioClip;
        }
        return clip;
    }
    public string GetAudioClipName()
    {
        string name = string.Empty;
        foreach (var sound in _dataSounds.Where(sound => sound.audioClip == _audioSource.clip))
        {
            name = sound.name;
        }
        return name;
    }

    public AudioSource GetAudioSource()
    {
        return _audioSource;
    }
    
    public void SetAudioSource(AudioSource source)
    {
        _audioSource = source;
    }

    [Serializable]
    private class DataSound
    {
        public string name;
        public AudioClip audioClip;
    }
}
