using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ControlTest : MonoBehaviour
{
    private InputAction _leanAction = null;
    private InputAction _moveAction = null;
    private InputAction _swingAction = null;
    
    private Vector2 _vecLeftStick = Vector2.zero;
    private Vector2 _vecRightStick = Vector2.zero;
    private float _valRightTrigger = 0;
    
    private float _curRotationY = 0f;
    
    [Header("Player Control")]
    public PlayerInput control;

    [Header("Transforms")]
    public Transform character;
    public Transform body;
    public Transform arm;
    public Transform lookTarget;
    
    [Header("Rotation Angle Limits in degree")]
    public float maxLeanAngleZ = 45;
    public float maxLeanAngleX = 45;
    public float maxSwingAngleY = 60;

    [Header("Movement Speeds")]
    public float rotateSpeed = 100;
    public float moveSpeed = 2;

    [Header("Control Thresholds")]
    public float thresholdLeftStick = 0.05f;
    public float thresholdRightStick = 0.05f;
    public float thresholdRightTrigger = 0.1f;

    [Header("Coord")]
    [Tooltip("Please choose X_Positive option in MGP1")]
    public ControlScheme forwardDirectionLeftStick = ControlScheme.Z_Positive;
    private Vector2 _vecForwardLeftStick = Vector2.zero;
    private Vector2 _vecInwardLeftStick = Vector2.zero;

    [Header("DEBUG mode")]
    public bool isDebug = false;
    
    [Header("Unused - please DO NOT change for now")]
    public float leanSpeed = 1;

    private Vector3 _initArmRot = Vector3.zero;

    private void Awake()
    {
        if (control != null)
        {
            _leanAction = control.actions["Lean"];
            _moveAction = control.actions["Move"];
            _swingAction = control.actions["Swing"];
        }

        _initArmRot = arm.eulerAngles;
        
        SetCoord();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateControl();
        UpdateLookTarget();
        UpdateRotate();
        UpdateMove();
        UpdateSwing();
    }

    private void UpdateControl()
    {
        Vector2 leftStick = _leanAction.ReadValue<Vector2>();
        _vecLeftStick = leftStick.magnitude >= thresholdLeftStick
            ? leftStick[0] * _vecForwardLeftStick + leftStick[1] * _vecInwardLeftStick
            : Vector2.zero;

        _vecRightStick = _swingAction.ReadValue<Vector2>();
        _valRightTrigger = _moveAction.ReadValue<float>();
    }
    
    private void UpdateLookTarget()
    {
        lookTarget.localPosition = new Vector3(_vecLeftStick[0], 1, _vecLeftStick[1]);
    }

    private void UpdateRotate()
    {
        if (_moveAction.WasReleasedThisFrame())
        {
            _curRotationY = body.eulerAngles.y;
        }
        
        Vector3 curDirectionLocal = body.InverseTransformDirection(new Vector3(_vecLeftStick[0], 0, _vecLeftStick[1]));

        float xAngle = curDirectionLocal.z * maxLeanAngleZ;
        float zAngle = -curDirectionLocal.x * maxLeanAngleX;
        float yAngle = _curRotationY;
        
        if (_moveAction.IsPressed() && _vecLeftStick.magnitude != 0)
        {
            Quaternion leanRotation = Quaternion.LookRotation(-body.position + lookTarget.position, body.up);
            yAngle = Mathf.Clamp(body.eulerAngles.y + rotateSpeed * Time.deltaTime, 0, leanRotation.eulerAngles.y);
        }
        
        body.rotation = Quaternion.Euler(new Vector3(xAngle, yAngle, zAngle));
        
        /* IF DEBUG */
        if (isDebug)
        {
            // Debug.Log(_curRotationY);
            // Debug.Log(curDirectionLocal);
            // Debug.DrawLine(Vector3.zero, curDirectionLocal, Color.blue, 100);
            // Debug.Log(_leanAction.ReadValue<Vector2>());
            Debug.Log(_vecLeftStick);
        }
    }

    private void UpdateMove()
    {
        character.Translate(_vecLeftStick[0] * _valRightTrigger * moveSpeed * Time.deltaTime, 
            0, 
            _vecLeftStick[1] * _valRightTrigger * moveSpeed * Time.deltaTime, Space.World);
    }

    private void UpdateSwing()
    {
       arm.localRotation = 
           Quaternion.Euler(new Vector3(0, _vecRightStick[0] * maxSwingAngleY - 90, 45));
    }

    private void SetCoord()
    {
        switch(forwardDirectionLeftStick)
        {
            case ControlScheme.X_Positive:
                _vecForwardLeftStick = Vector2.right;
                _vecInwardLeftStick = Vector2.up;
                break;
            case ControlScheme.X_Negative:
                _vecForwardLeftStick = Vector2.left;
                _vecInwardLeftStick = Vector2.down;
                break;
            case ControlScheme.Z_Positive:
                _vecForwardLeftStick = Vector2.up;
                _vecInwardLeftStick = Vector2.left;
                break;
            case ControlScheme.Z_Negative:
                _vecForwardLeftStick = Vector2.down;
                _vecInwardLeftStick = Vector2.right;
                break;
            default:
                _vecForwardLeftStick = Vector2.up;
                _vecInwardLeftStick = Vector2.left;
                break;
        }
    }
}
