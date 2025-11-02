using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SeaSerpentAI : MonoBehaviour, IDamagable
{
    public enum State { CircleTarget, Attack, Leap, Submerge, Flee }

    [Header("References")]
    public Transform BoatTransform;            // The target boat
    public Transform HeadTransform;            // Visual head transform used to face direction

    [Header("Spawn")]
    public float SpawnRadius = 50f;            // random spawn radius around boat

    [Header("Movement")]
    public float circleRadius = 20f;           // distance to hold while circling
    public float circleSpeed = 3f;             // tangential speed when circling
    public float attackForce = 50f;            // force applied when charging
    public float leapForce = 20f;              // upward force when leaping
    public float submergeDepth = -10f;         // y position to hide
    public float fleeSpeed = 10f;              // flee movement speed

    [Header("Combat")]
    public float Damage = 20f;                 // damage to boat on contact during attack
    public float MaxHP = 200f;

    [Header("Timing")]
    public float circleDuration = 6f;
    public float attackDuration = 3f;
    public float leapDuration = 2f;
    public float submergeDuration = 4f;

    private Rigidbody rb;
    private State currentState = State.CircleTarget;
    private float hp;
    private float stateTimer = 0f;

    // IDamagable property
    public float HP { get => hp; set => hp = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hp = MaxHP;
    }

    private void OnEnable()
    {
        if (BoatTransform != null)
        {
            // Spawn at random point around the boat
            Vector2 rnd = Random.insideUnitCircle.normalized * Random.Range(0f, SpawnRadius);
            transform.position = BoatTransform.position + new Vector3(rnd.x, 0f, rnd.y);
        }

        // Begin state
        currentState = State.CircleTarget;
        stateTimer = 0f;
    }

    private void FixedUpdate()
    {
        if (BoatTransform == null) return;

        stateTimer += Time.fixedDeltaTime;

        switch (currentState)
        {
            case State.CircleTarget:
                DoCircleTarget();
                if (stateTimer >= circleDuration)
                    TransitionTo(State.Attack);
                break;

            case State.Attack:
                DoAttack();
                if (stateTimer >= attackDuration)
                    TransitionTo(State.Leap);
                break;

            case State.Leap:
                DoLeap();
                if (stateTimer >= leapDuration)
                    TransitionTo(State.Submerge);
                break;

            case State.Submerge:
                DoSubmerge();
                if (stateTimer >= submergeDuration)
                    TransitionTo(State.CircleTarget);
                break;

            case State.Flee:
                DoFlee();
                // remain in flee
                break;
        }

        // Face head towards velocity if moving
        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;
        if (vel.sqrMagnitude > 0.01f && HeadTransform != null)
            HeadTransform.forward = vel.normalized;
    }

    /// <summary>Circular orbit around boat using centripetal adjustments.</summary>
    private void DoCircleTarget()
    {
        Vector3 toBoat = (transform.position - BoatTransform.position);
        toBoat.y = 0f;
        Vector3 desiredPos = BoatTransform.position + toBoat.normalized * circleRadius;
        Vector3 toDesired = desiredPos - transform.position;
        // Apply velocity towards desired tangential direction
        Vector3 tangent = Vector3.Cross(Vector3.up, (transform.position - BoatTransform.position)).normalized;
        Vector3 move = tangent * circleSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

    /// <summary>Charge toward boat with force.</summary>
    private void DoAttack()
    {
        Vector3 dir = (BoatTransform.position - transform.position).normalized;
        rb.AddForce(dir * attackForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    /// <summary>Leap up and forward so player can shoot; single impulse at start of state.</summary>
    private bool leapImpulsed = false;
    private void DoLeap()
    {
        if (!leapImpulsed)
        {
            // impulse forward + up
            Vector3 dir = (BoatTransform.position - transform.position).normalized;
            rb.AddForce(dir * attackForce * 0.5f + Vector3.up * leapForce, ForceMode.Impulse);
            leapImpulsed = true;
        }
    }

    /// <summary>Submerge to a depth and remain mostly immobile below surface.</summary>
    private void DoSubmerge()
    {
        // gradually move towards submergeDepth y
        Vector3 pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, submergeDepth, 5f * Time.fixedDeltaTime);
        transform.position = pos;
        // damp velocity
        rb.linearVelocity *= 0.9f;
    }

    /// <summary>Flee from boat at a set speed.</summary>
    private void DoFlee()
    {
        Vector3 dir = (transform.position - BoatTransform.position).normalized;
        rb.linearVelocity = dir * fleeSpeed;
    }

    private void TransitionTo(State next)
    {
        currentState = next;
        stateTimer = 0f;
        leapImpulsed = false;
    }

    /// <summary>Take damage and call Die() when HP <= 0.</summary>
    /// <param name="amount">Damage amount.</param>
    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0f) Die();
        else if (hp <= MaxHP * 0.1f) TransitionTo(State.Flee);
    }

    /// <summary>Ragdoll / death handling. For now, disable AI and enable ragdoll by making rigidbody non-kinematic.</summary>
    public void Die()
    {
        // simple death: stop AI and enable ragdoll-ish behaviour (no animation system provided)
        enabled = false;
        rb.isKinematic = false;
        // optionally add other cleanup (particles, loot drop)
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If colliding with boat while in Attack, deal damage
        if (collision.transform == BoatTransform && currentState == State.Attack)
        {
            IDamagable boat = BoatTransform.GetComponent<IDamagable>();
            if (boat != null)
            {
                boat.TakeDamage(Damage);
            }
        }
    }
}
