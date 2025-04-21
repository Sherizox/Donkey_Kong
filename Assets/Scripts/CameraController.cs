using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float followSpeed = 5f;

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.3f;
    private float shakeTimer = 0f;
    private Vector3 shakeOffset;

    [Header("Zoom Settings")]
    public float zoomOutSize = 3f;
    public float zoomSpeed = 2f;
    private float originalSize;
    private Camera cam;

    [Header("Dynamic FOV")]
    public float zoomInSize = 4.5f;
    public float zoomMidSize = 5.5f;
    public float zoomOutSizeDynamic = 6.5f;
    public float fovChangeSpeed = 2f;

    private Transform playerTransform;
    private Rigidbody2D playerRb;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            originalSize = cam.orthographicSize;
        }
        if (target != null)
        {
            playerTransform = target;
            playerRb = target.GetComponent<Rigidbody2D>();
        }

    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Follow logic
        Vector3 desiredPosition = target.position + offset;

        // Shake logic
        if (shakeTimer > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0f;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
        // Apply final position
        transform.position = Vector3.Lerp(transform.position, desiredPosition + shakeOffset, followSpeed * Time.deltaTime);

        if (playerRb != null)
        {
            float velocityY = playerRb.linearVelocity.y;
            float velocityX = Mathf.Abs(playerRb.linearVelocity.x);
            float targetZoom = originalSize;

            if (velocityY > 1f) // jumping
            {
                targetZoom = zoomOutSizeDynamic;
            }
            else if (velocityY < -1f) // falling
            {
                targetZoom = zoomOutSizeDynamic;
            }
            else if (velocityX > 1f) // running
            {
                targetZoom = zoomMidSize;
            }
            else // idle/climbing
            {
                targetZoom = zoomInSize;
            }

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, fovChangeSpeed * Time.deltaTime);
        }

    }

    public void ShakeCamera()
    {
        shakeTimer = shakeDuration;
    }

    public void ZoomOutTemporarily()
    {
        if (cam != null)
        {
            StopAllCoroutines();
            StartCoroutine(ZoomRoutine());
        }
    }

    public void ZoomOnDeath()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomInOnDeathRoutine());
    }

    IEnumerator ZoomInOnDeathRoutine()
    {
        float targetZoom = 3.5f; // Adjust this for how close you want to zoom
        while (Mathf.Abs(cam.orthographicSize - targetZoom) > 0.05f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator ZoomRoutine()
    {
        cam.orthographicSize = zoomOutSize;
        yield return new WaitForSeconds(0.3f);

        while (Mathf.Abs(cam.orthographicSize - originalSize) > 0.05f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalSize, zoomSpeed * Time.deltaTime);
            yield return null;
        }

        cam.orthographicSize = originalSize;
    }

    public void ZoomDeath()
    {
        cam.orthographicSize = zoomOutSize;
    }
}