using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetAxis("CycleLevel") == 1 && SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex + 1).IsValid())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (Input.GetAxis("CycleLevel") == -1 && SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex - 1).IsValid())
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
