using UnityEngine;

[System.Serializable]
public class PieceSpawnData
{
    public GameObject prefab;
    public Vector2Int[] positions;
    public bool isWhite;
    public Material material;
}
