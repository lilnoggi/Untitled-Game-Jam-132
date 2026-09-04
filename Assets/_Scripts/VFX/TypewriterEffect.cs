using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float _typingSpeed = 0.03f;

    private TextMeshProUGUI _textComponent;
    private Coroutine _typingCoroutine;
    private string _fullText;

    public bool IsTyping { get; private set; }

    private void Awake()
    {
        _textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void StartTyping(string textToType)
    {
        _fullText = textToType;
        _textComponent.text = "";

        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        _typingCoroutine = StartCoroutine(TypeTextRoutine());
    }

    public void SkipToFullText()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }

        _textComponent.text = _fullText;
        IsTyping = false;
    }

    private IEnumerator TypeTextRoutine()
    {
        IsTyping = true;
        _textComponent.text = "";

        // Iterate through each character and append it to the UI text component
        foreach (char c in _fullText)
        {
            _textComponent.text += c;

            // Pause at the end of sentences
            if (c == '.' || c == '!' || c == '?')
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                // Standard speed
                yield return new WaitForSeconds(_typingSpeed);   
            }
        }

        IsTyping = false;
    }
}
