using System;
using UnityEngine;
using UnityEngine.Events;

public class DebtManager : MonoBehaviour
{
    public bool DebtPaid = false;
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
    //
    public void DayPassed()
    {
        daysUntilDue -= 1;

        if (daysUntilDue <= 0)// if last day
        {
            if (DebtPaid)
            {
                DebtSucceeded();
            }
            else
            {
                DebtFailed();
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
    }

    public void PayDebt(int amount)
    {
        if (amount > DebtRemaining)// Stops player from paying more than is left on their debt
        {
            int paymentAmount = DebtRemaining - amount;
            DebtRemaining =- paymentAmount;
        }
        else
        {
            DebtRemaining -= amount;
        }      
    }
}
