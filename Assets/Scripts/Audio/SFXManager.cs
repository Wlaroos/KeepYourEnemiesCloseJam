using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioClip _applyMarkSFX;
    [SerializeField] private AudioClip _dashSFX;
    [SerializeField] private AudioClip _destroyBulletSFX;
    [SerializeField] private AudioClip _destroyBulletDashSFX;
    [SerializeField] private AudioClip _enemyShootSFX;
    [SerializeField] private AudioClip _enemyDeathSFX;
    [SerializeField] private AudioClip _playerDeathSFX;
    [SerializeField] private AudioClip _playerExplodeSFX;
    [SerializeField] private AudioClip _slashSFX;
    [SerializeField] private AudioClip _bigSlashSFX;
    [SerializeField] private AudioClip _sheathSFX;
    [SerializeField] private AudioClip _shockwaveSFX;
    [SerializeField] private AudioClip _snapshotSFX;
    [SerializeField] private AudioClip _snapshot2SFX;

    [SerializeField] private int _initialPoolSize = 10;
    [SerializeField] private int _maxPoolSize = 25;

    private Queue<AudioSource> _audioSourcePool;
    private List<AudioSource> _activeAudioSources;
    private Transform _poolParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        _audioSourcePool = new Queue<AudioSource>(_initialPoolSize);
        _activeAudioSources = new List<AudioSource>(_initialPoolSize);
        _poolParent = new GameObject("SFXAudioSourcePool").transform;
        _poolParent.SetParent(transform);
        _poolParent.localPosition = Vector3.zero;

        for (int i = 0; i < _initialPoolSize; i++)
        {
            var source = CreateNewAudioSource();
            _audioSourcePool.Enqueue(source);
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        var go = new GameObject("PooledAudioSource");
        go.transform.SetParent(_poolParent);
        var source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    private AudioSource GetPooledAudioSource()
    {
        // Clean up finished sources
        for (int i = _activeAudioSources.Count - 1; i >= 0; i--)
        {
            if (!_activeAudioSources[i].isPlaying)
            {
                _audioSourcePool.Enqueue(_activeAudioSources[i]);
                _activeAudioSources.RemoveAt(i);
            }
        }

        if (_audioSourcePool.Count > 0)
        {
            var src = _audioSourcePool.Dequeue();
            _activeAudioSources.Add(src);
            return src;
        }
        else if (_activeAudioSources.Count + _audioSourcePool.Count < _maxPoolSize)
        {
            var src = CreateNewAudioSource();
            _activeAudioSources.Add(src);
            return src;
        }
        else
        {
            // If pool is exhausted, reuse the first available (may cut off sound)
            var src = _activeAudioSources[0];
            src.Stop();
            _activeAudioSources.RemoveAt(0);
            _activeAudioSources.Add(src);
            return src;
        }
    }

    public void PlayApplyMark() =>  PlaySFX(_applyMarkSFX,1f, 0.3f);
    public void PlayEnemyDeath() => PlaySFX(_enemyDeathSFX, 1f, 0.2f);
    public void PlayEnemyShoot() => PlaySFX(_enemyShootSFX, .33f, 0.2f);
    public void PlayPlayerDeath() => PlaySFX(_playerDeathSFX, 1f, 0f);
    public void PlayPlayerExplode() => PlaySFX(_playerExplodeSFX, 1f, 0f);
    public void PlaySlash() => PlaySFX(_slashSFX, 1f, 0.2f);
    public void PlayBigSlash() => PlaySFX(_bigSlashSFX, 1f, 0.2f);
    public void PlayDash() => PlaySFX(_dashSFX, 1f, 0.2f);
    public void PlayDestroyBullet() => PlaySFX(_destroyBulletSFX, 0.5f, 0.2f);
    public void PlayDestroyBulletDash() => PlaySFX(_destroyBulletDashSFX, 0.75f, 0.2f);
    public void PlaySheath() => PlaySFX(_sheathSFX, 1f, 0.2f);
    public void PlayShockwave() => PlaySFX(_shockwaveSFX, 1f, 0.2f);
    public void PlaySnapshot() => PlaySFX(_snapshotSFX, 1f, 0.2f);
    public void PlaySnapshot2() => PlaySFX(_snapshot2SFX, 1f, 0.2f);
    private void PlaySFX(AudioClip clip, float volume = 1f, float pitchVariance = 0.1f)
    {
        if (clip == null) return;
        var source = GetPooledAudioSource();
        source.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    private void PlaySFX(AudioClip[] clip, float volume = 1f, float pitchVariance = 0.1f)
    {
        if (clip == null) return;
        var source = GetPooledAudioSource();
        source.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        source.clip = RandomClip(clip);
        source.volume = volume;
        source.Play();
    }

    private AudioClip RandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
        {
            return null;
        }
        int index = Random.Range(0, clips.Length);
        return clips[index];
    }
}
