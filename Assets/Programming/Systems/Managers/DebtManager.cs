using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DebtManager : MonoBehaviour
{
    [Tooltip("if true, player has paid off all their debt")]public bool DebtFullyPaid = false;
    [Space]
    public int TotalDebtAmount = 100;
    public int DebtRemaining = 100;
    [Tooltip("Percentage based")] public float InterestAmount = 10f;
    [Space]
    public int DebtTimeLimit = 7;
    public int daysUntilDue = 7;
    [Space]
    public int DebtsUntilPaidOff = 10;
    [Space]
    public UnityEvent FailureEvent;
    public TMP_Text TrackerText;

    [HideInInspector] public DebtManager Instance;
    //

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        TrackerText.text = ("" + daysUntilDue);
        Debug.Log(DebtRemaining);
    }
    public void DayPassed()
    {
        if (DebtFullyPaid) return;

        if (!DebtFullyPaid)
        {
            daysUntilDue -= 1;
            TrackerText.text = ("" + daysUntilDue);

            if (daysUntilDue <= 0)// if last day
            {
                if (DebtFullyPaid)
                {
                    DebtSucceeded();
                }
                else
                {
                    DebtFailed();
                }
            }
        }
    }

    public void DebtFailed()
    {
        FailureEvent.Invoke();
    }

    public void DebtSucceeded()
    {
        daysUntilDue = DebtTimeLimit;

        // Calculate new debt amount and apply it
        float NextDebt = TotalDebtAmount + (TotalDebtAmount * (InterestAmount / 100f));
        int NewDebtAmount = Convert.ToInt32(NextDebt);
        TotalDebtAmount = NewDebtAmount;

        // Reset how much has to be paid
        DebtRemaining = TotalDebtAmount;

        // Reduce the number of debts remaining to be paid
        DebtsUntilPaidOff -= 1;
        if (DebtsUntilPaidOff <= 0)
        {
            DebtFullyPaid = true;
            TrackerText.text = ("X");
        }
    }

    public void TryPayDebt(int amount)
    {
        if (EconomyManager.instance.CanPlayerAffordItem(amount))
        {
            if (amount > DebtRemaining)// Stops player from paying more than is left on their debt
                {
                    int paymentAmount = DebtRemaining - amount;
                    DebtRemaining = -paymentAmount;
                    EconomyManager.instance.TryRemoveMoney(amount);     
                }
            else
                {
                    DebtRemaining -= amount;
                    EconomyManager.instance.TryRemoveMoney(amount);
                }
        }
        else { return; }
    }
}
