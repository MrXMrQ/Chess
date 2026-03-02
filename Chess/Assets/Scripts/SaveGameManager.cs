using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string prefabName;
    public bool isWhite;
    public int x, z;
}

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance;
    public List<SaveData> savedPieces = new List<SaveData>(); // Store data, not Tiles
    public bool flag = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}