using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Merchant : MonoBehaviour
{
    public delegate void SoldItem(InventoryItemData itemData);
    public event SoldItem OnSoldItem;

    Wallet pWallet;
    GameObject player;
    InputManager inputManager;
    PlayerInputActions playerInputActions;
    [SerializeField] private UIMediator _uIMediator;
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


    string stage1 = "I've been waiting for you kid! i got a special weapon you can use to defeat the witch!";
    string stage2t = "Great! looks like you got enough crystals, here you go...";
    string stage2f = "Hmmm, I might be your father but i still need something in return, come back when you got enough crystals";
    string stage3 = "Good luck kid";


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _uIMediator = UIMediator.Instance;
        pWallet = player.GetComponentInChildren<Wallet>();
        inputManager = player.GetComponent<InputManager>();
        playerInputActions = inputManager.PlInputActions;
        playerInputActions.Player.Interact.performed += DialogueStartCheck;

        dialoguePanel = _uIMediator.DialougeUI.transform.GetChild(0).gameObject;
        dialogueText = _uIMediator.DialougeUI.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        dialogueButton = _uIMediator.DialougeUI.transform.GetChild(2).GetComponentInChildren<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isComplete)
        {

            if (Vector3.Distance(transform.position, player.transform.position) > interactionRange && inDialogue) //player gets out of range while in dialogue 
            {
                EndDialogue();
            }

            if (Vector3.Distance(transform.position, player.transform.position) <= interactionRange && !inDialogue)
            {
                interactTip.SetActive(true); //shows tip if player is in right range 
            }
            else
            {
                interactTip.SetActive(false);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueButton.onClick.AddListener(delegate { NextStage(); });
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
        if (Vector3.Distance(transform.position, player.transform.position) <= interactionRange && !inDialogue && !isComplete)
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

        
        if (!isFirstTime)
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
            ItemObject tempItem = weapon.GetComponent<ItemObject>();
            tempItem.CanBePicked = true;
            pWallet.DeductCoins(swordPrice);
            if (OnSoldItem != null) { OnSoldItem.Invoke(tempItem.ReferenceItem); };
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
