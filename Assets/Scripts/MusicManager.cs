using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _fightMusic;

    [SerializeField] private float _maxVolume = 0.25f;
    [SerializeField] private float _timeToFade = 2.5f;
    
    private AudioSource _track01, _track02;
    private bool _isPlayingMenu;

    private bool _isFirst = true;
    
    private void Awake()
    
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
        
        DontDestroyOnLoad(transform.gameObject);
        _track01 = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _track01 = gameObject.AddComponent<AudioSource>();
        _track02 = gameObject.AddComponent<AudioSource>();
        
        _track01.loop = true;
        _track02.loop = true;
        
        _isPlayingMenu = true;

        _track01.clip = _menuMusic;
        _track02.clip = _fightMusic;
        
        SwapTrack(true);
    }

    public void SwapTrack(Boolean isMenu)
    {
        StopAllCoroutines();
        
        StartCoroutine(FadeTrack(isMenu));
        
        if(!_isFirst) _isPlayingMenu = !_isPlayingMenu;
    }

    private IEnumerator FadeTrack(Boolean isMenu)
    {
        if (_isFirst)
        {
            _track01.Play();
            _track02.Pause();
            _isFirst = false;
            yield break;
        }
        
        float timeElapsed = 0;
        
        if (_isPlayingMenu == true)
        {
            _track01.Play();
            
            while(timeElapsed < _timeToFade)
            {
                _track01.volume = Mathf.Lerp(0, _maxVolume, timeElapsed / _timeToFade);
                _track02.volume = Mathf.Lerp(_maxVolume, 0, timeElapsed / _timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            
            _track02.Pause();
        }
        else
        {
            _track02.Play();
            while(timeElapsed < _timeToFade)
            {
                _track01.volume = Mathf.Lerp(_maxVolume, 0, timeElapsed / _timeToFade);
                _track02.volume = Mathf.Lerp(0, _maxVolume, timeElapsed / _timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            _track01.Pause();
        }
    }
}
