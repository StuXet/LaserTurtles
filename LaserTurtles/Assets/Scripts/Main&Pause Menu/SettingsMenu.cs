using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    PlayerController playerController;


    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        MoveType(true);
    }

    public void MoveType(bool state)
    {
        if (!state)
        {
            playerController.MoveType = MovementType.WorldPos;
        }
        else
        {
            playerController.MoveType = MovementType.WorldPosTrackLook;
        }
    }
}
