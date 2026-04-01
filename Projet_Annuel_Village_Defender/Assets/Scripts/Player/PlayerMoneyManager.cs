using System;
using UnityEngine;

namespace Player
{
public class PlayerMoneyManager : MonoBehaviour
{
    public static PlayerMoneyManager Instance { get; private set; }
    
    [SerializeField] private int startingMoney = 250;

    public int CurrentMoney => _currentMoney;
    public event Action<int> MoneyChanged;

    private int _currentMoney;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_currentMoney == 0)
        {
            _currentMoney = Mathf.Max(0, startingMoney);
            MoneyChanged?.Invoke(_currentMoney);
        }
    }


    private bool CanAfford(int amount)
    {
        return amount <= 0 || _currentMoney >= amount;
    }

    public bool TrySpend(int amount)
    {
        amount = Mathf.Max(0, amount);
        if (!CanAfford(amount)) return false;

        _currentMoney -= amount;
        MoneyChanged?.Invoke(_currentMoney);
        return true;
    }

    public void AddMoney(int amount)
    {
        amount = Mathf.Max(0, amount);
        _currentMoney += amount;
        MoneyChanged?.Invoke(_currentMoney);
    }

    public void SetMoney(int amount)
    {
        _currentMoney = Mathf.Max(0, amount);
        MoneyChanged?.Invoke(_currentMoney);
    }
}
}


