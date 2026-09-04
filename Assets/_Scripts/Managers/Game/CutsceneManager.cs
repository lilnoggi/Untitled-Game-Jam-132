using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[Serializable]
public struct CutsceneSlide
{
    public Sprite SlideImage;
    public string SpeakerName;
    [TextArea(3, 5)]
    public string DialogueText;
}

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _slideDisplay;
    [SerializeField] private TextMeshProUGUI _speakerName;
    // [SerializeField] private TextMeshProUGUI _dialogueText;

    [SerializeField] private TypewriterEffect _typeWriterEffect;

    [Header("Narrative Data")]
    [SerializeField] private CutsceneSlide[] _slides;
    [SerializeField] private string _nextSceneName = "01_Gameplay_Scene";

    private int _currentSlideIndex = 0;
    private InputSystem_Actions _inputActions;

    // ---------------------------------------------------------------------------

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputActions.UI.Enable();
        _inputActions.UI.Click.started += HandleClick;
    }

    private void OnDisable()
    {
        _inputActions.UI.Disable();
        _inputActions.UI.Click.started -= HandleClick;
    }

    private void Start()
    {
        ShowSlide(_currentSlideIndex);
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        if (_typeWriterEffect.IsTyping)
        {
            // If text is typing, click to skip to end
            _typeWriterEffect.SkipToFullText();
        }
        else
        {
            // If text is finished typing, click to load the next slide
            _currentSlideIndex++;

            if (_currentSlideIndex < _slides.Length)
            {
                ShowSlide(_currentSlideIndex);
            }
            else
            {
                SceneManager.LoadScene(_nextSceneName);
            }
        }
    }

    private void ShowSlide(int index)
    {
        if (_slideDisplay != null && _slides[index].SlideImage != null)
        {
            _slideDisplay.sprite = _slides[index].SlideImage;
        }

        if (_typeWriterEffect != null)
        {
            _typeWriterEffect.StartTyping(_slides[index].DialogueText);
        }

        if (_speakerName != null)
        {
            _speakerName.text = _slides[index].SpeakerName;
        }
    }
}
