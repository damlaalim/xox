using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource effectSource;

    [SerializeField] private AudioClip chooseState;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip line;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip draw;

    private void Awake()
    {
        Instance = this;
    }

    public void Play(AudioType type)
    {
        switch (type)
        {
            case AudioType.ChooseState:
                effectSource.clip = chooseState;
                effectSource.Play();
                break;
            case AudioType.Click:
                effectSource.clip = click;
                effectSource.Play();
                break;
            case AudioType.Line:
                effectSource.clip = line;
                effectSource.Play();
                break;
            case AudioType.Win:
                effectSource.clip = win;
                effectSource.Play();
                break;
            case AudioType.Draw:
                effectSource.clip = draw;
                effectSource.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}

public enum AudioType
{
    ChooseState,
    Click,
    Line,
    Win,
    Draw
}