using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [SerializeField] private GameObject _healthPrefab;
    [SerializeField] private Sprite _emptyHeart;
    [SerializeField] private int _maxHealth = 3; // Initial health
    [SerializeField] private float _invincibilityDuration = 1.0f; // Duration of invincibility frames
    
    private int _currentHealth;
    private bool _isInvincible = false;
    private SpriteRenderer _sr;
    private Color _invincibleColor = Color.clear;
    private GameObject _healthHolder;
    private SpriteRenderer[] _healthContainers;
    private bool _canFade;

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

        _currentHealth = _maxHealth;
        _sr = GetComponent<SpriteRenderer>();
        _healthHolder = transform.GetChild(1).gameObject;
        
        // Health Setup
        _healthContainers = new SpriteRenderer[_maxHealth];
        for (int i = 0; i < _maxHealth; i++)
        {
            GameObject healthContainer = Instantiate(_healthPrefab, transform.position, Quaternion.identity, _healthHolder.transform);
            _healthContainers[i] = healthContainer.GetComponent<SpriteRenderer>();
        }

        // Changes y pos based on how many hearts there are.
        float inc = Mathf.Ceil(_maxHealth / 3f) * 0.25f;
        
        if (_maxHealth is 1 or 2)
        {
            inc = 0.25f;
        }
        
        _healthHolder.transform.localPosition = new Vector2(0, 1.5f + inc);
    }

    private void Start()
    {
        Invoke("SetCanFade", 1f);
    }

    private void Update()
    {
        if (_canFade)
        {
            foreach (SpriteRenderer hp in _healthContainers)
            {
                hp.color = Color.Lerp(hp.color, Color.clear, Time.deltaTime * 8);
            }
        }
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public void IncreaseHealth(int amount)
    {
        if (_currentHealth + amount <= _maxHealth)
        {
            _currentHealth += amount;
            SetOpacity();
        }
    }

    public void DecreaseHealth(int amount)
    {
        if (!_isInvincible)
        {
            switch (_currentHealth - amount)
            {
                case > 0:
                    _currentHealth -= amount;
                    UpdateHealthContainersColor();
                    StartInvincibility();
                    break;
                case 0:
                    _currentHealth -= amount;
                    UpdateHealthContainersColor();
                    Death();
                    break;
                default:
                    _currentHealth = 0;
                    UpdateHealthContainersColor();
                    Death();
                    break;
            }
            SetOpacity();
        }
    }

    private void UpdateHealthContainersColor()
    {
        for (int i = 0; i < _healthContainers.Length; i++)
        {
            if (i >= _currentHealth)
            {
                _healthContainers[i].sprite = _emptyHeart;
            }
        }
    }

    private void StartInvincibility()
    {
        _isInvincible = true;
        // Start a coroutine to handle the invincibility duration
        StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _invincibilityDuration)
        {
            float t = Mathf.PingPong(elapsedTime * 5, _invincibilityDuration) / _invincibilityDuration;
            _sr.color = Color.Lerp(Color.white, _invincibleColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _isInvincible = false;
        _sr.color = Color.white; // Reset to original color
    }

    private void SetOpacity()
    {
        _canFade = false;
        foreach (SpriteRenderer hp in _healthContainers)
        {
            hp.color = Color.white;;
        }
        Invoke("SetCanFade", 1f);
    }

    private void SetCanFade()
    {
        _canFade = true;
    }

    public void SetInvincible()
    {
        _isInvincible = true;
        StopAllCoroutines();
    }
    
    private void Death()
    {
        ShockwaveManager.Instance.CallShockwave(PlayerController.Instance.transform.position);
        PlayerController.Instance.CanMove = false;
        PlayerController.Instance.HideDashBar();
        GetComponent<Animator>().SetTrigger("Death");
        DeathMarkManager.Instance.PlayerDeath();
        
    }
}
