using UnityEngine;

// Interface for anything that can take damage and die.
public interface IDamagable
{
    // Apply damage to this object. should call Die() when HP <= 0.   -   amount will be amount of damage taken
    void TakeDamage(float amount);

    // Called when HP reaches zero or below. Implement death behavior here.
    void Die();
}

