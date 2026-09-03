using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles weapon cycling, mouse-aiming rotation, and firing logic for the player.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Setup")]
    [SerializeField] private WeaponData[] _availableWeapons; // ScriptableObject data for each weapon
    [SerializeField] private Transform _weaponHolder; // The central point the weapons orbit around

    [Header("UI References")]
    [SerializeField] private CrosshairController _crosshair;

    private GameObject[] _instantiatedWeapons;
    private int _currentWeaponIndex = 0;
    private InputSystem_Actions _inputActions;
    private Camera _mainCamera;

    // Firing state variables
    private bool _isFiring;
    private float _timeSinceLastFire = 0f;

    // ----------------------------------------------------------

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        // Subscribe to the Q and E input events
        _inputActions.Player.CyclePrevious.performed += CyclePrevious;
        _inputActions.Player.CycleNext.performed += CycleNext;

        // Read the Left-MB hold state from the Attack action
        _inputActions.Player.Attack.started += ctx => _isFiring = true;
        _inputActions.Player.Attack.canceled += ctx => _isFiring = false;
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();

        // Unsubscribe to prevent memory leaks
        _inputActions.Player.CyclePrevious.performed -= CyclePrevious;
        _inputActions.Player.CycleNext.performed -= CycleNext;

        _inputActions.Player.Attack.started -= ctx => _isFiring = true;
        _inputActions.Player.Attack.canceled -= ctx => _isFiring = false;
    }

    private void Start() 
    {
        // Pre-spawn all weapons at the start of the game
        _instantiatedWeapons = new GameObject[_availableWeapons.Length];

        for (int i = 0; i < _availableWeapons.Length; i++)
        {
            if (_availableWeapons[i].WeaponPrefab != null)
            {
                GameObject weapon = Instantiate(_availableWeapons[i].WeaponPrefab, _weaponHolder);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.transform.localScale = Vector3.one;
                weapon.SetActive(false);
                _instantiatedWeapons[i] = weapon;
            }
        }

        EquipWeapon(_currentWeaponIndex);
    }

    private void Update()
    {
        AimWeapon();
        HandleShooting();
    }

    /// <summary>
    /// Calculates the angle between the player and the mouse, rotating the weapon holder to aim.
    /// Flips the Y-scale when aiming left to prevent the weapon sprite from beingu upside down
    /// </summary>
    private void AimWeapon()
    {
        // Read mouse position and convert to world space
        Vector2 screenMousePos = _inputActions.Player.Look.ReadValue<Vector2>();
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, _mainCamera.nearClipPlane));

        // Calculate the angle between the weapon holder and the mouse
        Vector2 aimDirection = worldPos - _weaponHolder.position;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Apply the rotation to the Weapon Holder
        _weaponHolder.rotation = Quaternion.Euler(0, 0, angle);

        // Flip the weapon holder's Y scale if aiming to the left
        if (angle > 90 || angle < -90)
        {
            _weaponHolder.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            _weaponHolder.localScale = new Vector3(1, 1, 1);
        }
    }

    private void HandleShooting()
    {
        _timeSinceLastFire += Time.deltaTime;

        WeaponData currentData = _availableWeapons[_currentWeaponIndex];

        // Check if player is holding click and enough time has passed based on the fire rate
        if (_isFiring && _timeSinceLastFire >= currentData.FireRate)
        {
            FireWeapon(currentData);
        }
    }

    /// <summary>
    /// Requests a projectile from the PoolManager and applies the active weapon's stats
    /// </summary>
    /// <param name="data">The Weapon ScriptableObject currently equipped</param>
    private void FireWeapon(WeaponData data)
    {
        _timeSinceLastFire = 0f;

        // Find the specific FirePoint transform on the currently active weapon
        Transform firePoint = _instantiatedWeapons[_currentWeaponIndex].transform.Find("FirePoint");

        if (firePoint != null && data.ProjectilePrefab != null)
        {
            // Calculate a random spread angle based on the weapon data
            float randomSpread = Random.Range(-data.SpreadAngle, data.SpreadAngle);
            Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0, 0, randomSpread);
            
            // Get a bullet from the pool manager
            GameObject bullet = PoolManager.Instance.SpawnFromPool(data.ProjectilePrefab, firePoint.position, firePoint.rotation);

            // Pass the speed and damage from the weapon to the bullet
            bullet.GetComponent<ProjectileBehaviour>().InitialiseProjectile(data.BulletSpeed, data.Damage);

            // Trigger crosshair animatino
            if (_crosshair != null)
            {
                _crosshair.OnFireWeapon();
            }
        }
    }

    private void CycleNext(InputAction.CallbackContext context)
    {
        _currentWeaponIndex = (_currentWeaponIndex + 1) % _availableWeapons.Length;
        EquipWeapon(_currentWeaponIndex);
    }

    private void CyclePrevious(InputAction.CallbackContext context)
    {
        _currentWeaponIndex--;
        if (_currentWeaponIndex < 0)
        {
            _currentWeaponIndex = _availableWeapons.Length - 1;
        }

        EquipWeapon(_currentWeaponIndex);
    }

    private void EquipWeapon(int index)
    {
        if (_availableWeapons == null || _availableWeapons.Length == 0 || 
            _instantiatedWeapons == null || _instantiatedWeapons.Length == 0)
        {
            return;
        }

        foreach (GameObject weapon in _instantiatedWeapons)
        {
            if (weapon != null)
            {
                weapon.SetActive(false);
            }
        }

        // Turn on the selected weapon
        if (_instantiatedWeapons[index] != null)
        {
            _instantiatedWeapons[index].SetActive(true);
        }
    }
}
