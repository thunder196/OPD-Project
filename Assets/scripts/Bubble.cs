using System.Collections;
using UnityEngine;

public class Bubble: MonoBehaviour
{
    public BubbleType type;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public int x;
    public int y;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetType(BubbleType newType)
    {
        type = newType;
        UpdateColour();
    }

    void OnMouseDown()
    { 
        BubbleGrid grid = Object.FindAnyObjectByType<BubbleGrid>();
        if (grid != null)
            grid.CheckMatch(this);
    }

    public void UpdateColour()
    {
        switch (type)
        {
            case BubbleType.Red:
                spriteRenderer.color = Color.red; break;
            case BubbleType.Green:
                spriteRenderer.color= Color.green; break;
            case BubbleType.Blue: 
                spriteRenderer.color= Color.blue; break;
            case BubbleType.Yeelow:
                spriteRenderer.color= Color.yellow; break;
        }
    }

    public static IEnumerator FallToPosition(Transform obj, Vector3 targetPos, float duration = 0.2f)
    {
        if (obj == null) yield break;
        Vector3 startPos = obj.position;
        float time = 0;

        while (time < duration)
        {
            obj.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        if (obj != null)
            obj.position = targetPos;
        Destroy(obj.gameObject);
    }

}

public enum BubbleType
{ 
    Red = 1,
    Blue,
    Yeelow,
    Green
}