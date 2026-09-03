using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;

    // References
    private Rigidbody2D _rb;
    private Vector2 _movementInput;
    private Vector2 _mousePosition;

    // Reference to Input Actions
    private InputSystem_Actions _inputActions;

    // Dash State Tracking
    private bool _isDashing;
    private float _dashTimeCounter;
    private float _dashCooldownCounter;
    private Vector2 _dashDirection;
    private Vector2 _lastMoveDirection = Vector2.right; // Default direction if standing still

    // ---------------------------------------------------

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _inputActions = new InputSystem_Actions();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        _inputActions.Player.Enable();

        // Subscribe to Dash action
        _inputActions.Player.Dash.performed += OnDashPerformed;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        _inputActions.Player.Disable();

        _inputActions.Player.Dash.performed -= OnDashPerformed;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        // Read continous Vector2 input from the Move Action
        _movementInput = _inputActions.Player.Move.ReadValue<Vector2>();

        // Store the last direction the player moved so they can dash from a standstill
        if (_movementInput != Vector2.zero)
        {
            _lastMoveDirection = _movementInput.normalized;
        }

        // Read the screen-space mouse position from the Look Action
        Vector2 screenMousePos = _inputActions.Player.Look.ReadValue<Vector2>();
        _mousePosition = Camera.main.ScreenToWorldPoint(screenMousePos);

        // Handle Dash Timers
        if (_isDashing)
        {
            _dashTimeCounter -= Time.deltaTime;
            if (_dashTimeCounter <= 0)
            {
                _isDashing = false;
            }
        }

        if (_dashCooldownCounter > 0f)
        {
            _dashCooldownCounter -= Time.deltaTime;
        }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        // Apply physics-based movement using the Rigidbody2D
        if (_isDashing)
        {
            // Lock movement to dash direction
            _rb.MovePosition(_rb.position + _dashDirection * _dashSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Standard movement
            _rb.MovePosition(_rb.position + _movementInput.normalized * _moveSpeed * Time.fixedDeltaTime);   
        }
    }

    // Dash Methods
    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        // Only trigger if not already dashing and the cooldown has finished
        if (!_isDashing && _dashCooldownCounter <= 0f)
        {
            _isDashing = true;
            _dashTimeCounter = _dashDuration;
            _dashCooldownCounter = _dashCooldown;

            // Dash in the current movement direction, or the last known direction if stationary
            _dashDirection = _movementInput != Vector2.zero ? _movementInput.normalized : _lastMoveDirection;
        }
    }
}
