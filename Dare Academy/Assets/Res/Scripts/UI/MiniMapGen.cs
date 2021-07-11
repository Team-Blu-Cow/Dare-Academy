using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using JUtil;

public class MiniMapGen : MonoBehaviour, IScrollHandler, IDragHandler, IBeginDragHandler
{
    private GridInfo[] m_gridInfo;
    private NodeOverrides<GridNode> m_links;
    private List<GameObject> m_squares = new List<GameObject>();
    private Vector2 m_movePos;
    private RectTransform transform;
    public float m_speed;

    private void Start()
    {
        transform = GetComponent<RectTransform>();
        DrawMap();
        App.GetModule<InputModule>().SystemController.MapControlls.Move.performed += ctx => MoveStart(ctx);
        App.GetModule<InputModule>().SystemController.MapControlls.Move.canceled += ctx => MoveEnd();
    }

    public void DrawMap()
    {
        m_gridInfo = App.GetModule<LevelModule>().MetaGrid.gridInfo;
        m_links = App.GetModule<LevelModule>().MetaGrid.nodeOverrides;
        JUtil.Grids.Grid<GridNode> currentRoom = App.GetModule<LevelModule>().CurrentRoom;

        foreach (GameObject go in m_squares)
        {
            Destroy(go);
        }
        m_squares.Clear();

        foreach (GridInfo grid in m_gridInfo)
        {
            // Draw and place the room box
            GameObject tempRoom = new GameObject("Room");
            tempRoom.AddComponent<Image>();
            RectTransform rect = tempRoom.GetComponent<RectTransform>();
            tempRoom.transform.SetParent(transform.GetChild(0));
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(grid.width, grid.height);
            rect.localPosition = grid.originPosition + (new Vector3(grid.width, grid.height, 0) / 2);

            m_squares.Add(tempRoom);
        }

        //Draw room connections
        foreach (GridLink link in m_links.gridLinks)
        {
            Vector3 linkStart = m_gridInfo[link.grid1.index].ToWorld(link.grid1.position);
            Vector3 linkEnd = m_gridInfo[link.grid2.index].ToWorld(link.grid2.position);

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

            m_squares.Add(tempLink);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 scale = (Vector3.one / 2) * eventData.scrollDelta.y;

        RectTransform rect = GetComponent<RectTransform>();

        //rect.anchoredPosition *= scale.x;
        transform.localScale += scale;
    }

    private Vector2 m_PointerStartLocalCursor;
    private Vector2 m_ContentStartPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_PointerStartLocalCursor = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
        m_ContentStartPosition = transform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        var pointerDelta = localCursor - m_PointerStartLocalCursor;
        Vector2 position = m_ContentStartPosition + pointerDelta * m_speed;

        transform.anchoredPosition = position;
    }

    private void MoveStart(InputAction.CallbackContext ctx)
    {
        m_movePos = ctx.ReadValue<Vector2>();
    }

    private void MoveEnd()
    {
        m_movePos = Vector2.zero;
    }

    private void Update()
    {
        if (m_movePos != Vector2.zero)
        {
            transform.GetChild(0).position += new Vector3(m_movePos.x, m_movePos.y, 0);
            transform.GetChild(1).position += new Vector3(m_movePos.x, m_movePos.y, 0);
        }
    }
}