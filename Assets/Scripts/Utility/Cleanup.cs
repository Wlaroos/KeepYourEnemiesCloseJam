using UnityEngine;

public class Cleanup : MonoBehaviour
{

    [SerializeField] private float _timeToDestroy = 2f;
    
    private void Awake()
    {
        Invoke(nameof(Delt),_timeToDestroy);
    }

    private void Delt()
    {
        Destroy(gameObject);
    }
}
