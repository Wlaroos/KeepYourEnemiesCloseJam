using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveManager : MonoBehaviour
{

    [SerializeField] private float _shockwaveTime = 0.75f;
    private Coroutine _shockwaveCoroutine;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    private Material _material;
    private CircleCollider2D _cc;

    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
        _cc = GetComponent<CircleCollider2D>();
    }
    
    public void CallShockwave(Vector2 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        StartCoroutine(ShockwaveAction(-0.1f, 1f));
    }

    private IEnumerator ShockwaveAction(float startPos, float endPos)
    {
        _material.SetFloat(_waveDistanceFromCenter, startPos);

        float lerpedAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < _shockwaveTime)
        {
            elapsedTime += Time.deltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / _shockwaveTime));
            
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);
            _cc.radius = (lerpedAmount / 2) + 0.05f;
            
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
