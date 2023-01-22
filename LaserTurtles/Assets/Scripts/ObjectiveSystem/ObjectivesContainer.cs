using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesContainer : MonoBehaviour
{
    public static ObjectivesContainer Instance;
    
    
    [SerializeField] private List<ObjectiveBase> _objectivesList = new List<ObjectiveBase>();
    public List<ObjectiveBase> ObjectivesList { get => _objectivesList;}


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ObjectiveBase[] tempList = GetComponentsInChildren<ObjectiveBase>();

        foreach (ObjectiveBase obj in tempList)
        {
            ObjectivesList.Add(obj);
        }
    }
}
