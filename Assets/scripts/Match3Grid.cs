using UnityEngine;

public class Match3Grid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public GameObject bubblePrefab;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float spacing = 1.1f;
                Vector2 position = new Vector2(x * spacing, y * spacing);

                Instantiate(bubblePrefab, position, Quaternion.identity);
            }
        }
    }
}
