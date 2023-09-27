using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _controlsCanvas;
    [SerializeField] private GameObject _mainCanvas;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _resumeButton;
    [SerializeField] private GameObject _controlsButton;

    private void Start()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            _eventSystem.SetSelectedGameObject(_startButton);
        }
    }

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
        if (Input.GetJoystickNames().Length > 0)
        {
            _eventSystem.SetSelectedGameObject(_resumeButton);
        }
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
    
    public void Resume()
    {
        _controlsCanvas.SetActive(false);
        _mainCanvas.SetActive(true);
        if (Input.GetJoystickNames().Length > 0)
        {
            _eventSystem.SetSelectedGameObject(_controlsButton);
        }
    }
    
}
