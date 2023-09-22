using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _dashDistance = 30f;
    [SerializeField] private float _dashDuration = 0.1f;
    [SerializeField] private float _dashCooldown = 1f;

    [SerializeField] private AnimatorController _ac;
    [SerializeField] private AnimatorController _acSword;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Animator _anim;
    private TrailRenderer _tr;
    
    private Vector2 _movement;
    private Vector2 _dashDirection;

    public bool IsDashing = false;
    private bool _canDash = true;
    
    private Color _originalColor;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _tr = GetComponent<TrailRenderer>();
        
        _anim.runtimeAnimatorController = _ac;
        _originalColor = _sr.color;
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
    }

    private void Update()
    {
        // Handle dashing
            if (Input.GetButtonDown("Dash") && !IsDashing && _canDash)
            {
                _anim.SetBool("isDashing",true);
                StartCoroutine(Dash());
            }
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                PickUpSword();
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
        
        _anim.SetFloat("Speed", Mathf.Abs(_movement.magnitude));
        
        
        // FLIPPING SPRITE
        if (_movement.x > 0)
        {
            // Player is moving right, flip the sprite to face right
            _sr.flipX = false;
        }
        else if (_movement.x < 0)
        {
            // Player is moving left, flip the sprite to face left
            _sr.flipX = true;
        }
        
        // Store dash direction regardless of movement
        if (_movement != Vector2.zero)
        {
            _dashDirection = _movement;
        }
        
        if (!IsDashing)
        {
            _rb.MovePosition(_rb.position + direction * _moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Dash()
    {
        IsDashing = true;
        _tr.emitting = true;

        float elapsedTime = 0f;
        float dashSpeed = (_dashDistance / _dashDuration);

        while (elapsedTime < _dashDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate velocity and clamp it
            Vector2 velocity = _dashDirection * dashSpeed;
            velocity = Vector2.ClampMagnitude(velocity, dashSpeed * 0.9f);

            _rb.MovePosition(_rb.position + velocity * Time.deltaTime);
            yield return null;
        }

        IsDashing = false;
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

    private void DashEnd()
    { 
        _anim.SetBool("isDashing",false); 
    }

    public void PickUpSword()
    {
        _anim.runtimeAnimatorController = _acSword;
    }
}