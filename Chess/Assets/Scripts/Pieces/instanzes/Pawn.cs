using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    private static readonly Vector2Int Forward = new(0, 1);
    private static readonly Vector2Int[] CaptureDirections =
    {
        new(1, 1),
        new(-1, 1)
    };

    private int direction => isWhite ? 1 : -1;
    public bool firstMove = true;

    public override void Test()
    {
        Debug.Log($"Pawn @ {currentTile.x}, {currentTile.z}");
    }

    public override List<Tile> CalcValidMoves(Tile[,] grid)
    {
        List<Tile> moves = new();

        TryAddForwardMove(grid, moves);
        TryAddCaptureMoves(grid, moves);

        if (firstMove)
            TryAddDoubleStep(grid, moves);

        return moves;
    }

    void TryAddForwardMove(Tile[,] grid, List<Tile> moves)
    {
        var pos = GetTile(currentTile.x, currentTile.z + direction, grid);

        if (pos != null && pos.piece == null)
            moves.Add(pos);
    }

    void TryAddDoubleStep(Tile[,] grid, List<Tile> moves)
    {
        var oneStep = GetTile(currentTile.x, currentTile.z + direction, grid);
        var twoStep = GetTile(currentTile.x, currentTile.z + direction * 2, grid);

        if (oneStep?.piece == null && twoStep?.piece == null)
            moves.Add(twoStep);
    }

    void TryAddCaptureMoves(Tile[,] grid, List<Tile> moves)
    {
        foreach (var dir in CaptureDirections)
        {
            var tile = GetTile(currentTile.x + dir.x, currentTile.z + dir.y * direction, grid);

            if (tile?.piece == null) continue;

            var other = tile.piece.GetComponent<Piece>();
            if (other.isWhite != isWhite)
                moves.Add(tile);
        }
    }

    Tile GetTile(int x, int z, Tile[,] grid)
    {
        if (!IndexExists<Tile>(grid, x, z))
            return null;

        return grid[x, z];
    }
}