using UnityEngine;

public class DeathMark : MonoBehaviour
{
    [SerializeField] private GameObject _deathMarkPrefab;
    private DeathMarkManager _dmm;
    private SpriteRenderer _sr;
    private SpriteRenderer _dmsr;
    
    private bool _isCreated = false;
    private bool _isActivated = false;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject dmHolder = Instantiate(_deathMarkPrefab, new Vector2(transform.position.x ,transform.position.y + 1.25f), Quaternion.identity, transform);
        _dmsr = dmHolder.GetComponent<SpriteRenderer>();

        _dmsr.color = Color.clear;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (!other.isTrigger && player)
        {
            CreateMark();
        }
    }

    private void CreateMark()
    {
        if (!_isCreated && !_isActivated)
        {
            _isCreated = true;
            _dmm.MarkCreated(this);
            //Debug.Log("MARK CREATED");
            _dmsr.color = new Color32(25,25,25,255);
            //_sr.color = Color.magenta;
        }
    }

    public void ActivateMark()
    {
        if (!_isActivated && _isCreated)
        {
            //Debug.Log("MARK ACTIVATED");
            _isActivated = true;
            GetComponent<Shooter>()?.MarkActivated();
            _dmsr.color = new Color32(150,0,0,255);
            //_sr.color = Color.blue;
        }
    }

    public void SetMarkManager(DeathMarkManager manager)
    {
        _dmm = manager;
    }
}
