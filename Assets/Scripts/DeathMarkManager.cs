using System.Collections.Generic;
using UnityEngine;

public class DeathMarkManager : MonoBehaviour
{
    public static DeathMarkManager Instance { get; private set; }
    
    private List<DeathMark> _markList = new List<DeathMark>();
    private List<DeathMark> _createdList = new List<DeathMark>();

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
        
        // Find all DeathMark objects in the scene and add them to _markList
        DeathMark[] deathMarks = FindObjectsOfType<DeathMark>();

        foreach (DeathMark dm in deathMarks)
        {
            _markList.Add(dm);
            dm.SetMarkManager(this);
        }
    }

    public void MarkCreated(DeathMark mark)
    {
        // Making sure this mark is in the _markList and not the _createdList
        if (_markList.Contains(mark) && !_createdList.Contains(mark))
        {
            _createdList.Add(mark);

            // Check if the sorted contents of _markList and _createdList are the same
            if (ListsAreEqual(_markList, _createdList))
            {
                Invoke(nameof(ActivateMarks), 0.5f);
            }
        }
        else
        {
            Debug.Log("THIS OBJECT WAS NOT FOUND IN THE DEATH MARK LIST");
        }
    }

    // Helper method to compare if two lists are equal
    private bool ListsAreEqual(List<DeathMark> list1, List<DeathMark> list2)
    {
        // Check if the counts are equal first
        if (list1.Count != list2.Count)
            return false;

        // Create sets to compare the items
        HashSet<DeathMark> set1 = new HashSet<DeathMark>(list1);
        HashSet<DeathMark> set2 = new HashSet<DeathMark>(list2);

        // Check if the sets are equal
        return set1.SetEquals(set2);
    }
    
    private void ActivateMarks()
    {                Debug.Log("ACTIVATED");
        foreach (DeathMark dm in _createdList)
        {
            dm.ActivateMark();
            // Let the player activate this later
            ShockwaveManager.Instance.CallShockwave(PlayerController.Instance.transform.position);
            PlayerController.Instance.ReadyToExecute();
            CameraController.Instance.StartZoomIn(4f, 1f);
            Time.timeScale = 0.25f;
        }
    }

}