using System.Collections;
using UnityEngine;

public class Bubble: MonoBehaviour
{
    public BubbleType type;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetType(BubbleType newType)
    { 
        type = newType;
        UpdateColour();
    }

    void OnMouseDown()
    {
        Match3Grid grid = Object.FindAnyObjectByType<Match3Grid>();
        if (grid != null)
        {
            grid.SelectBubble(this);
        }
    }

    public static IEnumerator FallToPosition(Transform obj, Vector3 targetPos, float duration = 0.2f)
    {
        if (obj==null) yield break;
        Vector3 startPos = obj.position;
        float time = 0;

        while (time < duration)
        {
            obj.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        if (obj!=null)
            obj.position = targetPos;
    }

    void UpdateColour()
    {
        switch (type)
        {
            case BubbleType.Red:
                spriteRenderer.color = Color.red; 
                break;
            case BubbleType.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case BubbleType.Green:
                spriteRenderer.color = Color.green;
                break;
            case BubbleType.Yellow:
                spriteRenderer.color = Color.yellow;
                break;
        }
    }
}
public enum BubbleType
{ 
    Red=1,
    Blue,
    Green,
    Yellow
}