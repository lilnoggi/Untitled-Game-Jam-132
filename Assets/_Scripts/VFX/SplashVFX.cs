using UnityEngine;

public class SplashVFX : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke(nameof(Deactivate), 0.2f);
    }

    private void Deactivate()
    {
        PoolManager.Instance.ReturnToPool(gameObject);
    }
}
