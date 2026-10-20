using JollyPanda.LastFlag.Handlers;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeGrower : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float growSpeed = 2f;
    [Tooltip("Retracting speeds up like a falling rope (units per second²).")]
    [SerializeField] private float dropAcceleration = 25f;

    [Header("Attached Moving Object")]
    [SerializeField] private Transform movingObject;
    [SerializeField] private float endYOffset = 0;

    private float currentHeight = 0f;
    private float targetHeight = 0f;
    private float dropSpeed;
    private bool isGrowing = false;
    private bool isRetracting = false;
    private bool isPaused;
    private System.Action onRetractComplete;

    public bool IsActive => isGrowing || isRetracting;

    /// <summary>True while any part of the rope is visible.</summary>
    public bool IsVisible => lineRenderer != null && lineRenderer.enabled;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
    }

    private void OnEnable() => Informant.OnPause += OnPause;

    private void OnDisable() => Informant.OnPause -= OnPause;

    private void OnPause(bool pause) => isPaused = pause;

    public void Initialize(float height, Transform point)
    {
        targetHeight = height;
        startPoint = point;
        Hide();
    }

    public void Grow()
    {
        currentHeight = 0f;
        isGrowing = true;
        isRetracting = false;
        onRetractComplete = null;
        lineRenderer.enabled = true;
    }

    /// <summary>Lets the rope fall down. Works at any moment, also while it is still growing.</summary>
    public void Shrink(System.Action onComplete = null)
    {
        if (!IsVisible)
        {
            onComplete?.Invoke();
            return;
        }

        if (!isRetracting)
            dropSpeed = growSpeed;

        onRetractComplete = onComplete;
        isRetracting = true;
        isGrowing = false;
    }

    /// <summary>Removes the rope instantly (new wave, back to home).</summary>
    public void Hide()
    {
        currentHeight = 0f;
        isGrowing = false;
        isRetracting = false;
        onRetractComplete = null;

        // park the hook at the bottom, otherwise it stays where the rope ended
        if (movingObject != null && startPoint != null)
            movingObject.position = startPoint.position + Vector3.up * endYOffset;

        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (isPaused)
            return;

        if (isGrowing)
        {
            currentHeight += Time.deltaTime * growSpeed;
            if (currentHeight >= targetHeight)
            {
                currentHeight = targetHeight;
                isGrowing = false;
            }
        }
        else if (isRetracting)
        {
            dropSpeed += dropAcceleration * Time.deltaTime;
            currentHeight -= Time.deltaTime * dropSpeed;
            if (currentHeight <= 0f)
            {
                var callback = onRetractComplete;
                Hide();
                callback?.Invoke();
            }
        }

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (!lineRenderer.enabled || startPoint == null) return;

        Vector3 startPos = startPoint.position;
        Vector3 endPos = startPos + Vector3.up * currentHeight + (Vector3.up * endYOffset);

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        if (movingObject != null)
        {
            movingObject.position = endPos;
        }
    }
}
