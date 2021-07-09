using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using JUtil;

public class MiniMapGen : MonoBehaviour, IScrollHandler, IDragHandler
{
    private GridInfo[] gridInfo;
    private NodeOverrides<GridNode> links;
    private List<GameObject> squares = new List<GameObject>();
    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        DrawMap();
        App.GetModule<InputModule>().SystemController.MapControlls.Move.started += ctx => Move(ctx);
    }

    public void DrawMap()
    {
        gridInfo = App.GetModule<LevelModule>().MetaGrid.gridInfo;
        links = App.GetModule<LevelModule>().MetaGrid.nodeOverrides;
        JUtil.Grids.Grid<GridNode> currentRoom = App.GetModule<LevelModule>().CurrentRoom;

        foreach (GameObject go in squares)
        {
            Destroy(go);
        }
        squares.Clear();

        foreach (GridInfo grid in gridInfo)
        {
            // Draw and place the room box
            GameObject tempRoom = new GameObject("Room");
            tempRoom.AddComponent<Image>();
            RectTransform rect = tempRoom.GetComponent<RectTransform>();
            tempRoom.transform.SetParent(transform.GetChild(0));
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(grid.width, grid.height);
            rect.localPosition = grid.originPosition + (new Vector3(grid.width, grid.height, 0) / 2);

            squares.Add(tempRoom);
        }

        //Draw room connections
        foreach (GridLink link in links.gridLinks)
        {
            Vector3 linkStart = gridInfo[link.grid1.index].ToWorld(link.grid1.position);
            Vector3 linkEnd = gridInfo[link.grid2.index].ToWorld(link.grid2.position);

            float length = Vector3.Distance(linkStart, linkEnd);

            float angle = Mathf.Atan2(linkEnd.y - linkStart.y, linkEnd.x - linkStart.x);

            GameObject tempLink = new GameObject("Link");
            tempLink.AddComponent<Image>();
            RectTransform rect = tempLink.GetComponent<RectTransform>();
            tempLink.transform.SetParent(transform.GetChild(1));
            rect.sizeDelta = new Vector2(length, link.width);
            rect.localScale = Vector3.one;
            rect.localPosition = (linkStart + linkEnd) / 2;
            rect.rotation = Quaternion.Euler(0, 0, angle);

            squares.Add(tempLink);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 scale = (Vector3.one / 2) * eventData.scrollDelta.y;
        rect = GetComponent<RectTransform>();

        rect.anchoredPosition *= scale.x;
        transform.localScale += scale;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    private void Move(InputAction.CallbackContext ctx)
    {
    }
}