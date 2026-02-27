using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject tile;
    public Material[] materials;
    public PieceManager pieceManager;
    private Tile[,] grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();
        SpawnPieces();
    }

    void GenerateGrid()
    {
        grid = new Tile[width, height];
        Renderer rend = tile.GetComponent<Renderer>();

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {

                rend.material = materials[(x + z) % 2];


                GameObject newTile = Instantiate(tile, new Vector3(x, 0, z), Quaternion.identity);
                newTile.name = $"Tile {x} {z}";

                Tile tileScript = newTile.GetComponent<Tile>();
                tileScript.Setup(x, z);

                grid[x, z] = tileScript;
            }
        }
    }

    void SpawnPieces()
    {
        foreach (var piece in pieceManager.pieces)
        {
            foreach (var pos in piece.positions)
            {
                grid[pos.x, pos.y].Spawn(piece);
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        return grid[x, y];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
