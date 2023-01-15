using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(0, 2)] public float TimeScale = 1;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TimeScale != Time.timeScale) Time.timeScale = TimeScale;
    }
}
