using TMPro;
using UnityEngine;

public class BoatHealthManager : MonoBehaviour, IDamagable
{
    public float HP = 100f;

    public float MaxHP = 100f;

    public void TakeDamage(float amount)
    {
        HP -= amount;
        if (HP <= 0f) Die();
    }

    public void Die()
    {
        Debug.Log("Boat Sank");
    }

    public void Repair()
    {
        HP = MaxHP;
    }
}
