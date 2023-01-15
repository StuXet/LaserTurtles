using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image BGHealthBar;
    [SerializeField] private Image MaxHealthBar;
    [SerializeField] private Image CurrentHealthBar1;
    [SerializeField] private Image DamageHealthBar1;
    [SerializeField] private Image CurrentHealthBar2;
    [SerializeField] private Image DamageHealthBar2;
    private HealthSystem _healthSystem;

    [SerializeField] private float _shrinkTimerDelay = 1;
    [SerializeField] private float _shrinkSpeed = 1;
    private float _damagedHealthShrinkTimer;

    [SerializeField] private float _hideSpeed = 1;
    private float _hideTimer;
    [SerializeField] private bool UseTransparency;

    private void Awake()
    {
        SetTransparency(0);
    }

    private void Update()
    {
        ShrinkBar();
    }


    private void ShrinkBar()
    {
        _damagedHealthShrinkTimer -= Time.deltaTime;
        if (_damagedHealthShrinkTimer < 0)
        {
            if (CurrentHealthBar1.fillAmount < DamageHealthBar1.fillAmount)
            {
                DamageHealthBar1.fillAmount -= _shrinkSpeed * Time.deltaTime;
                if (DamageHealthBar2 != null) DamageHealthBar2.fillAmount -= _shrinkSpeed * Time.deltaTime;
            }
            else if (CurrentHealthBar1.fillAmount < DamageHealthBar1.fillAmount)
            {
                DamageHealthBar1.fillAmount = CurrentHealthBar1.fillAmount;
                if (DamageHealthBar2 != null) DamageHealthBar2.fillAmount = CurrentHealthBar2.fillAmount;
            }

            if (_hideTimer >= _hideSpeed)
            {
                SetTransparency(0);
                _hideTimer = 0;
            }
            else
            {
                _hideTimer += Time.deltaTime;
            }
        }
    }

    public void Setup(HealthSystem healthSystem)
    {
        this._healthSystem = healthSystem;
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        _damagedHealthShrinkTimer = _shrinkTimerDelay;
        CurrentHealthBar1.fillAmount = _healthSystem.GetHealthPercent();
        if (CurrentHealthBar2 != null) CurrentHealthBar2.fillAmount = _healthSystem.GetHealthPercent();
        if (CurrentHealthBar1.fillAmount > DamageHealthBar1.fillAmount)
        {
            DamageHealthBar1.fillAmount = _healthSystem.GetHealthPercent();
            if (DamageHealthBar2 != null) DamageHealthBar2.fillAmount = _healthSystem.GetHealthPercent();
        }

        _hideTimer = 0;
        SetTransparency(1);
    }

    private void SetTransparency(float a)
    {
        if (UseTransparency)
        {
            Color tempCol = BGHealthBar.color;
            tempCol.a = a;
            BGHealthBar.color = tempCol;

            tempCol = MaxHealthBar.color;
            tempCol.a = a;
            MaxHealthBar.color = tempCol;

            tempCol = CurrentHealthBar1.color;
            tempCol.a = a;
            CurrentHealthBar1.color = tempCol;

            tempCol = DamageHealthBar1.color;
            tempCol.a = a;
            DamageHealthBar1.color = tempCol;
        }
    }
}
