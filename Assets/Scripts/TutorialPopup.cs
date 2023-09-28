using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text _dashText;

    private void Awake()
    {
        if (Input.GetJoystickNames().Length > 0 && _dashText != null)
        {
            _dashText.text = "Press A To Dash";
        }
    }
}
