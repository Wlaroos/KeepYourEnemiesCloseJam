using System.Collections;
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
    [Header("Other")] 
    [SerializeField] private Material _flashMat;
    [SerializeField] private GameObject _teleTrail;

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

    private bool _isCharged;

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
    }

    private void Start()
    {
        // Dash trail gets fucky if I don't do this
        Instantiate(_teleTrail, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity, transform);
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
        
        if (Input.GetKeyDown(KeyCode.E) && _isCharged && Time.timeScale < 1)
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
        Material oldMat =  _sr.material;
        
        yield return new WaitForSeconds(_dashCooldown);
        
        _sr.material = _flashMat;
        
        yield return new WaitForSeconds(0.15f);
        
        _sr.material = oldMat;

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

        _sr.sortingOrder = 5;
        transform.GetChild(1).GetComponent<TrailRenderer>().emitting = true;

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
            yield return new WaitForSeconds(.5f);
        }

        // Flip To the right, teleport to center of scene
        Flip(1);
        _rb.MovePosition(Vector2.zero);
        
        yield return new WaitForSeconds(.05f);
        transform.GetChild(1).GetComponent<TrailRenderer>().emitting = false;
        
        yield return new WaitForSeconds(.5f);
        _sr.color = Color.clear;
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
    }
    public void AuraEnd()
    {
        if (!_isCharged)
        {
            _isCharged = true;
            // Change pos based on whether the player is above or below the center of the scene
            transform.GetChild(0).localPosition = transform.position.y >= 0 ? new Vector2(0, -1) : new Vector2(0, 2.5f);
            // Text Scales In
            transform.GetChild(0).GetComponent<TextWobble>().ScaleText(new Vector2(0.75f, 0.75f), 0.125f);
        }
    }
    
    public void ReadyToExecute()
    {
        CanMove = false;
        _anim.SetTrigger("Unsheath");
    }
}