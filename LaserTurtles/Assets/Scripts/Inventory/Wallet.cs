using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    private int _coins;

    public int Coins { get => _coins; }

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
