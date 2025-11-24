using UnityEngine;

public class TentacleHitbox : MonoBehaviour
{
    [Header("References")]
    public KrakenTentacleEvent krakenEvent;

    [Header("References")]
    public Transform BoatTransform;

    private void Reset()
    {
        krakenEvent = GetComponentInParent<KrakenTentacleEvent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == BoatTransform)
        {
            BoatHealthManager boat = BoatTransform.GetComponent<BoatHealthManager>();
            if (boat != null)
                boat.TakeDamage(krakenEvent.Damage);
        }
    }
}
