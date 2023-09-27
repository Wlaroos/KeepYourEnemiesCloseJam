using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _questionMark;
    [SerializeField] private GameObject _exclamationPoint;
    [SerializeField] private GameObject _angry;
    [SerializeField] private GameObject _skull;

    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _enemy;
    [SerializeField] private GameObject _sword;

    [SerializeField] private Rigidbody2D _cloud1;
    [SerializeField] private Rigidbody2D _cloud2;
    
    [SerializeField] private TMP_Text _skipText;

    private bool _hasSkipped = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // Fix level cycling error where time scale wasn't set back after swapping off a different level too fast.
        Time.timeScale = 1;
        
        if (Input.GetJoystickNames().Length > 0)
        {
            _skipText.text = "Press X To Skip";
        }
        
        _player.transform.GetChild(0).GetComponent<ParticleSystem>().Pause();
        _player.transform.GetChild(0).GetComponent<ParticleSystem>().Clear();
        StartCoroutine(Cutscene());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Select")&& _hasSkipped == false)
        {
            StopAllCoroutines();
            StartCoroutine(Skipped());
            _hasSkipped = true;
        }
    }

    private IEnumerator Cutscene()
    {
        _cloud1.velocity = new Vector2(.25f, 0);
        _cloud2.velocity = new Vector2(-0.125f, 0);
        
        yield return new WaitForSeconds(2f);

        var enemies = GameObject.FindObjectsOfType<Rigidbody2D>();
        foreach (var rb in enemies)
        {
            if (!rb.gameObject.CompareTag("Player"))
            {
                rb.velocity = new Vector2(-7.5f, 0);
                rb.GetComponent<Animator>().Play(0, 0, Random.value);
            }
        }
        
        yield return new WaitForSeconds(5f);
        
        _player.GetComponent<Animator>().SetTrigger("Sit");
        
        yield return new WaitForSeconds(3.5f);
        Instantiate(_questionMark, new Vector2(_player.transform.position.x + 1f, _player.transform.position.y + 1f), Quaternion.identity);
        Destroy(_sword);
        
        yield return new WaitForSeconds(2.5f);
        _enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(7.5f, 0);
        
        yield return new WaitForSeconds(1f);
        Instantiate(_exclamationPoint, new Vector2(_player.transform.position.x + 1f, _player.transform.position.y + 1f), Quaternion.identity);
        
        yield return new WaitForSeconds(0.5f);
        _enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0);
        _enemy.transform.GetChild(0).GetComponent<ParticleSystem>().Pause();
        _enemy.transform.GetChild(0).GetComponent<ParticleSystem>().Clear();
        _enemy.GetComponent<Animator>().SetTrigger("Stop");
        
        yield return new WaitForSeconds(1f);
        Instantiate(_angry, new Vector2(_enemy.transform.position.x + 1f, _enemy.transform.position.y + 1f), Quaternion.identity);
        yield return new WaitForSeconds(.5f);
        Instantiate(_skull, new Vector2(_enemy.transform.position.x - 1f, _enemy.transform.position.y + 1f), Quaternion.identity);
        
        yield return new WaitForSeconds(1f);
        _player.GetComponent<Rigidbody2D>().velocity = new Vector2(7.5f, 0);
        _player.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        _player.GetComponent<Animator>().SetTrigger("Run");
        
        yield return new WaitForSeconds(1f);
        _enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(7.5f, 0);
        _enemy.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        _enemy.GetComponent<Animator>().SetTrigger("Run");
        
        yield return new WaitForSeconds(2f);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;

        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / 2f);
            Color newColor = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            sr.color = newColor;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator Skipped()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;
        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / 2f);
            Color newColor = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            sr.color = newColor;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
    }
}
