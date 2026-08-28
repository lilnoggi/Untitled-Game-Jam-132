using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairController : MonoBehaviour
{
    // Reference to Input Actions
    private InputSystem_Actions _inputActions;

    // Reference to Camera
    private Camera _mainCamera;

    // ---------------------------------------------

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _mainCamera = Camera.main;

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
        // Read the screen-space mouse position from the Look Action
        Vector2 screenMousePos = _inputActions.Player.Look.ReadValue<Vector2>();

        // Convert to world space and update the sprite's position
        Vector2 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, _mainCamera.nearClipPlane));
        transform.position = worldPos;
    }

    // --- HOOKS FOR FUTURE EXPANXIONS ---

    public void OnFireWeapon()
    {
        // TODO: Add tweening logic to scale when the Carrot Launcher fires
    }

    public void OnHoverEnemy(bool isHovering)
    {
        // TODO: Add SpriteRenderer colour change logic when hovering over enemies
    }
}
