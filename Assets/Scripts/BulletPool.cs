using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab; // The bullet prefab to be pooled.
    [SerializeField] private int _poolSize = 20; // The number of bullets to initially create and pool.

    private List<GameObject> _bulletPool; // The list to hold pooled bullets.

    private void Start()
    {
        // Initialize the bullet pool.
        _bulletPool = new List<GameObject>();

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject bullet = Instantiate(_bulletPrefab, transform);
            
            if (bullet.GetComponent<Projectile>())
            {
                bullet.GetComponent<Projectile>().SetBulletPool(this);
            }
            
            bullet.SetActive(false); // Deactivate the bullet initially.
            _bulletPool.Add(bullet);
        }
    }

    // Method to get a bullet from the pool.
    public GameObject GetBullet()
    {
        // Iterate through the bullet pool to find an inactive bullet.
        foreach (GameObject bullet in _bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }

        // If no inactive bullets are found, create a new one and add it to the pool.
        GameObject newBullet = Instantiate(_bulletPrefab, transform);
        
        if (newBullet.GetComponent<Projectile>())
        {
            newBullet.GetComponent<Projectile>().SetBulletPool(this);
        }
        
        newBullet.SetActive(true);
        _bulletPool.Add(newBullet);
        
        return newBullet;
    }

    // Method to return a bullet to the pool.
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
    }
}