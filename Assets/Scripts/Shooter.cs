using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private BulletPool _bulletPool;
    [SerializeField] private float _bulletMoveSpeed;
    [SerializeField] private int _burstCount;
    [SerializeField] private float _timeBetweenBursts;
    [SerializeField] private float _restTime = 1f;

    private bool _isShooting = false;

    private void Update()
    {
        Attack();
    }

    public void Attack()
    {
        if (!_isShooting)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        _isShooting = true;

        for (int i = 0; i < _burstCount; i++)
        {
            Vector2 _targetDirection = PlayerController.Instance.transform.position - transform.position;

            //GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            GameObject bullet = _bulletPool.GetBullet();
            bullet.transform.position = transform.position;
            bullet.transform.right = _targetDirection;

            if (bullet.TryGetComponent(out Projectile projectile))
            {
                projectile.UpdateMoveSpeed(_bulletMoveSpeed);
            }

            yield return new WaitForSeconds(_timeBetweenBursts);
        }

        yield return new WaitForSeconds(_restTime);
        _isShooting = false;
    }
}
