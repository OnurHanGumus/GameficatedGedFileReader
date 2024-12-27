using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    public List<int> cameraZoomValues;
    private int _selectedCameraValue;
    [SerializeField] Joystick joystick;
    [SerializeField] private Rigidbody rb;

    private void Start()
    {
        _selectedCameraValue = 0;
    }

    public void ZoomIn()
    {
        if (_selectedCameraValue == cameraZoomValues.Count - 1)
        {
            return;
        }
        ++_selectedCameraValue;
        Zoom();
    }

    public void ZoomOut()
    {
        if (_selectedCameraValue == 0)
        {
            return;
        }
        --_selectedCameraValue;
        Zoom();
    }

    private void Zoom()
    {
        cameraTransform.position = new Vector3(cameraTransform.position.x, cameraZoomValues[_selectedCameraValue], cameraTransform.position.z);
    }

    private void LateUpdate()
    {
        rb.velocity = new Vector3(joystick.Direction.x, 0, joystick.Direction.y) * -1 * cameraZoomValues[_selectedCameraValue];
    }
}