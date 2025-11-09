using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public int Currency = 0;
    public static EconomyManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Addition of Money
    public void AddMoney(int amount) 
    {
        Currency += amount;
    }

    
    // Removal of Money
    public void TryRemoveMoney(int amount)
    {
        if (CanPlayerAffordItem(amount))
        {
            Currency -= amount;
        }
        else return;
    }

    private bool CanPlayerAffordItem(int ItemCost)
    {
        if (Currency >= ItemCost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
