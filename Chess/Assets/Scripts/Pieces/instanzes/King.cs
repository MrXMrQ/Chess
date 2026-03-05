using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    private static readonly Vector2Int[] MoveOffsets =
    {
        new( 1,  0),
        new( -1,  0),
        new(0,  1),
        new(0,  -1),
        new( 1, 1),
        new( 1, -1),
        new(-1, 1),
        new(-1, -1),
    };

    public override List<Tile> CalcValidMoves(Tile[,] grid)
    {
        List<Tile> moves = new();
        foreach (var offset in MoveOffsets)
        {
            TryAddMove(grid, moves, offset);
        }
        return moves;
    }

    void TryAddMove(Tile[,] grid, List<Tile> moves, Vector2Int offset)
    {
        int x = currentTile.x + offset.x;
        int z = currentTile.z + offset.y;
        if (!IndexExists<Tile>(grid, x, z)) return;

        Tile tile = grid[x, z];

        if (tile.piece != null)
        {
            Piece other = tile.piece.GetComponent<Piece>();
            if (other.isWhite == isWhite) return;
        }

        GameObject originalPiece = tile.piece;
        Piece originalScript = tile.pieceScript;

        tile.piece = currentTile.piece;
        tile.pieceScript = currentTile.pieceScript;
        currentTile.piece = null;
        currentTile.pieceScript = null;

        Tile savedTile = currentTile;
        currentTile = tile;

        bool isSafe = !IsTileAttackedByEnemy(tile, grid);

        currentTile = savedTile;
        tile.piece = originalPiece;
        tile.pieceScript = originalScript;

        currentTile.piece = gameObject;
        currentTile.pieceScript = this;

        if (isSafe) moves.Add(tile);
    }

    bool IsTileAttackedByEnemy(Tile target, Tile[,] grid)
    {
        foreach (Tile t in grid)
        {
            if (t.pieceScript == null) continue;
            if (t.pieceScript.isWhite == isWhite) continue;
            if (t.pieceScript is King) // gegnerischen König einfach per Offset prüfen
            {
                King enemyKing = (King)t.pieceScript;
                foreach (var offset in MoveOffsets)
                {
                    int ex = t.x + offset.x;
                    int ez = t.z + offset.y;
                    if (ex == target.x && ez == target.z) return true;
                }
                continue;
            }

            List<Tile> enemyMoves = t.pieceScript.CalcValidMoves(grid);
            if (enemyMoves.Contains(target)) return true;
        }
        return false;
    }
}