using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeGrower : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float growSpeed = 2f;

    [Header("Attached Moving Object")]
    [SerializeField] private Transform movingObject;
    [SerializeField] private float endYOffset = 0;

    private float currentHeight = 0f;
    private float targetHeight = 0f;
    private bool isGrowing = false;
    private bool isRetracting = false;
    private System.Action onRetractComplete;

    public bool IsActive => isGrowing || isRetracting;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
    }

    public void Initialize(float height, Transform point)
    {
        targetHeight = height;
        currentHeight = 0f;
        isGrowing = false;
        isRetracting = false;
        lineRenderer.enabled = true;

        startPoint = point;
    }

    public void Grow()
    {
        currentHeight = 0f;
        isGrowing = true;
        isRetracting = false;
        lineRenderer.enabled = true;
    }

    public void Shrink(System.Action onComplete = null)
    {
        if (!IsActive)
        {
            onRetractComplete = onComplete;
            isRetracting = true;
            isGrowing = false;
        }
    }

    void Update()
    {
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
            currentHeight -= Time.deltaTime * growSpeed;
            if (currentHeight <= 0f)
            {
                currentHeight = 0f;
                isRetracting = false;
                lineRenderer.enabled = false;
                onRetractComplete?.Invoke();
            }
        }

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (!lineRenderer.enabled) return;

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
