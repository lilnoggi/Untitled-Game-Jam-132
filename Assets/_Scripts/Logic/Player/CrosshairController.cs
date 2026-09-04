using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("Crosshair Graphics")]
    [SerializeField] private Sprite _defaultCrosshair;
    [SerializeField] private Sprite _targetLockCrosshair;

    [Header("Targeting")]
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Animation Settings")]
    [SerializeField] private float _spinSpeed = -90f; // Negative for clockwise spin

    // Reference to Input Actions
    private InputSystem_Actions _inputActions;

    // Reference to Camera
    private Camera _mainCamera;

    // Reference to SpriteRenderer
    private SpriteRenderer _sr;

    // ---------------------------------------------

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _mainCamera = Camera.main;
        _sr = GetComponent<SpriteRenderer>();

        // Hide the default OS cursor
        Cursor.visible = false;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }
        
        // Read the screen-space mouse position from the Look Action
        Vector2 screenMousePos = _inputActions.Player.Look.ReadValue<Vector2>();

        // Convert to world space and update the sprite's position
        Vector2 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, _mainCamera.nearClipPlane));
        transform.position = worldPos;

        // Apply constant radar spin
        transform.Rotate(0, 0, _spinSpeed * Time.deltaTime);

        // Check for enemies directly under crosshair world pos
        Collider2D hit = Physics2D.OverlapPoint(worldPos, _enemyLayer);

        // Trigger sprite swap
        OnHoverEnemy(hit != null);
    }

    // --- HOOKS FOR FUTURE EXPANXIONS ---

    public void OnFireWeapon()
    {
        // Recoil snap
        transform.Rotate(0, 0, -45f);
    }

    public void OnHoverEnemy(bool isHovering)
    {
        if (_sr != null)
        {
            // Swap between default and locked sprites
            _sr.sprite = isHovering ? _targetLockCrosshair : _defaultCrosshair;
        }
    }
}
