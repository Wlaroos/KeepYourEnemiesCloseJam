using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private RuntimeAnimatorController _ac;
    [SerializeField] private RuntimeAnimatorController _acSword;
    [Header("Unlocks")] 
    [SerializeField] private bool _unlockedDash = false;
    [Header("Other")] 
    [SerializeField] private GameObject _teleTrail;
    
    // Components
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Animator _anim;
    private TrailRenderer _tr;
    private SpriteRenderer _dashBar;
    
    //Movement and Dash Vectors
    private Vector2 _movement;
    private Vector2 _dashDirection;

    // Movement and Dash booleans
    public bool CanMove { get; private set; }
    public bool IsDashing { get; private set; }
    private bool _canDash = true;
    private bool _isCharged;
    
    // Dash Input Buffer
    private float _dashBufferTime = 0.25f; // Adjust this value to set the buffer time in seconds
    private float _lastDashInputTime = -1000; // Store the time of the last dash input

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
        _dashBar = transform.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>();

        _anim.runtimeAnimatorController = _startWithSword ? _acSword : _ac;

        CanMove = true;
    }

    private void Start()
    {
        if(!_unlockedDash) {transform.GetChild(2).gameObject.SetActive(false);}
        // Dash trail gets fucky if I don't do this
        Instantiate(_teleTrail, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity, transform);
    }

    private void Update()
    {
        // Update the last dash input time
        if (Input.GetButtonDown("Dash") && _unlockedDash)
        {
            _lastDashInputTime = Time.time;
        }
        
        // Handle dashing with a buffer time
        if (Time.time <= _lastDashInputTime + _dashBufferTime && !IsDashing && _canDash && CanMove && _unlockedDash)
        {
            StartCoroutine(Dash());
        }
        
        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PickUpSword();
        }
        
        if (Input.GetButtonDown("Select")  && _isCharged && Time.timeScale < 1)
        {
            StartCoroutine(Teleporting());
            // Text Scales In
            transform.GetChild(0).GetComponent<TextWobble>().ScaleText(Vector2.zero, 0.5f);
            _anim.SetTrigger("CrouchIdle"); 
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
        
        // Flip Sprite
        Flip(_movement.x);
        
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
        _anim.SetBool("isDashing", true);
        _anim.SetTrigger("Dash");

        float elapsedTime = 0f;
        float dashSpeed = (_dashDistance / _dashDuration);

        while (elapsedTime < _dashDuration)
        {
            elapsedTime += Time.fixedDeltaTime;

            // Calculate velocity and clamp it
            Vector2 velocity = _dashDirection * dashSpeed;
            velocity = Vector2.ClampMagnitude(velocity, dashSpeed * 0.9f);

            _rb.MovePosition(_rb.position + velocity * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        IsDashing = false;
        _tr.emitting = false;

        // Start cooldown for the next dash
        StartCoroutine(DashCooldown());
    }

    
    private IEnumerator DashCooldown()
    {
        _canDash = false;
        Color oldColor = _dashBar.material.color;
        _dashBar.material.color = Color.red;

        // Calculate the increment needed for the dash bar to reach 0.85 over the cooldown duration
        float sizeIncrement = 0.85f / _dashCooldown;
        float currentSize = 0f;

        while (currentSize < 0.85f)
        {
            currentSize += sizeIncrement * Time.deltaTime;
            _dashBar.size = new Vector2(currentSize, 0.075f);

            yield return null;
        }

        _dashBar.size = new Vector2(0.85f, 0.075f);
        _dashBar.material.color = oldColor;
        _canDash = true;
    }

    
    // Changes Animation Controller
    public void PickUpSword()
    {
        _anim.runtimeAnimatorController = _acSword;
    }
    
    private IEnumerator Teleporting()
    {
        Time.timeScale = 1;
        
        // Zoom Camera Out
        CameraController.Instance.StartZoomIn(8, .5f);
        yield return new WaitForSeconds(.25f);
        
        // Start Shockwave
        ShockwaveManager.Instance.CallShockwave(PlayerController.Instance.transform.position);
        yield return new WaitForSeconds(1.25f);

        _sr.sortingOrder = 2;
        transform.GetChild(3).GetComponent<TrailRenderer>().emitting = true;

        // Set the initial delay and minimum clamp
        float initialDelay = .75f;
        float minDelay = 0.05f;
        
        // Teleport to each enemy with a mark.
        foreach (DeathMark mark in DeathMarkManager.Instance._createdList)
        {
            int randomDirection = (Random.value < 0.5f) ? 1 : -1;
            var position = mark.transform.position;
            
            // Teleport Sprite
            //_rb.MovePosition(new Vector2(position.x +randomDirection,position.y -.25f));
            transform.position = new Vector2(position.x + randomDirection, position.y - .25f);
            // Flip Sprite
            Flip(-randomDirection);
            _anim.SetTrigger("Slice");
            
            // Do anims and other shit
            // Destroy Marks?
            initialDelay = Mathf.Clamp(initialDelay * 0.9f, minDelay, initialDelay);
            
            yield return new WaitForSeconds(initialDelay);
        }

        // Flip To the right, teleport to center of scene
        Flip(1);
        _rb.MovePosition(new Vector2(0, -0.5f));
        
        yield return new WaitForSeconds(.05f);
        transform.GetChild(3).GetComponent<TrailRenderer>().emitting = false;

        initialDelay = .75f;
        
        foreach (DeathMark mark in DeathMarkManager.Instance._createdList)
        {
            mark.Death();
            initialDelay = Mathf.Clamp(initialDelay * 0.9f, minDelay, initialDelay);
            yield return new WaitForSeconds(initialDelay);
        }

        yield return new WaitForSeconds(1f);
        SnapshotManager.Instance.Callback();
    }

    private void Flip(float input)
    {
        // FLIPPING SPRITE
        if (input > 0)
        {
            // Player is moving right, flip the sprite to face right
            _sr.flipX = false;
        }
        else if (input < 0)
        {
            // Player is moving left, flip the sprite to face left
            _sr.flipX = true;
        }
    }
    
    // Animation Events
    private void DashEnd()
    { 
        _anim.SetBool("isDashing",false); 
    }
    private void UnsheathEnd()
    {
        _anim.SetTrigger("Aura");

        // If Controller Change Text
        if (Input.GetJoystickNames().Length > 0)
        {
            transform.GetChild(0).GetComponent<TMP_Text>().text = "Press X";
        }
        // Change pos based on whether the player is above or below the center of the scene
        transform.GetChild(0).localPosition = transform.position.y >= 0 ? new Vector2(0, -1) : new Vector2(0, 2.5f);
        // Text Scales In
        transform.GetChild(0).GetComponent<TextWobble>().ScaleText(new Vector2(0.75f, 0.75f), 1.5f);
        
        if (!_isCharged)
        {
            _isCharged = true;
        }
    }
    public void AuraEnd()
    {
        // Shockwave? ScreenShake? Sounds?
    }
    
    public void ReadyToExecute()
    {
        CanMove = false;
        transform.GetChild(2).gameObject.SetActive(false);
        StopAllCoroutines();
        PlayerHealth.Instance.SetInvincible();
        _anim.SetTrigger("Unsheath");
    }
}