using System.Collections;
using UnityEngine;

public class FadeAndScale : MonoBehaviour
{
    public float fadeScaleDelay = 0f;
    public float fadeScaleDuration = 2f;

    private Renderer r;
    private Transform trans;
    private Color startColor;
    private Vector3 startScale;

    private void Start()
    {
        r = GetComponent<Renderer>();
        trans = GetComponent<Transform>();
        startColor = r.material.color;
        startScale = trans.localScale;
        
        StartFadeAndScale();
    }

    public void StartFadeAndScale()
    {
        StartCoroutine(FadeScale());
    }

    private IEnumerator FadeScale()
    {
        yield return new WaitForSeconds(fadeScaleDelay);

        float elapsedTime = 0f;

        while (elapsedTime < fadeScaleDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeScaleDuration);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            r.material.color = newColor;
            
            float scaleValue = Mathf.Lerp(1f, .5f, elapsedTime / fadeScaleDuration);
            trans.localScale = startScale * scaleValue;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        r.material.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        trans.localScale = Vector3.zero;
        
        Destroy(gameObject);
    }
}

