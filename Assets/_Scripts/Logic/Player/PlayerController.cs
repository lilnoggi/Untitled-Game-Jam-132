using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _chargeMoveSpeed = 2f; // Slower movement while charging a dash

    [Header("Dash Charging Setup")]
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _maxChargeTime = 0.5f; // How long to hold for a max jump

    [Header("Min/Max Durations & Cooldowns")]
    [SerializeField] private float _minDashDuration = 0.15f;
    [SerializeField] private float _maxDashDuration = 0.4f;
    [SerializeField] private float _minDashCooldown = 0.2f;
    [SerializeField] private float _maxDashCooldown = 0.8f;

    [Header("Hop Settings")]
    [SerializeField] private Transform _playerVisuals;
    [SerializeField] private float _minHopHeight = 0.5f;
    [SerializeField] private float _maxHopHeight = 1.8f;

    // References
    private Rigidbody2D _rb;
    private Vector2 _movementInput;
    private Vector2 _mousePosition;

    // Reference to Input Actions
    private InputSystem_Actions _inputActions;

    // Dash State Tracking
    private bool _isCharging;
    private bool _isDashing;
    private float _currentChargeTime;
    private float _dashTimeCounter;
    private float _dashCooldownCounter;
    private Vector2 _dashDirection;
    private Vector2 _lastMoveDirection = Vector2.right; // Default direction if standing still

    // Calculated values per hop
    private float _activeDashDuration;
    private float _activeHopHeight;

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

        // Listen for when the button is first pressed and when it is released
        _inputActions.Player.Dash.started += OnDashStarted;
        _inputActions.Player.Dash.canceled += OnDashCancelled;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        _inputActions.Player.Disable();

        _inputActions.Player.Dash.started -= OnDashStarted;
        _inputActions.Player.Dash.canceled -= OnDashCancelled;
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

        // --- CHARGING LOGIC ---
        if (_isCharging)
        {
            _currentChargeTime += Time.deltaTime;

            // Auto-trigger the hop if the player holds it past the maximum charge time
            if (_currentChargeTime >= _maxChargeTime)
            {
                ExecuteHop();
            }
        }

        // --- DASHING LOGIC ---
        if (_isDashing)
        {
            _dashTimeCounter -= Time.deltaTime;

            // Progress from 0.0 to 1.0 based on duration
            float dashProgress = 1f - (_dashTimeCounter / _activeDashDuration);

            // Sine wave to create a vertical arc (0 -> 1 -> 0)
            if (_playerVisuals != null)
            {
                float currentHeight = Mathf.Sin(dashProgress * Mathf.PI) * _activeHopHeight;
                _playerVisuals.localPosition = new Vector3(0f, currentHeight, 0f);
            }

            if (_dashTimeCounter <= 0)
            {
                _isDashing = false;

                if (_playerVisuals != null)
                {
                    _playerVisuals.localPosition = Vector3.zero;
                }
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
            // Lerp the speed so the hop starts fast and decelerates as the rabbit lands
            float dashProgress = 1f - (_dashTimeCounter / _activeDashDuration);
            float currentSpeed = Mathf.Lerp(_dashSpeed, _moveSpeed, dashProgress);

            // Lock movement to dash direction
            _rb.MovePosition(_rb.position + _dashDirection * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Apply a heavy-slow effect while player is charging
            float currentMoveSpeed = _isCharging ? _chargeMoveSpeed : _moveSpeed;
            
            // Standard movement
            _rb.MovePosition(_rb.position + _movementInput.normalized * currentMoveSpeed * Time.fixedDeltaTime);   
        }
    }

    // --- DASHING LOGIC ---
    private void OnDashStarted(InputAction.CallbackContext context)
    {
        // Only trigger if not already dashing and the cooldown has finished
        if (!_isDashing && _dashCooldownCounter <= 0f)
        {
            _isCharging = true;
            _currentChargeTime = 0f;
        }
    }

    private void OnDashCancelled(InputAction.CallbackContext context)
    {
        if (_isCharging)
        {
            ExecuteHop();
        }
    }

    private void ExecuteHop()
    {
        _isCharging = false;
        _isDashing = true;

        // Calculate a 0.0 to 1.0 percentage of how long the button has been held
        float chargePercent = Mathf.Clamp01(_currentChargeTime / _maxChargeTime);

        // Scale the jump's stats based on the charge percentage
        _activeDashDuration = Mathf.Lerp(_minDashDuration, _maxDashDuration, chargePercent);
        _activeHopHeight = Mathf.Lerp(_minHopHeight, _maxHopHeight, chargePercent);

        _dashCooldownCounter = Mathf.Lerp(_minDashCooldown, _maxDashCooldown, chargePercent);
        _dashTimeCounter = _activeDashDuration;

        // Dash in the current movement direction, or the last known direction if stationary
        _dashDirection = _movementInput != Vector2.zero ? _movementInput.normalized : _lastMoveDirection;
    }
}
