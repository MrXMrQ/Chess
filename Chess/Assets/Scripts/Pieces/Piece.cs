using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Tile currentTile;
    public bool isWhite = false;
    public Vector2Int[] moveDirection = { };

    public virtual void Test()
    {
        Debug.Log("Piece");
    }

    public virtual List<Tile> CalcValidMoves(Tile[,] grid)
    {
        return new List<Tile>();
    }

    public void MoveTo(Tile targetTile, int y)
    {
        if (targetTile.piece != null)
        {
            //Take piece action
            Destroy(targetTile.piece);
        }

        currentTile.piece = null;

        currentTile = targetTile;
        targetTile.piece = gameObject;

        Vector3 newPos = targetTile.transform.position;
        newPos.y += y; // same hight
        transform.position = newPos;


    }

    protected bool IndexExists<T>(T[,] array, int i, int j)
    {
        return i >= 0 && i < array.GetLength(0) &&
               j >= 0 && j < array.GetLength(1);
    }

    protected Tile GetTile(int x, int z, Tile[,] grid)
    {
        if (!IndexExists<Tile>(grid, x, z))
        {
            return null;
        }

        return grid[x, z];
    }
}