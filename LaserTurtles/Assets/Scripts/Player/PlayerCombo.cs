using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombo : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float maxComboDelay = 2;
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

    private void ComboDelay()
    {

    }
}
