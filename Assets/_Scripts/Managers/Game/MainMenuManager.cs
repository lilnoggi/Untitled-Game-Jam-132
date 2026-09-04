using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainButtonsContainer;
    [SerializeField] private GameObject _settingsPanel;

    private string _nextSceneName = "00b_IntroCutscene_Scene";

    // ----------------------------------------------------------

    private void Start()
    {
        // Ensure settings panel is off
        _settingsPanel.SetActive(false);
    }

    // Attatch to Settings Button
    public void OpenSettings()
    {
        // Turn off the main buttons, and set the settings panel active
        _mainButtonsContainer.SetActive(false);
        _settingsPanel.SetActive(true);
    }

    // Attach to Back button in settings panel
    public void CloseSettings()
    {
        _settingsPanel.SetActive(false);
        _mainButtonsContainer.SetActive(true);
    }

    public void PlayStoryMode()
    {
        SceneManager.LoadScene(_nextSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}