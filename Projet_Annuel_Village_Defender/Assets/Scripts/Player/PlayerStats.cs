using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] int initialMoney = 100;
    private int currentMoney;

    private static PlayerStats instance;

    private void Awake()
    {
        instance = this;
        currentMoney = initialMoney;
    }

    public static bool TrySpendMoney(int amount)
    {
        if (instance.currentMoney >= amount)
        {
            instance.currentMoney -= amount;
            return true;
        }
        return false;
    }

    public static int GetMoney()
    {
        return instance.currentMoney;
    }

    public static void AddMoney(int amount)
    {
        instance.currentMoney += amount;
    }
}