using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BuyEvent : Interactable
{
    public int Cost = 1;

    public UnityEvent ItemBought;

    private AnalyticsManager analytics;

    private void Awake()
    {
        analytics = AnalyticsManager.Instance;
    }


    public override void OnMouseHover()
    {
        {
            if (_isGlowing)
                return;

            _isGlowing = true;
            if (_materialInstance.HasProperty("_EmissionColor"))
            {
                _materialInstance.EnableKeyword("_EMISSION");
                _materialInstance.SetColor("_EmissionColor", GlowColor * GlowIntensity);
            }

            if (interactionPrompt != null)
            {
                if (EconomyManager.instance.Currency < Cost)// If players money is lower than cost
                {
                    interactionText = gameObject.name + " is too expensive ($" + Cost + ")";
                    interactionPrompt.enabled = true;
                }

                if (EconomyManager.instance.Currency >= Cost)// If players has enough money
                {
                    interactionText = "- Buy " + gameObject.name + " for $" + Cost + "<br> You have: $" + EconomyManager.instance.Currency;
                    interactionPrompt.enabled = true;
                }
            }
        }
    }

    public override void OnInteract()
    {
        if (EconomyManager.instance.Currency < Cost)// If it costs more than the player has
        {
            interactionText = "Can't afford this right now";
            analytics.AddString("Player tried to buy " + gameObject.name + " for " + Cost + " but couldn't afford it - They had: $" + EconomyManager.instance.Currency);
            interactionPrompt.enabled = true;
            StartCoroutine(CantAfford());
            return;
        }
        else// If player can afford it
        {
            Debug.Log("- Buying " + gameObject.name);
            analytics.AddString("Player bought " + gameObject.name + " for " + Cost);
            EconomyManager.instance.TryRemoveMoney(Cost);
            ItemBought.Invoke();
        }
    }
    public IEnumerator CantAfford()
    {
        yield return new WaitForSeconds(1.5f);
        interactionPrompt.enabled = false;
    }
    private void OnMouseExit()
    {
        Debug.Log("- Mouse off");
        interactionText = "Press 'E' to Interact";
    }
    
}
