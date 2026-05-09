using UnityEngine;

public class shooter: MonoBehaviour
{
    public GameObject bubblePrefab;
    public Transform firePoint;
    public float shootForce = 10f;
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private GameObject currentBall;
    public float ballRadius = 0.5f;
    public float maxDistance = 8f;
    public int maxReflections = 2;
    public LayerMask collisionMask;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        PrepareNextBall();
    }
    void Update()
    {
        DrawTrajectory();
        if (Input.GetMouseButtonDown(0))
            Shoot(); 
    }

    void Shoot()
    {
        if (currentBall == null) return;
        currentBall.transform.SetParent(null);
        var rb = currentBall.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direction = ((Vector2)mousePos - (Vector2)firePoint.position).normalized;
        rb.linearVelocity = direction * shootForce;
        currentBall = null;
        Invoke("PrepareNextBall", 0.4f);
    }

    void PrepareNextBall()
    {
        currentBall = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        currentBall.transform.SetParent(firePoint);
        var rb = currentBall.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        Bubble bubble = currentBall.GetComponent <Bubble>();
        bubble.SetType((BubbleType)Random.Range(1, 5));
        bubble.isStopped = false;
    }

    void DrawTrajectory()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 10f;

        var worldMousePos = mainCamera.ScreenToWorldPoint(mousePos);

        var direction =
            ((Vector2)worldMousePos - (Vector2)firePoint.position).normalized;

        Vector2 startPosition = (Vector2)firePoint.position + direction * ballRadius;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPosition);

        int currentReflection = 0;

        while (currentReflection < maxReflections)
        {
            RaycastHit2D hit = Physics2D.CircleCast(
                startPosition,
                ballRadius,
                direction,
                maxDistance,
                collisionMask
            );

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == currentBall)
                {
                    startPosition += direction * 0.05f;
                    continue;
                }
                lineRenderer.positionCount++;

                lineRenderer.SetPosition(
                    lineRenderer.positionCount - 1,
                    hit.centroid
                );
                if (hit.collider.CompareTag("Ball"))
                {
                    break;
                }

                Vector2 reflectedDirection =
                Vector2.Reflect(direction, hit.normal).normalized;

                direction = reflectedDirection;

                startPosition =hit.centroid + reflectedDirection * 0.02f;

                currentReflection++;
            }
            else
            {
                lineRenderer.positionCount++;

                lineRenderer.SetPosition(
                    lineRenderer.positionCount - 1,
                    startPosition + direction * maxDistance
                );

                break;
            }
        }
    }

}
