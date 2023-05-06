using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MeleeComboState
{
    First = 1,
    Second,
    Third,
}

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private Collider _weaponCollider1;
    [SerializeField] private Collider _weaponCollider2;

    [SerializeField] private bool _comboTwoWeapons;
    [SerializeField] private int _comboLength = 1;
    public MeleeComboState ComboState = MeleeComboState.First;

    [SerializeField] private float _activeDuration = 1;
    [SerializeField] private float _startDelay = 0.25f;
    private float _timer;
    private float _delayTimer;
    private bool _isActive;
    private bool _wasActive;

    [SerializeField] private GameObject _prepAttackIcon, _attackingIcon;

    public bool IsActive { get => _isActive; }


    // Update is called once per frame
    void Update()
    {
        AttackState();
    }

    public void DoAttack()
    {
        _isActive = true;
    }

    private void ToggleComboState()
    {
        if (((int)ComboState) < _comboLength)
        {
            ComboState++;
        }
        else if (((int)ComboState) >= _comboLength)
        {
            ComboState = MeleeComboState.First;
        }
    }

    private void AttackState()
    {
        if (_isActive)
        {
            if (_delayTimer >= _startDelay)
            {
                if (_timer >= _activeDuration)
                {
                    _isActive = false;
                    _wasActive = true;
                }
                else
                {
                    // While Attacking Is Active
                    if (!_comboTwoWeapons)
                    {
                        _weaponCollider1.enabled = true;
                    }
                    else
                    {
                        if (((int)ComboState) == 1)
                        {
                            _weaponCollider1.enabled = true;
                        }
                        else
                        {
                            _weaponCollider2.enabled = true;
                        }
                    }
                    _timer += Time.deltaTime;
                    _prepAttackIcon.SetActive(false);
                    _attackingIcon.SetActive(true);
                }
            }
            else
            {
                _delayTimer += Time.deltaTime;
                _prepAttackIcon.SetActive(true);
            }
        }
        else
        {

            // Control Combo State
            if (_wasActive)
            {
                _wasActive = false;
                ToggleComboState();
            }

            // Reset Attack Values
            _timer = 0;
            _delayTimer = 0;
            _weaponCollider1.enabled = false;
            if (_weaponCollider2 != null) _weaponCollider2.enabled = false;
            _prepAttackIcon.SetActive(false);
            _attackingIcon.SetActive(false);
        }
    }
}
