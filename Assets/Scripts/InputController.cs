using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetButtonDown ("CycleLevelPos") && SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (Input.GetButtonDown ("CycleLevelNeg") && SceneManager.GetActiveScene().buildIndex - 1 >= 0)
        {
            SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex - 1);
        }
        
        if (Input.GetButtonDown("ExitGame"))
        {
            Application.Quit();
        }
        
        if (Input.GetButtonDown("RestartLevel"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
