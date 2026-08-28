using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;

    // References
    private Rigidbody2D _rb;
    private Vector2 _movementInput;
    private Vector2 _mousePosition;

    // Reference to Input Actions
    private InputSystem_Actions _inputActions;

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
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        // Read continous Vector2 input from the Move Action
        _movementInput = _inputActions.Player.Move.ReadValue<Vector2>();

        // Read the screen-space mouse position from the Look Action
        Vector2 screenMousePos = _inputActions.Player.Look.ReadValue<Vector2>();
        _mousePosition = Camera.main.ScreenToWorldPoint(screenMousePos);
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void FixedUpdate()
    {
        // Apply physics-based movement using the Rigidbody2D
        _rb.MovePosition(_rb.position + _movementInput.normalized * _moveSpeed * Time.fixedDeltaTime);
    }
}
