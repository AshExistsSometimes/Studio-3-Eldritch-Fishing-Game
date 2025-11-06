using TMPro;
using UnityEngine;

public class BoatHealthManager : MonoBehaviour, IDamagable
{
    public float HP = 100f;

    public float MaxHP = 100f;

    public TMP_Text boatHPText;

    public void TakeDamage(float amount)
    {
        HP -= amount;
        boatHPText.text = ("Boat HP : " + HP + " / " + MaxHP);
        if (HP <= 0f) Die();
    }

    public void Die()
    {
        boatHPText.text = ("Boat Sank :(");
        Debug.Log("Boat Sank");
    }
}
