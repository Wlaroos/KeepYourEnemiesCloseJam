using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseMenu : MonoBehaviour
{
    public static LoseMenu Instance { get; private set; }
    
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 1.5f;

    private TMP_Text _loseText;

    private bool _canPress = false;

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

        
        _fadeImage = GetComponent<Image>(); 
        _loseText = transform.GetChild(0).GetComponent<TMP_Text>();
        _loseText.color = Color.clear;
        _fadeImage.color = Color.clear;
    }

    void Start()
    {
        transform.GetChild(0).GetComponent<Renderer>().sortingOrder = 2;
        if (Input.GetJoystickNames().Length > 0)
        {
            _loseText.text = "--[ Executed ]--\n\nPress X To Restart\nPress B To Exit";
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Select") && _canPress)
        {
            SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetButtonDown("Exit") && _canPress)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void StartFade()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1f);
        
        float elapsedTime = 0f;
        Color startColor = _fadeImage.color;
        Color startColor2 = _loseText.color;
        Color endColor = Color.black;
        Color endColor2 = Color.red;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _loseText.color = Color.Lerp(startColor2, endColor2, elapsedTime / _fadeDuration);
            _fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / _fadeDuration);
            yield return null;
        }

        _fadeImage.color = endColor;
        _canPress = true;
    }
}
