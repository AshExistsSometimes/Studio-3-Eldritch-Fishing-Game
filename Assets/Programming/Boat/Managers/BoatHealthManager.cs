using TMPro;
using UnityEngine;

public class BoatHealthManager : MonoBehaviour, IDamagable
{
    public float HP = 100f;

    public float MaxHP = 100f;

    private AnalyticsManager analytics;

    [Header("Death Settings")]
    public float DeathReloadDelay = 2.5f;
    public GameObject DeathScreen;


    private void Awake()
    {
        analytics = AnalyticsManager.Instance;
    }

    public void TakeDamage(float amount)
    {
        HP -= amount;
        if (HP <= 0f) Die();
    }

    public void Die()
    {
        Debug.Log("Boat Sank");
        analytics.AddString("Players Boat Sank, Had Max HP of " + MaxHP + " at time of death");

        // Show death screen
        if (DeathScreen != null)
            DeathScreen.SetActive(true);

        // reload scene after a delay
        StartCoroutine(DelayedReload());
    }

    public void Repair()
    {
        HP = MaxHP;
    }

    // UPGRADES
    public void UpgradeMaxHP(float UpgradeAmount)
    {
        MaxHP += UpgradeAmount;
        analytics.AddString("Player upgraded Boat HP to" + MaxHP);
    }


    private System.Collections.IEnumerator DelayedReload()
    {
        yield return new WaitForSecondsRealtime(DeathReloadDelay);

        if (DeathManager.Instance != null)
        {
            DeathManager.Instance.ReloadSceneAndLoadSave();
        }
        else
        {
            // fallback
            var loader = FindObjectOfType<SceneLoader>();
            if (loader != null)
            {
                loader.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }
    }
}
