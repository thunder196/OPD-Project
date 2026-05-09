using System.Collections;
using UnityEngine;

public class Bubble: MonoBehaviour
{
    public BubbleType type;
    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public float x;
    public float y;
    public float b;
    public bool isStopped=false;
    float rowHeight = 0.93f;
    public bool startCondition;
    private Vector2 lastVelocity;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb= GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lastVelocity = rb.linearVelocity;
    }
    public void SetType(BubbleType newType)
    {
        type = newType;
        UpdateColour();
    }

    public void UpdateColour()
    {
        switch (type)
        {
            case BubbleType.Red:
                spriteRenderer.color = Color.red; break;
            case BubbleType.Green:
                spriteRenderer.color = Color.green; break;
            case BubbleType.Blue:
                spriteRenderer.color = Color.blue; break;
            case BubbleType.Yellow:
                spriteRenderer.color = Color.yellow; break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isStopped) return;
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 wallNormal = collision.contacts[0].normal;
            Vector2 newDirection = Vector2.Reflect(lastVelocity.normalized, wallNormal);
            rb.linearVelocity = newDirection * lastVelocity.magnitude;
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg; 
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Ceil"))
        {
            StopBall();
        }
    }


    void StopBall()
    {
        isStopped = true;
        rb.linearVelocity = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        y = Mathf.RoundToInt(transform.position.y / rowHeight);
        b = ((int)y % 2 == 0) ? 0.5f : 0f;
        x = Mathf.RoundToInt(transform.position.x - b);
        x = Mathf.Clamp(x, 0f, 9f); 
        transform.position = new Vector3(x + b, y*rowHeight, 0);
        BubbleGrid grid = FindAnyObjectByType<BubbleGrid>();
        if (grid != null)
        {
            if (x >= 0 && x < grid.width && y >= 0 && y < grid.height && !startCondition)
            {
                grid.grid[(int)x, (int)y] = this;
                grid.CheckMatch(this);
            }
        }
        startCondition = true;
    }

    public static IEnumerator FallToPosition(Transform obj, Vector3 targetPos, float duration = 0.4f)
    {
        if (obj == null) yield break;
        Vector3 startPos = obj.position;
        float time = 0;

        while (time < duration)
        {
            if (obj == null) yield break;

            obj.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        if (obj != null)
        {
            Destroy(obj.gameObject);
        }
    }
}


public enum BubbleType
{ 
    Red = 1,
    Blue,
    Yellow,
    Green
}