using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private ParticleSystem stepParticle;

    public void PlayStepParticle()
    {
        if (stepParticle != null)
        {
            stepParticle.Play();
        }
    }
}
