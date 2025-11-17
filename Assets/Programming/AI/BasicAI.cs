using UnityEngine;

public class BasicAI : MonoBehaviour, IDamagable
{
    [Header("AI Settings")]
    public float MoveSpeed = 5f;
    public float Damage = 10f;
    public float SpawnRadius = 40f;
    public float Ypos = 0f;
    private float lockedY;

    public float hp = 20f;

    [Header("References")]
    public Transform BoatTransform;

    private Rigidbody rb;
    private Vector3 spawnPoint;
    private bool returningToSpawn = false;
    private bool sinking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        lockedY = transform.position.y;

        // Pick random spawn point around boat
        Vector2 circle = Random.insideUnitCircle.normalized * SpawnRadius;
        spawnPoint = new Vector3(
            circle.x + BoatTransform.position.x,
            Ypos,
            circle.y + BoatTransform.position.z
        );

        transform.position = spawnPoint;
        returningToSpawn = false;
        sinking = false;

        rb.isKinematic = true;
        enabled = true;
    }

    private void Update()
    {
        if (sinking)
        {
            // Sink straight down
            transform.position += Vector3.down * Time.deltaTime * 10f;

            if (transform.position.y <= -100f)
                gameObject.SetActive(false);

            return;
        }

        if (!returningToSpawn)
        {
            // Move toward boat
            Vector3 direction = (BoatTransform.position - transform.position).normalized;
            Vector3 velocity = direction * MoveSpeed;
            transform.position += velocity * Time.deltaTime;

            RotateTowardMovement(velocity);
        }
        else
        {
            // Move back to origin point
            Vector3 backDir = (spawnPoint - transform.position).normalized;
            Vector3 velocity = backDir * MoveSpeed;
            transform.position += velocity * Time.deltaTime;

            RotateTowardMovement(velocity);

            float dist = Vector3.Distance(transform.position, spawnPoint);
            if (dist < 1f)
            {
                // Start sinking
                sinking = true;
            }
        }
        Vector3 pos = transform.position;
        pos.y = lockedY;
        transform.position = pos;
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0f)
        {
            Die();
            return;
        }
    }

    public void Die()
    {
        enabled = false;
        rb.isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == BoatTransform)
        {
            BoatHealthManager boat = BoatTransform.GetComponent<BoatHealthManager>();
            if (boat != null)
                boat.TakeDamage(Damage);

            // Start returning to spawn
            returningToSpawn = true;
        }
    }

    private void RotateTowardMovement(Vector3 velocity)
    {
        velocity.y = 0f; // keep upright

        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }
}
