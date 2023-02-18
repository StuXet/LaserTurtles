using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinsText, _coinsChangePopup;

    [SerializeField] private int _coins;

    private int _coinsChangeSum;
    private float _popupTime = 1f;
    private float _popupTimer;

    public int Coins { get => _coins; }

    private void Awake()
    {
        _coinsChangePopup.gameObject.SetActive(false);
        _popupTimer = _popupTime;
    }

    private void Update()
    {
        _coinsText.text = _coins.ToString();
        CoinsPopup();
    }


    public void AddCoins(int amount)
    {
        _coins += amount;

        _popupTimer = 0;
        _coinsChangePopup.color = Color.green;
        _coinsChangeSum+= amount;
        _coinsChangePopup.text = "+" + _coinsChangeSum;
    }

    public void DeductCoins(int amount)
    {
        if (_coins > 0)
        {
            if (Coins >= amount)
            {
                _coins -= amount;

                _popupTimer = 0;
                _coinsChangePopup.color = Color.red;
                _coinsChangePopup.text = "-" + amount;
            }
        }
    }

    private void CoinsPopup()
    {
        if (_popupTimer <= _popupTime)
        {
            _popupTimer += Time.deltaTime;
            _coinsChangePopup.gameObject.SetActive(true);
        }
        else
        {
            _coinsChangeSum = 0;
            _coinsChangePopup.text = 0.ToString();
            _coinsChangePopup.color = Color.white;
            _coinsChangePopup.gameObject.SetActive(false);
        }
    }
}
