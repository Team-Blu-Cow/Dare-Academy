using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MiniMapGen : MonoBehaviour
{
    private GridInfo[] gridInfo;
    private NodeOverrides<GridNode> links;
    [SerializeField] private GameObject room;

    private void Start()
    {
        gridInfo = App.GetModule<LevelModule>().MetaGrid.gridInfo;
        links = App.GetModule<LevelModule>().MetaGrid.nodeOverrides;
        JUtil.Grids.Grid<GridNode> currentRoom = App.GetModule<LevelModule>().CurrentRoom;

        foreach (GridInfo grid in gridInfo)
        {
            // Draw and place the room box
            GameObject tempRoom = Instantiate(room, transform);
            tempRoom.GetComponent<RectTransform>().sizeDelta = new Vector2(grid.width, grid.height);
            tempRoom.GetComponent<RectTransform>().localPosition = grid.originPosition - currentRoom.OriginPosition;

            //Draw room connections
            foreach (GridLink link in links.gridLinks)
            {
            }
        }
    }
}