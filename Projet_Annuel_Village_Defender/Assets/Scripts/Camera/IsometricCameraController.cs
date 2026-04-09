using UnityEngine;
using UnityEngine.InputSystem;

public class IsometricCameraController : MonoBehaviour
{
    [Header("Paramètres")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;
    public float zoomSpeed = 5f;
    public float minSize = 2f;
    public float maxSize = 15f;

    private InputSystem_Actions _inputs;
    private Vector2 _moveInput;
    private float _rotateInput;
    private float _zoomInput;
    private float _XRotation;
    private Camera _cam;

    private void Awake()
    {
        _inputs = new InputSystem_Actions();
        _cam = GetComponentInChildren<Camera>();
        _XRotation = transform.rotation.eulerAngles.x;
    }

    private void OnEnable()
    {
        if (_inputs != null)
            _inputs.Enable();
    }

    private void OnDisable()
    {
        if (_inputs != null)
            _inputs.Disable();
    }

    private void OnDestroy()
    {
        if (_inputs != null)
        {
            _inputs.Disable();
            _inputs.Dispose();
        }
    }

    private void Update()
    {
        if (_inputs == null)
            return;
            
        // 1. Lire les valeurs de l'Input System
        _moveInput = _inputs.Camera.Move.ReadValue<Vector2>();
        _rotateInput = _inputs.Camera.Rotate.ReadValue<float>();
        _zoomInput = _inputs.Camera.Zoom.ReadValue<float>();

        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0; // On reste au sol
        right.y = 0;

        Vector3 direction = (forward * _moveInput.y + right * _moveInput.x).normalized;
        transform.position += direction * (moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (_rotateInput == 0)
            return;
            
        float rotationAmount = _rotateInput * rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(_XRotation, transform.rotation.eulerAngles.y + rotationAmount, 0.0f);
    }

    private void HandleZoom()
    {
        if (!_cam || _zoomInput == 0)
            return;

        
        if (_cam.orthographic)
        {
            _cam.orthographicSize -= (_zoomInput / 120f) * zoomSpeed;
            _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minSize, maxSize);
        }
    }
}