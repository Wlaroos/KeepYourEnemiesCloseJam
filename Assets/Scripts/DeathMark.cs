using System.Collections;
using UnityEngine;

public class DeathMark : MonoBehaviour
{
    [SerializeField] private GameObject _deathMarkPrefab;
    [SerializeField] private GameObject _killParticles;
    [SerializeField] private GameObject _deathMarkParticles;
    private DeathMarkManager _dmm;
    private SpriteRenderer _sr;
    private SpriteRenderer _dmsr;

    private bool _isCreated = false;
    private bool _isActivated = false;

    private float _shrinkDuration = 0.5f;
    private float _rotationSpeed = 90f;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject dmHolder = Instantiate(_deathMarkPrefab, new Vector2(transform.position.x, transform.position.y + 2f), Quaternion.identity, transform);
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
            _dmsr.color = new Color32(25, 25, 25, 255);
            Instantiate(_deathMarkParticles, new Vector2(transform.position.x, transform.position.y + 1f), Quaternion.identity);
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
            _dmsr.color = new Color32(150, 0, 0, 255);
            //_sr.color = Color.blue;

            StartCoroutine(MarkShrink());
        }
    }

    private IEnumerator MarkShrink()
    {
        Destroy(_dmsr.GetComponent<RotateHoverUtil>());
        
        float initialScale = _dmsr.transform.localScale.x;
        float targetScale = 0f;
        float elapsedTime = 0f;

        Quaternion initialRotation = _dmsr.transform.rotation;

        while (elapsedTime < _shrinkDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / _shrinkDuration;
            _dmsr.transform.localScale = new Vector3(
                Mathf.Lerp(initialScale, targetScale, t),
                Mathf.Lerp(initialScale, targetScale, t),
                1f
            );

            // Rotate the object gradually
            _dmsr.transform.rotation = Quaternion.Euler(0f, 0f, t * _rotationSpeed) * initialRotation;

            yield return null;
        }
        
    }
    
    public void Death()
    {
        Instantiate(_killParticles, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.Euler(0, 0, Random.Range(0, 360)), transform);
    }

    public void SetMarkManager(DeathMarkManager manager)
    {
        _dmm = manager;
    }
}
