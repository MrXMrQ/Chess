using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject piece; // The GameObject
    public Piece pieceScript; // Add this to track the logic/data
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

    public void Clear()
    {
        Destroy(piece);
        pieceScript = null;
    }

    public void Spawn(GameObject prefab, bool isWhite, Material mat)
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += y;

        GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity);
        this.piece = go;
        this.pieceScript = go.GetComponent<Piece>();

        if (pieceScript != null)
        {
            pieceScript.currentTile = this;
            pieceScript.isWhite = isWhite;
            go.GetComponentInChildren<Renderer>().material = mat;
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
