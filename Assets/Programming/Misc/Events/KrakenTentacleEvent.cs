using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TentacleState
{
    public Transform tentacle;          // Assigned in Inspector
    [HideInInspector] public Vector3 defaultLocalPos;
    [HideInInspector] public bool isMoving = true;
}

public class KrakenTentacleEvent : MonoBehaviour, IDamagable
{
    [Header("Tentacle Settings")]
    public List<TentacleState> TentacleStates = new List<TentacleState>();

    [Tooltip("The final top Y-position that the tentacles rise to.")]
    public float RiseHeight = 0f;

    public float RiseSpeed = 5f;
    public float BobAmplitude = 0.5f;
    public float BobSpeed = 1f;

    [Header("Attack Settings")]
    public float AttackIntervalMin = 10f;
    public float AttackIntervalMax = 20f;
    public float AttackSpeed = 4f;
    public float AttackReturnSpeed = 3f;
    public float Damage = 10f;

    [Header("Health Settings")]
    public float hp = 50f;
    public float MaxHp = 50f;

    [Header("Boat Reference")]
    public Transform Boat;

    [Header("Rotation")]
    public float RotationSpeed = 10f;

    [Header("Fog Settings")]
    public SceneManager sceneManager;
    public Color DayFogColour = Color.yellow;
    public float DayFogDensity = 0.001f;
    [Space]
    public Color NightFogColour = Color.green;
    public float NightFogDensity = 0.001f;

    [Header("Debug")]
    public bool IsAttacking = false;

    // Internal state
    private bool bobbing = false;
    private bool eventRunning = false;

    private void Awake()
    {
        // Auto-fill tentacles if none assigned manually
        if (TentacleStates.Count == 0)
        {
            foreach (Transform child in transform)
            {
                TentacleState t = new TentacleState();
                t.tentacle = child;
                TentacleStates.Add(t);
            }
        }

        // Store default local positions (X/Z + RiseHeight for Y)
        foreach (var t in TentacleStates)
        {
            Vector3 pos = t.tentacle.localPosition;
            pos.y = RiseHeight;
            t.defaultLocalPos = pos;

            t.isMoving = true;
        }

        // Overwrite Fog
        sceneManager.FogOverwritten = true;
        if (sceneManager.IsDay)
        {
            sceneManager.FogOverwriteColour = DayFogColour;
            sceneManager.FogOverwriteDensity = DayFogDensity;
        }
        else
        {
            sceneManager.FogOverwriteColour = NightFogColour;
            sceneManager.FogOverwriteDensity = NightFogDensity;
        }
    }

    private void OnEnable()
    {
        // Move tentacles down to starting underground height
        foreach (var t in TentacleStates)
        {
            Vector3 p = t.defaultLocalPos;
            p.y = RiseHeight - 50f;
            t.tentacle.localPosition = p;
            t.isMoving = true;
        }

        eventRunning = true;
        bobbing = false;
        hp = MaxHp;

        StartCoroutine(EventRoutine());
    }

    private void Update()
    {
        if (!eventRunning) return;

        // Follow boat XZ
        if (Boat != null)
        {
            Vector3 center = new Vector3(Boat.position.x, transform.position.y, Boat.position.z);
            transform.position = center;
        }

        // Rotate Kraken root
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
    }

    private IEnumerator EventRoutine()
    {
        yield return StartCoroutine(RiseTentacles());

        StartCoroutine(BobbingRoutine());
        StartCoroutine(RandomAttackRoutine());
    }

    // ------------------------------
    // RISE
    // ------------------------------
    private IEnumerator RiseTentacles()
    {
        bool allAtHeight = false;

        while (!allAtHeight)
        {
            allAtHeight = true;

            foreach (var t in TentacleStates)
            {
                Vector3 current = t.tentacle.localPosition;
                Vector3 target = t.defaultLocalPos;

                current.y = Mathf.MoveTowards(current.y, target.y, RiseSpeed * Time.deltaTime);
                t.tentacle.localPosition = current;

                if (current.y < target.y - 0.05f)
                    allAtHeight = false;
            }

            yield return null;
        }

        bobbing = true;
    }

    // ------------------------------
    // BOBBING
    // ------------------------------
    private IEnumerator BobbingRoutine()
    {
        float timer = 0f;

        while (eventRunning)
        {
            if (!bobbing)
            {
                yield return null;
                continue;
            }

            timer += Time.deltaTime * BobSpeed;

            for (int i = 0; i < TentacleStates.Count; i++)
            {
                var t = TentacleStates[i];

                if (!t.isMoving)
                    continue; // Skip tentacles that are attacking

                float dir = (i % 2 == 0) ? 1f : -1f;
                float bob = Mathf.Sin(timer) * BobAmplitude * dir;

                Vector3 basePos = t.defaultLocalPos;

                t.tentacle.localPosition = new Vector3(
                    basePos.x,
                    basePos.y + bob,
                    basePos.z
                );
            }

            yield return null;
        }
    }

    // ------------------------------
    // RANDOM ATTACKS
    // ------------------------------
    private IEnumerator RandomAttackRoutine()
    {
        while (eventRunning)
        {
            float wait = Random.Range(AttackIntervalMin, AttackIntervalMax);
            yield return new WaitForSeconds(wait);

            // Pick random tentacle
            int i = Random.Range(0, TentacleStates.Count);
            TentacleState t = TentacleStates[i];

            yield return StartCoroutine(AttackTentacle(t));
        }
    }

    // ------------------------------
    // ATTACK SEQUENCE
    // ------------------------------
    private IEnumerator AttackTentacle(TentacleState tentacle)
    {
        IsAttacking = true;
        tentacle.isMoving = false; // Stop bobbing/rotation overrides

        Vector3 original = tentacle.defaultLocalPos;
        Vector3 attackPos = new Vector3(0f, original.y, 0f); // local center

        // Move in
        while (Vector3.Distance(tentacle.tentacle.localPosition, attackPos) > 0.1f)
        {
            tentacle.tentacle.localPosition = Vector3.MoveTowards(
                tentacle.tentacle.localPosition,
                attackPos,
                AttackSpeed * Time.deltaTime
            );
            yield return null;
        }

        // TODO: Damage boat here

        yield return new WaitForSeconds(0.4f);

        // Move back out
        while (Vector3.Distance(tentacle.tentacle.localPosition, original) > 0.1f)
        {
            tentacle.tentacle.localPosition = Vector3.MoveTowards(
                tentacle.tentacle.localPosition,
                original,
                AttackReturnSpeed * Time.deltaTime
            );
            yield return null;
        }

        tentacle.isMoving = true;
        IsAttacking = false;
    }

    // ------------------------------
    // HEALTH / DEATH
    // ------------------------------
    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0f)
            Die();
    }

    public void Die()
    {
        StopAllCoroutines();
        eventRunning = false;
        bobbing = false;

        StartCoroutine(SinkAndDisable());
    }

    private IEnumerator SinkAndDisable()
    {
        bool done = false;

        while (!done)
        {
            done = true;

            foreach (var t in TentacleStates)
            {
                Vector3 pos = t.tentacle.localPosition;
                float targetY = RiseHeight - 50f;

                pos.y = Mathf.MoveTowards(pos.y, targetY, RiseSpeed * Time.deltaTime);
                t.tentacle.localPosition = pos;

                if (pos.y > targetY + 0.1f)
                    done = false;
            }

            yield return null;
        }
        sceneManager.FogOverwritten = false;
        gameObject.SetActive(false);
    }
}
