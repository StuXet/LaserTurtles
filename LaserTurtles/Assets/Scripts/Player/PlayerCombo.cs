using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombo : MonoBehaviour
{

    [System.Serializable]
    public class ComboTimer
    {
        [SerializeField] private float _activeStart;
        [SerializeField] private float _activeEnd;
        public float DamageMultiplier;

        public float ActiveStart { get => _activeStart; }
        public float ActiveEnd { get => _activeEnd; }
    }
    [SerializeField] private Animator anim;
    [SerializeField] private float maxComboDelay = 2;
    [SerializeField] private List<ComboTimer> comboTimers = new List<ComboTimer>();
    private float lastClickedTime;
    private int numOfClicks;


    // Update is called once per frame
    void Update()
    {
        ResetAnimations();
    }

    public void OnClick()
    {
        numOfClicks++;
        numOfClicks = numOfClicks > 3 ? 1 : numOfClicks;

        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numOfClicks = 1;
        }
        lastClickedTime = Time.time;

        switch (numOfClicks)
        {
            case 1:
                anim.SetBool("LightAttack1", true);
                break;
            case 2:
                anim.SetBool("LightAttack2", true);
                break;
            case 3:
                anim.SetBool("LightAttack3", true);
                break;
            default:
                print("numOfClicks ERROR");
                break;
        }

        //if (numOfClicks == 2 && anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(1).IsName("LightAttack1"))
        //{
        //    anim.SetBool("LightAttack1", false);
        //    anim.SetBool("LightAttack2", true);
        //}
        //if (numOfClicks == 3 && anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(1).IsName("LightAttack2"))
        //{
        //    anim.SetBool("LightAttack2", false);
        //    anim.SetBool("LightAttack3", true);
        //}

    }

    private void ResetAnimations()
    {
        if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(1).IsName("LightAttack1"))
        {
            anim.SetBool("LightAttack1", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(1).IsName("LightAttack2"))
        {
            anim.SetBool("LightAttack2", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(1).IsName("LightAttack3"))
        {
            anim.SetBool("LightAttack3", false);
        }
    }

    public float GetActiveStart()
    {
        switch (numOfClicks)
        {
            case 1:
                return comboTimers[0].ActiveStart;
            case 2:
                return comboTimers[1].ActiveStart;
            case 3:
                return comboTimers[2].ActiveStart;
            default:
                return 0;
        }
    }

    public float GetActiveEnd()
    {
        switch (numOfClicks)
        {
            case 1:
                return comboTimers[0].ActiveEnd;
            case 2:
                return comboTimers[1].ActiveEnd;
            case 3:
                return comboTimers[2].ActiveEnd;
            default:
                return 2;
        }
    }

    public float GetDamageMultiplier()
    {
        switch (numOfClicks)
        {
            case 1:
                return comboTimers[0].DamageMultiplier;
            case 2:
                return comboTimers[1].DamageMultiplier;
            case 3:
                return comboTimers[2].DamageMultiplier;
            default:
                return 1;
        }
    }
}
