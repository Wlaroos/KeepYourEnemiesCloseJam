using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3; // Initial health
    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
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
        switch (_currentHealth - amount)
        {
            case > 0:
                _currentHealth -= amount;
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

    private void Death()
    {
        Debug.Log("Death");
    }
}