using System.Collections;
using UnityEngine;

public class ShockwaveManager : MonoBehaviour
{
    public static ShockwaveManager Instance { get; private set; }

    [SerializeField] private float _shockwaveTime = 0.75f;
    private Coroutine _shockwaveCoroutine;
    private static readonly int WaveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    private Material _material;
    private CircleCollider2D _cc;
    private SpriteRenderer _sr;

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
        
        _sr = GetComponent<SpriteRenderer>();
        _material = GetComponent<SpriteRenderer>().material;
        _cc = GetComponent<CircleCollider2D>();
        
        _sr.color = Color.clear;
    }
    
    public void CallShockwave(Vector2 pos)
    {
        transform.position = pos;
        _sr.color = Color.white;
        StartCoroutine(ShockwaveAction(-0.1f, 1f));
    }

    private IEnumerator ShockwaveAction(float startPos, float endPos)
    {
        _material.SetFloat(WaveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < _shockwaveTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / _shockwaveTime));
            
            _material.SetFloat(WaveDistanceFromCenter, lerpedAmount);
            _cc.radius = (lerpedAmount / 2) + 0.05f;
            
            yield return null;
        }

        _sr.color = Color.clear;
    }
}
