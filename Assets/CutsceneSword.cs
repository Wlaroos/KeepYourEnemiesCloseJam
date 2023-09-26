using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneSword : MonoBehaviour
{
    private Animator _anim;
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _anim.StopPlayback();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _anim.SetTrigger("Sword");
    }
}
