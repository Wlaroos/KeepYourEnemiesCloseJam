using System.Collections;
using UnityEngine;

public class FadeAndScale : MonoBehaviour
{
    public float fadeDuration = 2f;
    public float scaleDuration = 2f;

    private Renderer renderer;
    private Transform transform;
    private Color startColor;
    private Vector3 startScale;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
        startColor = renderer.material.color;
        startScale = transform.localScale;
        
        StartFadeAndScale();
    }

    public void StartFadeAndScale()
    {
        StartCoroutine(FadeScale());
    }

    private IEnumerator FadeScale()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            renderer.material.color = newColor;
            
            float scaleValue = Mathf.Lerp(1f, .5f, elapsedTime / scaleDuration);
            transform.localScale = startScale * scaleValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        renderer.material.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        transform.localScale = Vector3.zero;
        
        Destroy(gameObject);
    }
}

