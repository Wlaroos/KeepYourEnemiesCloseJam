using System.Collections;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _dashDistance = 50f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    private Rigidbody2D _rb;
    private Vector2 _movement;
    private Vector2 _dashDirection;
    private bool _isDashing = false;
    private bool _canDash = true;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        Vector2 originalPosition = _rb.position;
        float elapsedTime = 0f;

        // Calculate the desired dash speed
        float dashSpeed = _dashDistance / _dashDuration;

        while (elapsedTime < _dashDuration)
        {
            elapsedTime += Time.deltaTime;
            Vector2 dashVelocity = _dashDirection * dashSpeed;

            // Clamp the dash velocity to a maximum value (e.g., 20 units per second)
            dashVelocity = ClampMag(dashVelocity, dashSpeed * 0.9f,-dashSpeed * 0.9f);

            // Update the position based on the clamped dash velocity
            _rb.MovePosition(_rb.position + dashVelocity * Time.deltaTime);
            yield return null;
        }

        _rb.MovePosition(originalPosition + _dashDirection * _dashDistance);
        _isDashing = false;

        // Start cooldown for the next dash
        StartCoroutine(DashCooldown());
    }


    private IEnumerator DashCooldown()
    {
        _canDash = false;
        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }

    private static Vector2 ClampMag(Vector2 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if(sm > (double)max * (double)max) return v.normalized * max;
        else if(sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }
}
