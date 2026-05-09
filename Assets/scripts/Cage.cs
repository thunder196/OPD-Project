using System.Collections;
using UnityEngine;

public class Cage: MonoBehaviour
{
    public CageType type;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetType(CageType newType, Sprite visual)
    { 
        type = newType;
        spriteRenderer.sprite = visual;
        spriteRenderer.color = Color.white;
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
}
public enum CageType
{ 
    Red=1,
    Blue,
    Green,
    Yellow
}