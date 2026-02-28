using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject piece;
    public GameObject marker;
    private GameObject markerInstance;
    private bool isMarked = false;
    public int x;
    public int y;
    public int z;

    public void Setup(int x, int z)
    {
        this.x = x;
        this.z = z;
        y = 2;
    }

    public void Spawn(PieceSpawnData piece)
    {
        piece.prefab.GetComponent<Renderer>().material = piece.material;

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += y;

        GameObject gameObject = Instantiate(piece.prefab, spawnPosition, Quaternion.identity);
        gameObject.name = $"{piece.prefab.name} {x} {z} {(piece.isWhite ? "white" : "black")}";
        this.piece = gameObject;

        Piece pieceSkript = gameObject.GetComponent<Piece>();
        if (pieceSkript != null)
        {
            pieceSkript.currentTile = this;
            if (piece.isWhite)
            {
                pieceSkript.isWhite = true;
            }
        }
    }

    public void Mark()
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += y;

        markerInstance = Instantiate(marker, spawnPosition, Quaternion.identity);
        markerInstance.name = $"{marker.name} {x} {z}";

        isMarked = true;
    }

    public void Unmark()
    {
        Destroy(markerInstance);
        isMarked = false;
    }

    void OnMouseDown()
    {
        GridManager gm = FindFirstObjectByType<GridManager>();

        if (gm == null) return;

        if (gm.HasSelection() && isMarked)
        {
            gm.MoveSelectedPieceTo(this, y);
        }
    }
}
