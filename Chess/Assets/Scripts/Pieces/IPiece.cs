public interface IPiece
{
    Tile[] calcMoves();

    void OnMouseDown();
    void OnMouseEnter();
    void OnMouseExit();
}