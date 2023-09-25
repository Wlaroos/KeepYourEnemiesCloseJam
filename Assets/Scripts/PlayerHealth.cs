using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3; // Initial health
    [SerializeField] private float _invincibilityDuration = 1.0f; // Duration of invincibility frames
    private int _currentHealth;
    private bool _isInvincible = false;
    private SpriteRenderer _sr;
    private Color _invincibleColor = Color.clear;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _sr = GetComponent<SpriteRenderer>();
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
                    StartInvincibility();
                    break;
                case 0:
                    _currentHealth -= amount;
                    Death();
                    break;
                default:
                    _currentHealth = 0;
                    Death();
                    break;
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

    private void Death()
    {
        Debug.Log("Death");
    }
}