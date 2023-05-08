using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombo : MonoBehaviour
{

    [System.Serializable]
    public class ComboTimer
    {
        [SerializeField] private float _duration = 1;
        [SerializeField] private float _activeStart = 0;
        [SerializeField] private float _activeEnd = 1;
        public float DamageMultiplier = 1;
        [SerializeField] private bool _canKnockback;

        public float Duration { get => _duration; }
        public float ActiveStart { get => _activeStart; }
        public float ActiveEnd { get => _activeEnd; }
        public bool CanKnockback { get => _canKnockback; }
    }

    [SerializeField] private Animator anim;
    [SerializeField] private float maxComboDelay = 2;
    [SerializeField] private List<ComboTimer> comboTimers = new List<ComboTimer>();
    private float lastClickedTime;
    private int numOfClicks;
    private bool _clicked;


    // Update is called once per frame
    void Update()
    {
        ComboTimeHandler();
        //ResetAnimations();
    }

    public void OnClick()
    {
        numOfClicks++;
        numOfClicks = numOfClicks > 3 ? 1 : numOfClicks;

        _clicked = true;
        lastClickedTime = 0;
        //if (Time.time - lastClickedTime > maxComboDelay)
        //{
        //    numOfClicks = 1;
        //}
        //lastClickedTime = Time.time;

        switch (numOfClicks)
        {
            case 1:
                anim.SetTrigger("LightAttack1");
                break;
            case 2:
                anim.SetTrigger("LightAttack2");
                break;
            case 3:
                anim.SetTrigger("LightAttack3");
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

    private void ComboTimeHandler()
    {
        if (_clicked)
        {
            lastClickedTime += Time.deltaTime;
            if (lastClickedTime > maxComboDelay)
            {
                numOfClicks = 0;

                lastClickedTime = 0;
                _clicked = false;
            }
        }
    }

    //private void ResetAnimations()
    //{
    //    if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(1).IsName("LightAttack1"))
    //    {
    //        anim.SetBool("LightAttack1", false);
    //    }
    //    if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(1).IsName("LightAttack2"))
    //    {
    //        anim.SetBool("LightAttack2", false);
    //    }
    //    if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(1).IsName("LightAttack3"))
    //    {
    //        anim.SetBool("LightAttack3", false);
    //    }
    //}

    public float GetDuration()
    {
        if (numOfClicks == 0)
        {
            return comboTimers[0].Duration;
        }
        return comboTimers[numOfClicks - 1].Duration;
    }

    public float GetActiveStart()
    {
        if (numOfClicks == 0)
        {
            return comboTimers[0].Duration;
        }
        return comboTimers[numOfClicks - 1].ActiveStart;
    }

    public float GetActiveEnd()
    {
        if (numOfClicks == 0)
        {
            return comboTimers[0].Duration;
        }
        return comboTimers[numOfClicks - 1].ActiveEnd;
    }    
    
    public float GetDamageMultiplier()
    {
        if (numOfClicks == 0)
        {
            return comboTimers[0].Duration;
        }
        return comboTimers[numOfClicks - 1].DamageMultiplier;
    }
    public bool GetCanKnockback()
    {
        if (numOfClicks == 0)
        {
            return comboTimers[0].CanKnockback;
        }
        return comboTimers[numOfClicks - 1].CanKnockback;
    }
}
