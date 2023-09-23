using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 10f;
    [Header("Dash")]
    [SerializeField] private float _dashDistance = 30f;
    [SerializeField] private float _dashDuration = 0.1f;
    [SerializeField] private float _dashCooldown = 1f;
    [Header("Animations")]
    [SerializeField] private bool _startWithSword;
    [Space(10)]
    [SerializeField] private AnimatorController _ac;
    [SerializeField] private AnimatorController _acSword;

    // Components
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Animator _anim;
    private TrailRenderer _tr;
    
    //Movement and Dash Vectors
    private Vector2 _movement;
    private Vector2 _dashDirection;

    // Movement and Dash booleans
    public bool CanMove { get; private set; }
    public bool IsDashing { get; private set; }
    private bool _canDash = true;
    
    // Color
    private Color _originalColor;

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
        
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _tr = GetComponent<TrailRenderer>();

        _anim.runtimeAnimatorController = _startWithSword ? _acSword : _ac;

        CanMove = true;

        _originalColor = _sr.color;
    }

    private void Update()
    {
        // Handle dashing
        if (Input.GetButtonDown("Dash") && !IsDashing && _canDash && CanMove)
        {
            StartCoroutine(Dash());
        }
        
        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PickUpSword();
        }
    }

    private void FixedUpdate()
    {
        // Move the character
        if (CanMove)
        {
            MoveCharacter(_movement);
        }
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
        _anim.SetBool("isDashing",true);
        _anim.SetTrigger("Dash");

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
    
    // Animation Event
    private void DashEnd()
    { 
        _anim.SetBool("isDashing",false); 
    }

    private void UnsheathEnd()
    {
        _anim.SetTrigger("Aura"); 
    }
    
    // Changes Animation Controller
    public void PickUpSword()
    {
        _anim.runtimeAnimatorController = _acSword;
    }

    public void ReadyToExecute()
    {
        CanMove = false;
        _anim.SetTrigger("Unsheath");
    }
}