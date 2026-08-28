using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBehaviour : MonoBehaviour
{
    private float _speed;
    private float _damage;
    private Rigidbody2D _rb;

    // ----------------------------------------

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Configures the projectile's velocity and damage values immediately upon spawning from the pool.
    /// </summary>
    /// <param name="speed">The speed the projectile travels.</param>
    /// <param name="damage">The amount of damage the projectile deals when hit.</param>
    public void InitialiseProjectile(float speed, float damage)
    {
        _speed = speed;
        _damage = damage;

        // Fire the projectile forward along its local X axis
        _rb.linearVelocity = transform.right * _speed;

        // TEMPORARY auto-cleanup to prevent memory leaks until PoolManager is built
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object implements the damage interface
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }      

        // TODO: Spawn impact VFX here

        // Return the bullet to the pool
        PoolManager.Instance.ReturnToPool(gameObject);
    }
}
