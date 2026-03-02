using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject tile;
    public Material[] materials;
    public PieceManager pieceManager;
    private List<Tile> validMoves = new List<Tile>();
    private Tile[,] grid;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();

        if (SaveGameManager.Instance != null && SaveGameManager.Instance.flag)
        {
            LoadGame();
            Replace();
        }
        else
        {
            SpawnPieces();
        }
    }

    // GridManager.Replace():
    void Replace()
    {
        int x = SaveGameManager.Instance.pos.x;
        int z = SaveGameManager.Instance.pos.y;
        grid[x, z].Clear();

        foreach (var pData in pieceManager.pieces)
        {
            if (pData.prefab.name == SaveGameManager.Instance.selectionPrefabName
                && pData.isWhite == SaveGameManager.Instance.selectionIsWhite)
            {
                grid[x, z].Spawn(pData.prefab, pData.isWhite, pData.material);
                break;
            }
        }
    }

    void GenerateGrid()
    {
        grid = new Tile[width, height];
        Renderer rend = tile.GetComponent<Renderer>();
        Vector3 tileSize = tile.GetComponent<Renderer>().bounds.size;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                rend.material = materials[(x + z) % 2];

                float posX = x * tileSize.x;
                float posZ = z * tileSize.z;
                Vector3 spawnPos = new Vector3(posX, 0, posZ);


                GameObject newTile = Instantiate(tile, spawnPos, Quaternion.identity);
                newTile.name = $"Tile {x} {z}";

                Tile tileScript = newTile.GetComponent<Tile>();
                tileScript.Setup(x, z);

                grid[x, z] = tileScript;
            }
        }
    }

    void SpawnPieces()
    {
        foreach (var piece in pieceManager.pieces)
        {
            foreach (var pos in piece.positions)
            {
                grid[pos.x, pos.y].Spawn(piece.prefab, piece.isWhite, piece.material);
            }
        }
    }

    void MarkValidMoves()
    {
        foreach (var item in validMoves)
        {
            if (item != null)
            {
                item.Mark();
            }
        }
    }

    void UnmarkValidMoves()
    {
        foreach (var item in validMoves)
        {
            if (item != null)
            {
                item.Unmark();
            }
        }
    }

    //MIT License
    //Copyright (c) 2023 DA LAB (https://www.youtube.com/@DA-LAB)
    //Permission is hereby granted, free of charge, to any person obtaining a copy
    //of this software and associated documentation files (the "Software"), to deal
    //in the Software without restriction, including without limitation the rights
    //to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    //copies of the Software, and to permit persons to whom the Software is
    //furnished to do so, subject to the following conditions:
    //The above copyright notice and this permission notice shall be included in all
    //copies or substantial portions of the Software.
    //THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    //IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    //FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    //AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    //LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    //OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    //SOFTWARE.

    // Update is called once per frame
    void Update()
    {
        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.magenta;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                }
            }
            else
            {
                highlight = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0))
        {
            if (highlight)
            {
                if (selection != null)
                {
                    UnmarkValidMoves();
                    selection.gameObject.GetComponent<Outline>().enabled = false;

                }
                selection = raycastHit.transform;
                selection.gameObject.GetComponent<Outline>().enabled = true;

                Piece piece = selection.gameObject.GetComponent<Piece>();

                if (piece != null)
                {
                    validMoves = piece.CalcValidMoves(grid);
                    MarkValidMoves();
                }

                highlight = null;
            }
            else
            {
                if (selection)
                {
                    UnmarkValidMoves();
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection = null;
                }
            }
        }
    }

    public bool HasSelection()
    {
        return selection != null;
    }

    public void MoveSelectedPieceTo(Tile targetTile, int y)
    {
        Piece piece = selection.GetComponent<Piece>();

        if (piece == null) return;

        UnmarkValidMoves();
        piece.firstMove = false;
        piece.MoveTo(targetTile, y);

        if (piece is Pawn)
        {
            if (piece.currentTile.z == 0 || piece.currentTile.z == height - 1)
            {
                Pawn pawn = (Pawn)piece;
                SaveGameManager.Instance.pos = new Vector2Int(pawn.currentTile.x, pawn.currentTile.z);
                SaveGameManager.Instance.callerIsWhite = pawn.isWhite;
                SaveGame();
                pawn.Promotion();
            }
        }

        selection.GetComponent<Outline>().enabled = false;
        selection = null;
    }

    public void SaveGame()
    {
        SaveGameManager.Instance.savedPieces.Clear();
        foreach (Tile t in grid)
        {
            if (t.pieceScript != null)
            {
                SaveData data = new SaveData();
                data.x = t.x;
                data.z = t.z;
                data.isWhite = t.pieceScript.isWhite;

                string cleanName = t.pieceScript.gameObject.name;
                int cloneIndex = cleanName.IndexOf("(Clone)");
                if (cloneIndex > 0)
                {
                    cleanName = cleanName.Substring(0, cloneIndex).Trim();
                }

                data.prefabName = cleanName;
                SaveGameManager.Instance.savedPieces.Add(data);
            }
        }
        SaveGameManager.Instance.flag = true;
    }

    private void LoadGame()
    {
        foreach (var data in SaveGameManager.Instance.savedPieces)
        {
            bool found = false;
            foreach (var pData in pieceManager.pieces)
            {
                if (pData.prefab.name == data.prefabName && pData.isWhite == data.isWhite)
                {
                    grid[data.x, data.z].Spawn(pData.prefab, pData.isWhite, pData.material);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Debug.LogWarning("Cant find " + data.prefabName);
            }
        }
    }
}
