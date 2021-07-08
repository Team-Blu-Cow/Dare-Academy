using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.UI;
using JUtil;

public class MiniMapGen : MonoBehaviour
{
    private GridInfo[] gridInfo;
    private NodeOverrides<GridNode> links;
    private List<GameObject> squares = new List<GameObject>();

    private void Start()
    {
        DrawMap();
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
            tempRoom.transform.parent = transform;
            tempRoom.GetComponent<RectTransform>().localScale = Vector3.one;
            tempRoom.GetComponent<RectTransform>().sizeDelta = new Vector2(grid.width, grid.height);
            tempRoom.GetComponent<RectTransform>().localPosition = grid.originPosition + (new Vector3(grid.width, grid.height, 0) / 2);

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
            tempLink.transform.parent = transform;
            tempLink.GetComponent<RectTransform>().sizeDelta = new Vector2(length, link.width);
            tempLink.GetComponent<RectTransform>().localScale = Vector3.one;
            tempLink.GetComponent<RectTransform>().localPosition = (linkStart + linkEnd) / 2;
            tempLink.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

            squares.Add(tempLink);
        }
    }
}