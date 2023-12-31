using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    
    private BoxCollider2D _cameraBounds;  // Reference to the box collider for camera bounds
    private Transform _followTransform;  // Reference to the player's transform
    [SerializeField] private float _tiltDegrees = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
    }

    private void Start()
    {
        _cameraBounds = GetComponentInParent<BoxCollider2D>();
        _followTransform = PlayerController.Instance.transform;
    }

    private void LateUpdate()
    {
        if (_followTransform == null)
            return;

        Camera cameraComponent = GetComponentInChildren<Camera>();
        float cameraHeight = 2.0f * cameraComponent.orthographicSize;
        float cameraWidth = cameraHeight * cameraComponent.aspect;

        // Calculate the camera bounds based on the box collider and adjusted camera size
        var bounds = _cameraBounds.bounds;
        float boundsMinX = bounds.min.x + cameraWidth / 2;
        float boundsMaxX = bounds.max.x - cameraWidth / 2;
        float boundsMinY = bounds.min.y + cameraHeight / 2;
        float boundsMaxY = bounds.max.y - cameraHeight / 2;

        // Adjust the desired position to be slightly above the player
        var position = _followTransform.position;
        Vector3 desiredPosition = new Vector3(position.x, position.y + 0.5f, transform.position.z);

        // Clamp camera position within the specified bounds
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, boundsMinX, boundsMaxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, boundsMinY, boundsMaxY);

        transform.position = desiredPosition;

        // Smoothly interpolate the camera's rotation to the target rotation
        if (PlayerController.Instance.CanMove)
        {
            // Calculate the tilt based on the player's movement direction
            float playerDirection = (Input.GetAxis("Horizontal")); 
            Quaternion targetRotation = Quaternion.Euler(0, 0, -playerDirection * _tiltDegrees);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,0,0), Time.deltaTime * 10.0f);
        }
    }

    public void StartZoomIn(float targetZoom, float duration)
    {
        StartCoroutine(ZoomInCoroutine(targetZoom, duration));
    }

    private IEnumerator ZoomInCoroutine(float targetZoom, float duration)
    {
        Camera cameraComponent = GetComponentInChildren<Camera>();
        float initialZoom = cameraComponent.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            
            // Smooth interpolation using SmoothStep
            t = Mathf.SmoothStep(0f, 1f, t);

            cameraComponent.orthographicSize = Mathf.Lerp(initialZoom, targetZoom, t);

            // Recalculate camera dimensions based on the updated orthographic size
            float cameraHeight = 2.0f * cameraComponent.orthographicSize;
            float cameraWidth = cameraHeight * cameraComponent.aspect;

            // Recalculate the camera bounds based on the updated camera size
            var bounds = _cameraBounds.bounds;
            float boundsMinX = bounds.min.x + cameraWidth / 2;
            float boundsMaxX = bounds.max.x - cameraWidth / 2;
            float boundsMinY = bounds.min.y + cameraHeight / 2;
            float boundsMaxY = bounds.max.y - cameraHeight / 2;

            // Clamp camera position within the updated bounds
            Vector3 desiredPosition = transform.position;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, boundsMinX, boundsMaxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, boundsMinY, boundsMaxY);
            transform.position = desiredPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraComponent.orthographicSize = targetZoom;
    }
}
