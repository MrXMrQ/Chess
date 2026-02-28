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
            TryAddMove(grid, moves, offset);

        return moves;
    }

    void TryAddMove(Tile[,] grid, List<Tile> moves, Vector2Int offset)
    {
        int x = currentTile.x + offset.x;
        int z = currentTile.z + offset.y;

        if (!IndexExists<Tile>(grid, x, z))
            return;

        Tile tile = grid[x, z];

        if (tile.piece == null)
        {
            moves.Add(tile);
            return;
        }

        Piece other = tile.piece.GetComponent<Piece>();

        if (other.isWhite != isWhite)
            moves.Add(tile);
    }
}