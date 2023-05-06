using UnityEngine;
using UnityEngine.Events;

public class CombatHandler : MonoBehaviour
{
    public int killCounter;
    [HideInInspector] public UnityEvent OnKill;

    private static CombatHandler _instance;

    public static CombatHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}