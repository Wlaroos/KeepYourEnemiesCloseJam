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
    private TrailRenderer _tr;
    
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
        _tr = transform.GetChild(3).GetComponent<TrailRenderer>();
    }

    public void Callback()
    {
        _quadTop.SetActive(true);
        _quadBottom.SetActive(true);
        _quadTop.transform.GetChild(0).gameObject.SetActive(false);
        _quadBottom.transform.GetChild(0).gameObject.SetActive(false);
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
        
        int direction = Random.Range(0, 2) * 2 - 1;  // Randomly pick 1 or -1
        
        //ANIMATION OF SCREEN SLICE
        float increment = 50 * Time.fixedDeltaTime;

        if (direction == -1)
        {
            _tr.transform.localPosition = new Vector2(50, 0);
            while (_tr.transform.localPosition.x > -50 )
            {
                Vector3 newPosition = _tr.transform.localPosition;
                newPosition.x -= increment;
                _tr.transform.localPosition = newPosition;

                // Wait for the next frame
                yield return null;
            }
        }
        else
        {
            _tr.transform.localPosition = new Vector2(-50, 0);
            while (_tr.transform.localPosition.x < 50 )
            {
                Vector3 newPosition = _tr.transform.localPosition;
                newPosition.x += increment;
                _tr.transform.localPosition = newPosition;

                // Wait for the next frame
                yield return null;
            }

        }
        
        _quadTop.GetComponent<Rigidbody>().AddForce(0, 0.01f, 0, ForceMode.Impulse);
        _quadBottom.GetComponent<Rigidbody>().AddForce(0, -0.01f, 0, ForceMode.Impulse);
        yield return new WaitForSeconds(.1f);
        _quadTop.transform.GetChild(0).gameObject.SetActive(true);
        _quadBottom.transform.GetChild(0).gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1f);
    
        _quadTop.GetComponent<Rigidbody>().AddForce(direction * 3, 0, 0, ForceMode.Impulse);
        _quadBottom.GetComponent<Rigidbody>().AddForce(direction * -3, 0, 0, ForceMode.Impulse);
    
        yield return new WaitForSeconds(.25f);
    
        _quadTop.GetComponent<Rigidbody>().AddForce(direction * -2, 0, 0, ForceMode.Impulse);
        _quadBottom.GetComponent<Rigidbody>().AddForce(direction * 2, 0, 0, ForceMode.Impulse);
    
        yield return new WaitForSeconds(1f);
    
        _quadTop.GetComponent<Rigidbody>().AddForce(0, Random.Range(5, 8), 0, ForceMode.Impulse);
        _quadBottom.GetComponent<Rigidbody>().AddForce(0, Random.Range(-8, -5), 0, ForceMode.Impulse);
        _quadTop.GetComponent<Rigidbody>().AddTorque(0, 0, Random.Range(-8, -6) * direction, ForceMode.Impulse);
        _quadBottom.GetComponent<Rigidbody>().AddTorque(0, 0, Random.Range(6, 8) * direction, ForceMode.Impulse);
    }
}
