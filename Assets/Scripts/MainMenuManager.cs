using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _controlsCanvas;
    [SerializeField] private GameObject _mainCanvas;

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void SkipIntro()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    
    public void Controls()
    {
        _controlsCanvas.SetActive(true);
        _mainCanvas.SetActive(false);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void Resume()
    {
        _controlsCanvas.SetActive(false);
        _mainCanvas.SetActive(true);
    }
    
}
