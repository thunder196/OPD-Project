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
        int x = Mathf.FloorToInt(transform.position.x / grid.spacing);
        int y = Mathf.FloorToInt(transform.position.y / grid.spacing);
        x = Mathf.Clamp(x, 0, grid.width - 1);
        y = Mathf.Clamp(y, 0, grid.height - 1);

        grid.CheckMatch(x, y);
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