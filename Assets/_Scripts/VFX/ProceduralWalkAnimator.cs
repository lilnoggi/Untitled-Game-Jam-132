using UnityEngine;

public class ProceduralWalkAnimator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _visualTransform;

    [Header("Bob Settings")]
    [SerializeField] private float _bobHeight = 0.25f;
    [SerializeField] private float _bobSpeed = 10f;

    [Header("Rotation Settings")]
    [SerializeField] private float _maxRotationAngle = 5f;
    [SerializeField] private float _rotationSpeed = 5f;

    [Header("State")]
    [SerializeField] private bool _isMoving = true;

    private float _animationTimer = 0f;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    public bool IsMoving
    {
        get => _isMoving;
        set => _isMoving = value;
    }

    // -----------------------------------------------------

    private void Awake()
    {
        if (_visualTransform != null)
        {
            _startPosition = _visualTransform.localPosition;
            _startRotation = _visualTransform.localRotation;
        }
    }

    private void Update()
    {
        if (_visualTransform == null || Time.timeScale == 0f)
        {
            return;
        }

        if (_isMoving)
        {
            _animationTimer += Time.deltaTime;

            // Bobbing (Abs Sine Wave for sharp bounces)
            float currentHeight = Mathf.Abs(Mathf.Sin(_animationTimer * _bobSpeed)) * _bobHeight;
            _visualTransform.localPosition = _startPosition + new Vector3(0f, currentHeight, 0f);

            // Waddling (Standard Sine Wave for smooth back & forth rotation)
            float currentAngle = Mathf.Sin(_animationTimer * _rotationSpeed) * _maxRotationAngle;
            _visualTransform.localRotation = _startRotation * Quaternion.Euler(0f, 0f, currentAngle);
        }
        else
        {
            // Smoothly return to the default resting pose when stopped
            _animationTimer = 0f;
            _visualTransform.localPosition = Vector3.Lerp(_visualTransform.localPosition, _startPosition, Time.deltaTime * 10f);
            _visualTransform.localRotation = Quaternion.Lerp(_visualTransform.localRotation, _startRotation, Time.deltaTime * 10f);
        }
    }
}
