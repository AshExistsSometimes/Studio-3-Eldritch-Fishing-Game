using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SeaSerpentAI with refined circling smoothing, proper spawn teleport, leap cooldown, stun/resurface/fall mechanics,
/// and event stubs for OnStunStart/OnLeapStart/OnResurfaceStart/OnDie.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SeaSerpentAI : MonoBehaviour, IDamagable
{
    public enum State { CircleTarget, Attack, Leap, Fall, Submerge, Resurface, Flee, Stunned }

    [Header("References")]
    public Transform BoatTransform;
    public Transform HeadTransform;
    public Collider MouthCollider;

    [Header("Spawn")]
    public float SpawnRadius = 50f;                 // teleported to circumference of this radius at spawn

    [Header("Movement")]
    public float circleRadius = 20f;
    public float MovementSpeed = 3f;
    public float attackImpulseForce = 60f;
    public float leapImpulseForce = 40f;
    public float leapUpForce = 20f;
    public float submergeDepth = -10f;
    public float SubmergeRadius = 75f;
    public float submergeSpeed = 5f;
    public float fleeSpeed = 10f;
    public float OceanHeight = 0f;
    public float ResurfaceSpeed = 5f;

    [Header("Timing & Smoothing")]
    public float velocitySmoothing = 6f;            // higher = snappier; lower = smoother
    public float LeapCooldown = 3f;                 // seconds before leap allowed again after finishing a fall

    [Header("Combat")]
    public float Damage = 20f;
    public float MaxHP = 200f;

    [Header("State Timing (Randomized)")]
    public Vector2 CircleTimeRange = new Vector2(4f, 8f);
    public Vector2 AttackTimeRange = new Vector2(1f, 2f);
    public Vector2 SubmergeTimeRange = new Vector2(3f, 6f);
    public Vector2 FleeTimeRange = new Vector2(5f, 10f);
    public Vector2 StunTimeRange = new Vector2(3f, 5f);

    [Header("Debug")]
    [SerializeField] private State currentState = State.Submerge;

    // internals
    private Rigidbody rb;
    public float hp;
    private float stateTimer = 0f;
    private float currentStateDuration = 0f;
    private bool reachedCircleRadius = false;
    private bool attackImpulsed = false;
    private bool leapImpulsed = false;
    private float lastLeapTime = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        hp = MaxHP;

        if (MouthCollider != null)
            MouthCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        hp = MaxHP;
        // Teleport to a random point on the circumference of SpawnRadius (not inside)
        if (BoatTransform != null)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * SpawnRadius;
            Vector3 spawnPos = BoatTransform.position + offset;
            spawnPos.y = OceanHeight + submergeDepth; // spawn submerged
            transform.position = spawnPos;
            Debug.Log(spawnPos);
        }

        EnterState(State.Submerge);
    }

    private void FixedUpdate()
    {
        if (BoatTransform == null) return;

        stateTimer += Time.fixedDeltaTime;

        switch (currentState)
        {
            case State.CircleTarget:
                DoCircleTarget();
                if (reachedCircleRadius && stateTimer >= currentStateDuration)
                    TransitionToRandomSurfaceState(exclude: State.CircleTarget);
                break;

            case State.Attack:
                DoAttack();
                if (stateTimer >= currentStateDuration)
                {
                    if (hp <= MaxHP * 0.1f)
                        EnterState(State.Flee);
                    else
                        EnterState(State.CircleTarget);
                }
                break;

            case State.Leap:
                DoLeap();
                break;

            case State.Fall:
                DoFall();
                break;

            case State.Submerge:
                DoSubmerge();
                if (stateTimer >= currentStateDuration)
                    EnterState(State.Resurface);
                break;

            case State.Resurface:
                DoResurface();
                break;

            case State.Flee:
                DoFlee();
                if (stateTimer >= currentStateDuration)
                    EnterState(State.CircleTarget);
                break;

            case State.Stunned:
                if (stateTimer >= currentStateDuration)
                    TransitionAfterStun();
                break;
        }

        // Failsafe: if farther than circleRadius, immediately go to CircleTarget (unless in Leap/Fall/Stunned)
        float distToBoatXZ = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                                              new Vector3(BoatTransform.position.x, 0f, BoatTransform.position.z));
        if (distToBoatXZ > circleRadius && currentState != State.Leap && currentState != State.Fall && currentState != State.Stunned)
        {
            if (currentState != State.CircleTarget)
                EnterState(State.CircleTarget);
        }

        // Maintain Y for non-Leap/Fall/Submerge/Resurface states: smooth to OceanHeight
        if (currentState != State.Leap && currentState != State.Fall && currentState != State.Submerge && currentState != State.Resurface)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(pos.y, OceanHeight, 2f * Time.fixedDeltaTime);
            transform.position = pos;

            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
        }

        // Head orientation: 3D in Leap/Fall, horizontal otherwise
        if (HeadTransform != null)
        {
            Vector3 headVel = rb.linearVelocity;
            if (currentState == State.Leap || currentState == State.Fall)
            {
                if (headVel.sqrMagnitude > 0.01f)
                    HeadTransform.forward = headVel.normalized;
            }
            else
            {
                headVel.y = 0f;
                if (headVel.sqrMagnitude > 0.01f)
                    HeadTransform.forward = headVel.normalized;
            }
        }
    }

    #region State Behaviors

    private void DoCircleTarget()
    {
        Vector3 toBoat = BoatTransform.position - transform.position;
        toBoat.y = 0f;
        float dist = toBoat.magnitude;

        // Desired position at circleRadius
        Vector3 desiredPos = BoatTransform.position - toBoat.normalized * circleRadius;
        Vector3 toDesired = desiredPos - transform.position;
        toDesired.y = 0f;
        Vector3 approachDir = toDesired.sqrMagnitude > 0.001f ? toDesired.normalized : Vector3.zero;

        // Tangent for circling
        Vector3 tangent = Vector3.Cross(Vector3.up, (transform.position - BoatTransform.position)).normalized;

        // Blend approach + tangential movement; approach stronger when further from radius
        float radialError = Mathf.Abs(dist - circleRadius);
        float approachWeight = Mathf.Clamp01(radialError / (circleRadius * 0.5f));
        Vector3 targetVel = (approachDir * MovementSpeed * approachWeight) + (tangent * MovementSpeed);

        // Smoothly interpolate velocity to reduce jitter
        Vector3 smoothed = Vector3.Lerp(rb.linearVelocity, new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z), velocitySmoothing * Time.fixedDeltaTime);
        rb.linearVelocity = smoothed;

        // Mark reached radius (timer starts) when close enough
        if (!reachedCircleRadius)
        {
            if (radialError <= 0.5f)
            {
                reachedCircleRadius = true;
                stateTimer = 0f;
                currentStateDuration = Random.Range(CircleTimeRange.x, CircleTimeRange.y);
            }
        }
    }

    private void DoAttack()
    {
        // Face boat horizontally
        Vector3 toBoat = BoatTransform.position - transform.position;
        toBoat.y = 0f;
        if (toBoat.sqrMagnitude > 0.001f)
        {
            Vector3 faceDir = toBoat.normalized;
            transform.forward = faceDir;
            if (HeadTransform != null)
                HeadTransform.forward = faceDir;
        }

        if (!attackImpulsed)
        {
            attackImpulsed = true;
            // Clean dash: zero lateral velocity
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);

            // Impulse along forward (horizontal)
            Vector3 impulseDir = transform.forward;
            impulseDir.y = 0f;
            impulseDir.Normalize();
            rb.AddForce(impulseDir * attackImpulseForce, ForceMode.Impulse);

            // initialize attack timer
            stateTimer = 0f;
            currentStateDuration = Random.Range(AttackTimeRange.x, AttackTimeRange.y);
        }
    }

    private void DoLeap()
    {
        // Prevent immediate re-leap via cooldown check if we've recently fallen
        if (Time.time - lastLeapTime < LeapCooldown)
        {
            // Skip leap; transition to Circle to avoid repeated leap attempts
            TransitionToRandomSurfaceState();
            return;
        }

        if (!leapImpulsed)
        {
            leapImpulsed = true;
            lastLeapTime = Time.time;

            rb.useGravity = true;

            // Determine forward: prefer horizontal velocity, else toward boat
            Vector3 forward = rb.linearVelocity;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.1f)
                forward = (BoatTransform.position - transform.position).normalized;

            Vector3 leapDir = (forward.normalized + Vector3.up).normalized;

            // Clear vertical momentum then impulse
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(leapDir * leapImpulseForce + Vector3.up * leapUpForce, ForceMode.Impulse);

            // Fire event stub
            OnLeapStart();

            // Immediately go to Fall to let gravity handle descent
            EnterState(State.Fall);
        }
    }

    private void DoFall()
    {
        // Natural fall under gravity. When at or below ocean, finish fall.
        if (transform.position.y <= OceanHeight + 0.1f)
        {
            Vector3 pos = transform.position;
            pos.y = OceanHeight;
            transform.position = pos;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.useGravity = false;

            // Choose next surface state (but Leap is gated by LeapCooldown)
            TransitionToRandomSurfaceState();
        }
    }

    private void DoSubmerge()
    {
        float targetY = OceanHeight + submergeDepth;
        Vector3 pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, targetY, 5f * Time.fixedDeltaTime);

        Vector3 toBoat = BoatTransform.position - transform.position;
        toBoat.y = 0f;
        float dist = toBoat.magnitude;
        Vector3 tangent = Vector3.Cross(Vector3.up, (transform.position - BoatTransform.position)).normalized;

        if (dist > SubmergeRadius + 0.5f)
        {
            Vector3 desiredPos = BoatTransform.position - toBoat.normalized * SubmergeRadius;
            Vector3 approach = desiredPos - transform.position;
            approach.y = 0f;
            Vector3 approachDir = approach.sqrMagnitude > 0.001f ? approach.normalized : Vector3.zero;
            Vector3 finalVel = approachDir * submergeSpeed + tangent * (submergeSpeed * 0.6f);
            Vector3 smoothed = Vector3.Lerp(rb.linearVelocity, new Vector3(finalVel.x, rb.linearVelocity.y, finalVel.z), velocitySmoothing * Time.fixedDeltaTime);
            rb.linearVelocity = smoothed;
        }
        else if (dist < SubmergeRadius - 0.5f)
        {
            Vector3 outward = (transform.position - BoatTransform.position);
            outward.y = 0f;
            if (outward.sqrMagnitude > 0.001f)
                rb.linearVelocity = outward.normalized * submergeSpeed;
            else
                rb.linearVelocity = tangent * submergeSpeed * 0.5f;
        }
        else
        {
            Vector3 desired = new Vector3(tangent.x * submergeSpeed, rb.linearVelocity.y, tangent.z * submergeSpeed);
            Vector3 smoothed = Vector3.Lerp(rb.linearVelocity, desired, velocitySmoothing * Time.fixedDeltaTime);
            rb.linearVelocity = smoothed;
        }

        transform.position = pos;
    }

    private void DoResurface()
    {
        // Rise smoothly then pick next state
        Vector3 pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, OceanHeight, ResurfaceSpeed * Time.fixedDeltaTime);
        transform.position = pos;

        // gentle horizontal circling while resurfacing
        Vector3 toBoat = BoatTransform.position - transform.position;
        toBoat.y = 0f;
        Vector3 tangent = Vector3.Cross(Vector3.up, (transform.position - BoatTransform.position)).normalized;
        Vector3 targetHoriz = tangent * (submergeSpeed * 0.6f);
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(targetHoriz.x, rb.linearVelocity.y, targetHoriz.z), velocitySmoothing * Time.fixedDeltaTime);

        if (Mathf.Approximately(transform.position.y, OceanHeight))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            // Fire resurface event
            OnResurfaceStart();
            TransitionToRandomSurfaceState();
        }
    }

    private void DoFlee()
    {
        Vector3 dir = (transform.position - BoatTransform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, dir.normalized * fleeSpeed, velocitySmoothing * Time.fixedDeltaTime);
        else
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, velocitySmoothing * Time.fixedDeltaTime);
    }

    #endregion

    #region State management

    private void EnterState(State next)
    {
        currentState = next;
        stateTimer = 0f;
        reachedCircleRadius = false;
        attackImpulsed = false;
        leapImpulsed = false;

        switch (next)
        {
            case State.CircleTarget:
                currentStateDuration = Random.Range(CircleTimeRange.x, CircleTimeRange.y);
                break;
            case State.Attack:
                currentStateDuration = Random.Range(AttackTimeRange.x, AttackTimeRange.y);
                break;
            case State.Submerge:
                currentStateDuration = Random.Range(SubmergeTimeRange.x, SubmergeTimeRange.y);
                break;
            case State.Flee:
                currentStateDuration = Random.Range(FleeTimeRange.x, FleeTimeRange.y);
                break;
            case State.Stunned:
                currentStateDuration = Random.Range(StunTimeRange.x, StunTimeRange.y);
                rb.linearVelocity = Vector3.zero;
                rb.useGravity = false;
                OnStunStart();
                break;
            default:
                currentStateDuration = 0f;
                break;
        }
    }

    /// <summary>Choose among surface-level states, but exclude Leap if LeapCooldown hasn't passed.</summary>
    private void TransitionToRandomSurfaceState(State? exclude = null)
    {
        List<State> possible = new List<State> { State.CircleTarget, State.Attack, State.Leap, State.Submerge };
        if (hp <= MaxHP * 0.1f) possible.Add(State.Flee);
        if (exclude.HasValue) possible.Remove(exclude.Value);

        // If Leap cooldown not satisfied, remove Leap option
        if (Time.time - lastLeapTime < LeapCooldown)
            possible.Remove(State.Leap);

        if (possible.Count == 0)
        {
            EnterState(State.CircleTarget);
            return;
        }

        State next = possible[Random.Range(0, possible.Count)];
        EnterState(next);
    }

    private void TransitionAfterStun()
    {
        List<State> possible = new List<State> { State.CircleTarget, State.Submerge, State.Flee };
        State next = possible[Random.Range(0, possible.Count)];
        EnterState(next);
    }

    #endregion

    #region Damage & collision

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0f)
        {
            Die();
            return;
        }

        if (hp <= MaxHP * 0.1f)
            EnterState(State.Flee);
    }

    public void Die()
    {
        enabled = false;
        rb.isKinematic = false;
        OnDie();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == BoatTransform)
        {
            if (currentState == State.Attack)
            {
                BoatHealthManager boat = BoatTransform.GetComponent<BoatHealthManager>();
                if (boat != null)
                    boat.TakeDamage(Damage);

                // Cause stun and zero momentum to prevent phasing
                EnterState(State.Stunned);
            }
        }
    }

    #endregion

    #region Event stubs (empty by design; place hooks here)

    private void OnStunStart()
    {
        // Logic Here
    }

    private void OnLeapStart()
    {
        // Logic Here
    }

    private void OnResurfaceStart()
    {
        // Logic Here
    }

    private void OnDie()
    {
        // Logic Here
    }

    #endregion
}
