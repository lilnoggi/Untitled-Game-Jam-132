using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIBobber : MonoBehaviour
{
    [SerializeField] private float _bobSpeed = 2f;
    [SerializeField] private float _bobHeight = 10f; 

    private RectTransform _rectTransform;
    private Vector2 _startPosition;
    private float _timer = 0f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.anchoredPosition;
    }

    private void Update()
    {
        _timer += Time.deltaTime * _bobSpeed;
        
        float currentHeight = Mathf.Sin(_timer) * _bobHeight;
        
        _rectTransform.anchoredPosition = _startPosition + new Vector2(0f, currentHeight);
    }
}