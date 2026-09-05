using UnityEngine;

public class CarrotExplosion : MonoBehaviour
{
    [Header("Explosion Stats")]
    [SerializeField] private float _explosionRadius = 2.5f;
    [SerializeField] private float _explosionDamage = 50f;
    [SerializeField] private float _animationLength = 0.5f;
    [SerializeField] private LayerMask _damageableLayerMask;

    // --------------------------------------------------------

    private void OnEnable()
    {
        // Detect all colliders within the blast radius on the specific layer
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _damageableLayerMask);

        // Loop through everything caught in the blast and apply damage
        foreach (Collider2D obj in hitObjects)
        {
            IDamageable damageable = obj.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_explosionDamage);
            }
        }

        Invoke(nameof(Deactivate), _animationLength);
    }

    private void Deactivate()
    {
        PoolManager.Instance.ReturnToPool(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a red circle in the Scene View to view blast radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
