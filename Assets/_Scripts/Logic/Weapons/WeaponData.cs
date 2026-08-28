using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Rabbit Gun Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private WeaponType _type;
    [SerializeField] private string _weaponName;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _damage;
    [SerializeField] private float _spreadAngle;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _weaponPrefab;

    // ---------------------------------------------------------

    // --- GETTERS ---
    public WeaponType Type => _type;
    public string WeaponName => _weaponName;
    public float FireRate => _fireRate;
    public float Damage => _damage;
    public float SpreadAngle => _spreadAngle;
    public float BulletSpeed => _bulletSpeed;
    public GameObject ProjectilePrefab => _projectilePrefab;
    public GameObject WeaponPrefab => _weaponPrefab;
}
