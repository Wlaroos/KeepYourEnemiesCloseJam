using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 22f;
    [SerializeField] private float _projectileLifeTime = 2f;
    [SerializeField] private GameObject _particleOnHit;

    private BulletPool _bulletPool;
    
    private void OnEnable()
    {
        //Invoke(nameof(RemoveProjectile), _projectileLifeTime);
    }

    private void Update()
    {
        MoveProjectile();
    }
    
    public void UpdateMoveSpeed(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHP = other.gameObject.GetComponent<PlayerHealth>();
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        
        if(!other.isTrigger && player && !player.IsDashing)
        {
            playerHP.DecreaseHealth(1);
            RemoveProjectile();
        }
        else if(!other.isTrigger && player && player.IsDashing)
        {
            // Nothing for now, maybe add special effect for dashing through a bullet
            Debug.Log("DASHED THROUGH");
        }
        
        if (other.CompareTag("Wall"))
        {
            RemoveProjectile();
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector2.right * Time.deltaTime *_moveSpeed);
    }

    public void SetBulletPool(BulletPool pool)
    {
        _bulletPool = pool;
    }

    private void RemoveProjectile()
    {
        //Maybe pool particles later
        Instantiate(_particleOnHit, transform.position, transform.rotation);
        _bulletPool.ReturnBullet(gameObject);
    }
}
