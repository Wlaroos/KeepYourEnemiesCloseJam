using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SnapshotManager : MonoBehaviour
{
    public static SnapshotManager Instance { get; private set; }

    private GameObject _quadTop;
    private GameObject _quadBottom;
    private GameObject _bg;
    
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

        _bg = transform.GetChild(0).gameObject;
        _quadBottom = transform.GetChild(1).gameObject;
        _quadTop = transform.GetChild(2).gameObject;
    }

    public void Callback()
    {
        _quadTop.SetActive(true);
        _quadBottom.SetActive(true);
        GetComponent<SnapshotController>().Snapshot(HandleNewSnapshotTexture);
    }
    
    private void HandleNewSnapshotTexture (Texture2D texture)
    {
        var material = _quadTop.GetComponent<Renderer>().material;
        var material2 = _quadBottom.GetComponent<Renderer>().material;

        // IMPORTANT! Textures are not automatically GC collected. 
        // So in order to not allocate more and more memory consider actively destroying
        // a texture as soon as you don't need it anymore
        //if(material.mainTexture) DestroyImmediate(material.mainTexture);     

        material.SetTexture("_MainTex", texture);
        material2.SetTexture("_MainTex", texture);
        //material2.mainTexture = texture;

        StartCoroutine(Slice());
    }

    private IEnumerator Slice()
    {
        _bg.SetActive(true);
        //ANIMATION OF SCREEN SLICE
        yield return new WaitForSeconds(1f);
        
        _quadTop.GetComponent<Rigidbody>().AddForce(3,0,0,ForceMode.Impulse);
        _quadBottom.GetComponent<Rigidbody>().AddForce(-3,0,0,ForceMode.Impulse);
        
        yield return new WaitForSeconds(1f);
        
        _quadTop.GetComponent<Rigidbody>().AddForce(0,Random.Range(5,8),0,ForceMode.Impulse);
        _quadBottom.GetComponent<Rigidbody>().AddForce(0,Random.Range(-5,-8),0,ForceMode.Impulse);
        _quadTop.GetComponent<Rigidbody>().AddTorque(0,0, Random.Range(-8,8), ForceMode.Impulse);
        _quadBottom.GetComponent<Rigidbody>().AddTorque(0,0, Random.Range(-8,8), ForceMode.Impulse);
    }
}
