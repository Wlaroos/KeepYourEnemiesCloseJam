using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelNameMenu : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDelay = 1f;
    [SerializeField] private float _fadeDuration = 0.75f;

    private TMP_Text _levelName;

    private void Awake()
    {
        _fadeImage = GetComponent<Image>(); 
        _levelName = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    void Start()
    {
        transform.GetChild(0).GetComponent<Renderer>().sortingOrder = 2;
        StartLevel();
        _levelName.text = "[ " + SceneManager.GetActiveScene().name + " ]";
    }

    void StartLevel()
    {
        Time.timeScale = 0f; // Pause the game by setting time scale to zero
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(_fadeDelay);
        
        float elapsedTime = 0f;
        Color startColor = _fadeImage.color;
        Color startColor2 = _levelName.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Fully transparent

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _levelName.color = Color.Lerp(startColor2, endColor, elapsedTime * 2.5f / _fadeDuration);
            _fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / _fadeDuration);
            yield return null;
        }

        _fadeImage.color = endColor;
        Time.timeScale = 1f; // Set time scale back to 1 to start the level
        
        MusicManager.Instance.SwapTrack(false);
        
        Destroy(gameObject);
    }
}
