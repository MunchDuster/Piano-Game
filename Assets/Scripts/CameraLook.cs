using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 360;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        initialRotation = transform.rotation;
    }

    private Quaternion initialRotation;
    private float _xAngle;
    private float _yAngle;
    
    void Update()
    {
        _xAngle = Mathf.Clamp(_xAngle - Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime, -90f, 90f);
        _yAngle = Mathf.Clamp(_yAngle + Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime, -90f, 90f);
        transform.rotation = Quaternion.Euler(_xAngle, _yAngle, 0) * initialRotation;
    }
}
