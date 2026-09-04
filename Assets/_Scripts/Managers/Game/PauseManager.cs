using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel;
    [SerializeField] private GameObject _settingsPanel;

    private InputSystem_Actions _inputActions;
    private bool _isPaused = false;

    // ----------------------------------------------------------

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        _settingsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        _inputActions.Player.Pause.performed += OnPauseInput;
        _inputActions.UI.Cancel.performed += OnPauseInput;
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Pause.performed -= OnPauseInput;
        _inputActions.UI.Cancel.performed -= OnPauseInput;

        _inputActions.Player.Disable();
        _inputActions.UI.Disable();
    }

    private void OnPauseInput(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0f; // Stop all physics and movement
            _pauseMenuPanel.SetActive(true);

            // Swap inputs to prevent shooting while clicking menus
            _inputActions.Player.Disable();
            _inputActions.UI.Enable();

            // Show cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f; // Resume normal time
            _pauseMenuPanel.SetActive(false);
            _settingsPanel.SetActive(false);

            // Restore player controls
            _inputActions.UI.Disable();
            _inputActions.Player.Enable();

            // Hide cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void OpenSettingsPanel()
    {
        _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        _settingsPanel.SetActive(false);
    }
}
