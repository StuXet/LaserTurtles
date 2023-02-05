using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Merchant : MonoBehaviour
{
    Wallet pWallet;
    GameObject player;
    InputManager inputManager;
    PlayerInputActions playerInputActions;
    [SerializeField] float interactionRange;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Button dialogueButton;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject interactTip;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] int swordPrice = 10;

    [Header("Reward Setting")]
    [SerializeField] GameObject reward;
    [SerializeField] Transform rewardSpawnPos;

    [HideInInspector] public bool inDialogue;
    bool isComplete;
    bool isFirstTime = true;
    

    string stage1 = "Hello there! i got a super rad sword you can use to defeat the witch!";
    string stage2t = "Great! looks like you got enought shmekels for the sword, here you go...";
    string stage2f = "Damn, looks like youre a brokie my guy, come back when you got enough money";
    string stage3 = "Good luck homie";

    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject dialogueUI = player.transform.Find("DialogueUI").gameObject;
        dialogueText = dialogueUI.GetComponentInChildren<TextMeshProUGUI>();
        dialogueButton = dialogueUI.GetComponentInChildren<Button>();
        dialoguePanel = dialogueUI.transform.Find("DialoguePanel").gameObject;

        pWallet = player.GetComponentInChildren<Wallet>();
        inputManager = player.GetComponent<InputManager>();
        playerInputActions = inputManager.PlInputActions;
        playerInputActions.Player.Interact.performed += DialogueStartCheck;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > interactionRange && inDialogue) //player gets out of range while in dialogue 
        {
            EndDialogue();
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= interactionRange && !inDialogue)
        {
            interactTip.SetActive(true); //shows tip if player is in right range 
            dialogueButton.onClick.AddListener(delegate { NextStage(); });
        }
        else
        {
            interactTip.SetActive(false);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueButton.onClick.RemoveAllListeners();
        }
    }

        void DialogueStartCheck(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= interactionRange && !inDialogue)
        {
            StartDialogue();
        }
        else if (inDialogue)
        {
            NextStage();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        dialogueButton.gameObject.SetActive(false);
        inDialogue = false;
        player.GetComponent<PlayerCombatSystem>().inDialogue = false;
        isFirstTime = false;
    }

    void StartDialogue()
    {
        inDialogue = true;
        player.GetComponent<PlayerCombatSystem>().inDialogue = true;
        dialoguePanel.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        dialogueButton.gameObject.SetActive(true);
        interactTip.SetActive(false);
        exclamationMark.SetActive(false);

        dialogueText.text = stage1;

        if (isComplete)
        {
            dialogueText.text = stage3;
        }
        else if (!isFirstTime)
        {
            NextStage();
        }
    }

    public void NextStage()
    {
        print("checkcheck");
        if (dialogueText.text == stage1 && pWallet.Coins >= swordPrice)
        {
            dialogueText.text = stage2t;
            GameObject weapon = Instantiate(reward, rewardSpawnPos.position, rewardSpawnPos.rotation);
            weapon.GetComponent<ItemObject>().CanBePicked = true;
            pWallet.DeductCoins(swordPrice);
        }
        else if (dialogueText.text == stage1)
        {
            dialogueText.text = stage2f;
        }
        else if (dialogueText.text == stage2f)
        {
            EndDialogue();
        }
        else if (dialogueText.text == stage2t)
        {
            dialogueText.text = stage3;
        }
        else if (dialogueText.text == stage3)
        {
            isComplete = true;
            EndDialogue();
        }
    }
}
