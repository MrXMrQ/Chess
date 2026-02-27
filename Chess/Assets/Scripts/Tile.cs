using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject piece;
    public int x;
    public int y;
    public int z;

    public void Setup(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public void Spawn(PieceSpawnData piece)
    {
        piece.prefab.GetComponent<Renderer>().material = piece.material;

        GameObject gameObject = Instantiate(piece.prefab, new Vector3(x, y, z), Quaternion.identity);
        gameObject.name = $"{piece.prefab.name} {x} {z} {(piece.isWhite ? "white" : "black")}";
        this.piece = gameObject;
    }

    void OnMouseDown()
    {
        Debug.Log($"Tile {x} {z} clicked");

        if (piece != null)
        {
            Debug.Log(piece.name);
        }
        else
        {
            Debug.Log("No Piece on tile");
        }
    }
}
