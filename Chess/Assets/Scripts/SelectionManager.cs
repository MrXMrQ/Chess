using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class SelectionManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public Material[] mats;
    public Vector3 pos;
    public int offset;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    void Start()
    {
        foreach (var item in prefabs)
        {
            if (SaveGameManager.Instance.callerIsWhite)
            {
                item.GetComponent<Renderer>().material = mats[0];
            }
            else
            {
                item.GetComponent<Renderer>().material = mats[1];
            }

            Instantiate(item, pos, Quaternion.identity);
            pos = new Vector3(pos.x + offset, pos.y, pos.z);
        }
    }

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
                    selection.gameObject.GetComponent<Outline>().enabled = false;

                }
                selection = raycastHit.transform;
                selection.gameObject.GetComponent<Outline>().enabled = true;

                Piece piece = selection.gameObject.GetComponent<Piece>();

                if (piece != null)
                {
                    // Statt: SaveGameManager.Instance.selection = selection.gameObject;
                    SaveGameManager.Instance.selectionPrefabName = piece.gameObject.name
                        .Replace("(Clone)", "").Trim();
                    SaveGameManager.Instance.selectionIsWhite = SaveGameManager.Instance.callerIsWhite;

                    SceneManager.LoadScene("default");
                }

                highlight = null;
            }
            else
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection = null;
                }
            }
        }
    }
}
