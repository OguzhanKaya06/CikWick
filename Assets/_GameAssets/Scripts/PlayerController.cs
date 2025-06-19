using System.Numerics;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _OrientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private KeyCode _movementKey;
    [SerializeField] private float _movementSpeed;

    [Header("Jumping Settings")]
    [SerializeField] private bool _canjump;
    [SerializeField] private KeyCode _jumpkey;
    [SerializeField] private float __jumpforce;
    [SerializeField] private float __jumpCooldown;

    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    private Rigidbody _playerRigidbody;

    private UnityEngine.Vector3 _movementDireciton;

    private float _verticalInput, _horizontalInput;

    private bool _isSliding;

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
    }

    private void Update()
    {
        SetInputs();

        LimitPlayerSpeed();
    }

    private void FixedUpdate()
    {
        SetPlayerMovement();
    }

    private void SetInputs()
    {
        _verticalInput = Input.GetAxisRaw("Vertical");
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;
            Debug.Log("Slide Mode");
        }
        else if (Input.GetKeyDown(_movementKey))
        {
            _isSliding = false;
            Debug.Log("Movement Mode");
        }
        else if (Input.GetKey(_jumpkey) && _canjump && IsGrounded())
        {
            _canjump = false;
            SetPlayerJump();
            Invoke(nameof(ResetJumping), __jumpCooldown);
        }
    }

    private void SetPlayerMovement()
    {
        _movementDireciton = _verticalInput * _OrientationTransform.forward
            + _horizontalInput * _OrientationTransform.right;

        if (_isSliding)
        {
            _playerRigidbody.AddForce(_movementDireciton.normalized * _movementSpeed * _slideMultiplier, ForceMode.Force);
        }
        else
        {
            _playerRigidbody.AddForce(_movementDireciton.normalized * _movementSpeed, ForceMode.Force);
        }
    }

    private void SetPlayerJump()
    {
        _playerRigidbody.linearVelocity = new UnityEngine.Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(transform.up * __jumpforce, ForceMode.Impulse);
    }

    private void SetPlayerDrag()
    {
        if (_isSliding)
        {
            _playerRigidbody.linearDamping = _slideDrag;
        }
        else
        {
            _playerRigidbody.linearDamping = _groundDrag;
        }
    }

    private void LimitPlayerSpeed()
    {
        UnityEngine.Vector3 flatVelocity = new UnityEngine.Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);

        if (flatVelocity.magnitude > _movementSpeed)
        {
            UnityEngine.Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigidbody.linearVelocity = new UnityEngine.Vector3(limitedVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void ResetJumping()
    {
        _canjump = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, UnityEngine.Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);
    }
}
