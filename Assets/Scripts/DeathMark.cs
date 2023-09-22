using UnityEngine;

public class DeathMark : MonoBehaviour
{
    private DeathMarkManager _dmm;
    
    private bool _isCreated = false;
    private bool _isActivated = false;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (!other.isTrigger && player)
        {
            CreateMark();
        }
    }

    private void CreateMark()
    {
        _isCreated = true;
        _dmm.MarkCreated(this);
        Debug.Log("MARK CREATED");
    }

    public void ActivateMark()
    {
        Debug.Log("MARK ACTIVATED");
        _isActivated = true;
    }

    public void SetMarkManager(DeathMarkManager manager)
    {
        _dmm = manager;
    }
}
