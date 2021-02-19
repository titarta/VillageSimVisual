using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class farmManager : MonoBehaviour
{
    private int rows = 5;
    private int cols = 4;
    private float tileSize = 1;

    public Sprite harvested;
    public Sprite seeds;
    public Sprite plant;

    private List<List<GameObject>> farmCrops;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();

    }

    private void GenerateGrid()
    {
        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("cropSlot"));
        referenceTile.GetComponent<SpriteRenderer>().sprite = harvested;
        farmCrops = new List<List<GameObject>>();
        for (int row = 0; row < rows; row++)
        {
            farmCrops.Add(new List<GameObject>());
            for (int col = 0; col < cols; col++)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);

                float posX = col * tileSize + transform.position.x;
                float posY = row * -tileSize + transform.position.y;

                tile.transform.position = new Vector2(posX, posY);

                farmCrops[row].Add(tile);

            }
        }

        Destroy(referenceTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void plantSeed(int x, int y)
    {
        farmCrops[y][x].GetComponent<SpriteRenderer>().sprite = seeds;
    }

    public void harvest(int x, int y)
    {
        farmCrops[y][x].GetComponent<SpriteRenderer>().sprite = harvested;
    }

    public void grow(int x, int y)
    {
        farmCrops[y][x].GetComponent<SpriteRenderer>().sprite = plant;
    }
}
