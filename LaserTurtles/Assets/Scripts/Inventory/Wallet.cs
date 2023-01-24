using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinsText;

    [SerializeField] private int _coins;

    public int Coins { get => _coins; }


    private void Update()
    {
        _coinsText.text = _coins.ToString();
    }


    public void AddCoins(int amount)
    {
        _coins += amount;
    }

    public void DeductCoins(int amount)
    {
        if (_coins > 0)
        {
            if (Coins >= amount)
            {
                _coins -= amount;
            }
        }
    }
}
