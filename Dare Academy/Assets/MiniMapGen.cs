using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MiniMapGen : MonoBehaviour
{
    private GridInfo[] gridInfo;
    private NodeOverrides<GridNode> links;
    [SerializeField] private GameObject room;
    private List<GameObject> squares = new List<GameObject>();

    private void Start()
    {
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
            GameObject tempRoom = Instantiate(room, transform);
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
            float angle = Vector3.Angle(linkStart, linkEnd);

            GameObject tempLink = Instantiate(room, transform);
            tempLink.GetComponent<RectTransform>().sizeDelta = new Vector2(length, 5);
            tempLink.GetComponent<RectTransform>().localPosition = (linkStart + linkEnd) / 2;
            tempLink.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angle);

            squares.Add(tempLink);
        }
    }
}