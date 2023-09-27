using System;
using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool _canMove;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxDistanceFromPlayer;
    [Header("Bullet Shape And Color")] 
    [SerializeField] private Sprite _bulletSprite;
    [SerializeField] private Material _bulletMat;
    [Header("Bullet Info")] 
    [SerializeField] private float _bulletMoveSpeed;
    [SerializeField] private float _startingDistance = 0.1f;
    [Header("Burst Info")]
    [SerializeField] private int _burstCount = 1;
    [SerializeField] private int _projectilesPerBurst = 1;
    [SerializeField][Range(0,359)] private float _angleSpread = 0;
    [SerializeField] private float _timeBetweenBursts = .25f;
    [Header("Delay Between Bursts")]
    [SerializeField] private float _restTime = 1f;
    [Header("Stagger/Oscilate")] 
    [SerializeField] private bool _stagger;
    [SerializeField] private bool _oscillate;
    [Header("Animation Stuff")] 
    [SerializeField] private bool _attackEachBurst;
    [SerializeField] private float _attackStartDelay = 0.5f;

    private bool _isShooting = false;
    private bool _isMarkActivated = false;
    private bool _isDelayed = true;
    
    private SpriteRenderer _sr;
    private Transform _shootPos;
    private float _originalXValue;

    private Animator _anim;

    private void Update()
    {
        if (!_isShooting && !_isMarkActivated)
        {
            Flip();
        }
        Attack();
    }

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _shootPos = transform.GetChild(0);
        _originalXValue = _shootPos.localPosition.x;
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Invoke(nameof(DelayedStart), _attackStartDelay);
    }

    public void Attack()
    {
        if (!_isShooting && !_isMarkActivated && !_isDelayed)
        {
            _anim.SetTrigger("Attack");
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        _isShooting = true;

        float startAngle, currentAngle, angleStep, endAngle;
        float timeBetweenProjectiles = 0f;
        
        TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);

        if (_stagger) { timeBetweenProjectiles = _timeBetweenBursts / _projectilesPerBurst; }

        for (int i = 0; i < _burstCount; i++)
        {
            
            if (!_oscillate)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }
            
            if (_oscillate && i % 2 != 1)
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            }
            else if(_oscillate)
            {
                currentAngle = endAngle;
                endAngle = startAngle;
                startAngle = currentAngle;
                angleStep *= -1;
            }
            
            if (!_isMarkActivated)
            {
                for (int j = 0; j < _projectilesPerBurst; j++)
                {
                    Vector2 pos = FindBulletSpawnPos(currentAngle);
                    //GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
                    GameObject bullet = BulletPool.Instance.GetBullet();
                    bullet.GetComponent<SpriteRenderer>().sprite = _bulletSprite;
                    bullet.GetComponent<SpriteRenderer>().material = _bulletMat;
                    bullet.transform.position = pos;
                    bullet.transform.right = bullet.transform.position - _shootPos.position;
                    
                    if (bullet.TryGetComponent(out Projectile projectile))
                    {
                        projectile.UpdateMoveSpeed(_bulletMoveSpeed);
                    }

                    currentAngle += angleStep;
                    
                    if (_stagger) { yield return new WaitForSeconds(timeBetweenProjectiles); }
                }

                currentAngle = startAngle;

                if(!_stagger) {yield return new WaitForSeconds(_timeBetweenBursts);}
            }
        }
        yield return new WaitForSeconds(_restTime);
        _isShooting = false;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - _shootPos.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        startAngle = targetAngle;
        endAngle = targetAngle;
        currentAngle = targetAngle;
        float halfAngleSpread = 0f;
        angleStep = 0f;

        if (_angleSpread != 0)
        {
            angleStep = _angleSpread / (_projectilesPerBurst - 1);
            halfAngleSpread = _angleSpread / 2f;
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            currentAngle = startAngle;
        }
    }

    private Vector2 FindBulletSpawnPos(float currentAngle)
    {
        float x = _shootPos.position.x + _startingDistance * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
        float y = _shootPos.position.y + _startingDistance * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

        Vector2 pos = new Vector2(x, y);
        
        return pos;
    }

    public void MarkActivated()
    {
        _isMarkActivated = true;
    }

    public void BackToIdle()
    {
        _anim.SetTrigger("Idle");
    }
    
    private void Flip()
    {
        Vector3 directionToPlayer = PlayerController.Instance.transform.position - transform.position;

        // Flip the sprite based on the direction
        if (directionToPlayer.x > 0)
        {
            _sr.flipX = false; // Face right
            _shootPos.localPosition = new Vector2(_originalXValue,_shootPos.localPosition.y);
        }
        else
        {
            _sr.flipX = true; // Face left
            _shootPos.localPosition = new Vector2(-_originalXValue,_shootPos.localPosition.y);
        }
    }

    private void DelayedStart()
    {
        _isDelayed = false;
    }
}
