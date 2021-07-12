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

    private void Start()
    {
        transform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        App.GetModule<InputModule>().SystemController.MapControlls.Move.performed += ctx => MoveStart(ctx);
        App.GetModule<InputModule>().SystemController.MapControlls.Move.canceled += ctx => MoveEnd();
        App.GetModule<InputModule>().SystemController.MapControlls.ZoomIn.started += _ => Zoom(true);
        App.GetModule<InputModule>().SystemController.MapControlls.ZoomOut.started += _ => Zoom(false);
    }

    private void OnDisable()
    {
        App.GetModule<InputModule>().SystemController.MapControlls.Move.performed -= ctx => MoveStart(ctx);
        App.GetModule<InputModule>().SystemController.MapControlls.Move.canceled -= ctx => MoveEnd();
        App.GetModule<InputModule>().SystemController.MapControlls.ZoomIn.started -= _ => Zoom(true);
        App.GetModule<InputModule>().SystemController.MapControlls.ZoomOut.started -= _ => Zoom(false);
    }

    public void DrawMap()
    {
        m_gridInfo = App.GetModule<LevelModule>().MetaGrid.gridInfo;
        m_links = App.GetModule<LevelModule>().MetaGrid.nodeOverrides;
        JUtil.Grids.Grid<GridNode> currentRoom = App.GetModule<LevelModule>().CurrentRoom;

        transform.anchoredPosition = Vector3.zero;

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
            rect.localPosition = grid.originPosition + (new Vector3(grid.width, grid.height, 0) / 2) - currentRoom.OriginPosition;

            m_squares.Add(tempRoom);
        }

        //Draw room connections
        foreach (GridLink link in m_links.gridLinks)
        {
            Vector3 linkStart = m_gridInfo[link.grid1.index].ToWorld(link.grid1.position);
            Vector3 linkEnd = m_gridInfo[link.grid2.index].ToWorld(link.grid2.position);

            // Length the link should be
            float length = Vector3.Distance(linkStart, linkEnd);

            // The angle at witch the link shall sit
            float angle = Mathf.Atan2(linkEnd.y - linkStart.y, linkEnd.x - linkStart.x);

            // Spawning the square
            GameObject tempLink = new GameObject("Link");
            tempLink.AddComponent<Image>();
            RectTransform rect = tempLink.GetComponent<RectTransform>();
            tempLink.transform.SetParent(transform.GetChild(1));

            // Seting the size and position
            rect.sizeDelta = new Vector2(link.width, length);
            rect.localScale = Vector3.one;
            rect.localPosition = ((linkStart + linkEnd) / 2) - currentRoom.OriginPosition;
            rect.rotation = Quaternion.Euler(0, 0, angle);

            // offset link to be in the centre
            angle = (link.grid1.direction + 2) * 45;

            switch (angle)
            {
                case 0:
                    rect.localPosition -= new Vector3(0, link.width / 2, 0);
                    break;

                case 45:
                    rect.localPosition += new Vector3(link.width / 2, -link.width / 2, 0);
                    break;

                case 90:
                    rect.localPosition += new Vector3(link.width / 2, 0, 0);
                    break;

                case 135:
                    rect.localPosition += new Vector3(link.width / 2, link.width / 2, 0);
                    break;

                case 180:
                    rect.localPosition += new Vector3(0, link.width / 2, 0);
                    break;

                case 225:
                    rect.localPosition += new Vector3(-link.width / 2, link.width / 2, 0);
                    break;

                case 270:
                    rect.localPosition -= new Vector3(link.width / 2, 0, 0);
                    break;

                case 315:
                    rect.localPosition += new Vector3(-link.width / 2, link.width / 2, 0);
                    break;

                default:
                    break;
            }

            m_squares.Add(tempLink);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 scale = (Vector3.one / 2) * eventData.scrollDelta.y;

        if (transform.localScale.x + scale.x > 5 && transform.localScale.x + scale.x < 30)
            transform.localScale += scale;
    }

    public void Zoom(bool pos)
    {
        Vector3 scale = Vector3.one * 3;
        if (!pos)
            scale *= -1;

        if (transform.localScale.x + scale.x > 5 && transform.localScale.x + scale.x < 30)
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
        Vector2 position = m_ContentStartPosition + pointerDelta * (5 * Mathf.Log10(transform.localScale.x));

        if (position.x < Screen.width / 2 - 25 && position.x > -Screen.width / 2 + 25
            && position.y < Screen.height / 2 - 25 && position.y > -Screen.height / 2 + 25)
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
            Vector2 moveAmount = m_movePos * (5 * Mathf.Log10(transform.localScale.x));
            Vector2 newPos = transform.anchoredPosition + moveAmount;

            if (newPos.x < Screen.width / 2 - 25 && newPos.x > -Screen.width / 2 + 25
                && newPos.y < Screen.height / 2 - 25 && newPos.y > -Screen.height / 2 + 25)
                transform.anchoredPosition = newPos;
        }
    }
}