using UnityEngine;

public class DeathMark : MonoBehaviour
{
    private DeathMarkManager _dmm;
    private SpriteRenderer _sr;
    
    private bool _isCreated = false;
    private bool _isActivated = false;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
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
            Debug.Log("MARK CREATED");
            _sr.color = Color.magenta;
        }
    }

    public void ActivateMark()
    {
        if (!_isActivated && _isCreated)
        {
            Debug.Log("MARK ACTIVATED");
            _isActivated = true;
            GetComponent<Shooter>()?.MarkActivated();
            _sr.color = Color.blue;
        }
    }

    public void SetMarkManager(DeathMarkManager manager)
    {
        _dmm = manager;
    }
}
