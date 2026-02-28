using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    private static readonly Vector2Int[] Directions =
    {
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1),
        new(1, 1),
        new(-1, 1),
        new(1, -1),
        new(-1, -1)
    };

    public override List<Tile> CalcValidMoves(Tile[,] grid)
    {
        List<Tile> moves = new();

        foreach (var dir in Directions)
        {
            AddMovesInDirection(grid, moves, dir);
        }

        return moves;
    }

    void AddMovesInDirection(Tile[,] grid, List<Tile> moves, Vector2Int dir)
    {
        int x = currentTile.x;
        int z = currentTile.z;

        while (true)
        {
            x += dir.x;
            z += dir.y;

            if (!IndexExists<Tile>(grid, x, z))
            {
                break;
            }


            Tile tile = grid[x, z];

            if (tile.piece == null)
            {
                moves.Add(tile);
                continue;
            }

            Piece other = tile.piece.GetComponent<Piece>();

            if (other.isWhite != isWhite)
            {
                moves.Add(tile);
            }

            break;
        }
    }
}