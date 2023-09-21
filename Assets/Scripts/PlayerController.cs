using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _dashDistance = 30f;
    [SerializeField] private float _dashDuration = 0.1f;
    [SerializeField] private float _dashCooldown = 1f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private TrailRenderer _tr;
    
    private Vector2 _movement;
    private Vector2 _dashDirection;
    
    private bool _isDashing = false;
    private bool _canDash = true;
    
    private Color _originalColor;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _tr = GetComponent<TrailRenderer>();
        _originalColor = _sr.color;
    }

    private void Update()
    {
        // Handle dashing
            if (Input.GetButtonDown("Dash") && !_isDashing && _canDash)
            {
                StartCoroutine(Dash());
            }
    }

    private void FixedUpdate()
    {
        // Move the character
        MoveCharacter(_movement);
    }

    private void MoveCharacter(Vector2 direction)
    {
        // Handle movement input
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        // Normalize the movement vector to ensure constant speed in all directions
        _movement.Normalize();

        // Store dash direction regardless of movement
        if (_movement != Vector2.zero)
        {
            _dashDirection = _movement;
        }
        
        if (!_isDashing)
        {
            _rb.MovePosition(_rb.position + direction * _moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Dash()
    {
        _isDashing = true;
        _tr.emitting = true;
        
        float elapsedTime = 0f;

        while (elapsedTime < _dashDuration)
        {
            elapsedTime += Time.deltaTime;
            _rb.MovePosition(_rb.position + _dashDirection * (_dashDistance / _dashDuration) * Time.deltaTime);
            yield return null;
        }
        
        _isDashing = false;
        _tr.emitting = false;

        // Start cooldown for the next dash
        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        _canDash = false;

        // Flash the sprite red
        _sr.color = Color.red;
        
        yield return new WaitForSeconds(_dashCooldown);

        // Reset the sprite color to the original color
        _sr.color = _originalColor;

        _canDash = true;
    }
}