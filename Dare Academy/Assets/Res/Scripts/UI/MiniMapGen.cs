using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using JUtil.Grids;

public class MiniMapGen : MonoBehaviour, IScrollHandler, IDragHandler, IBeginDragHandler
{
    private GridInfo[] m_gridInfo;
    private NodeOverrides<GridNode> m_links;

    private List<GameObject> m_squares = new List<GameObject>();
    private List<GameObject> m_quests = new List<GameObject>();
    private Vector2 m_movePos;
    [SerializeField] private GameObject m_toolTip;

    private bool open = false;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private RectTransform transform;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    private void Start()
    {
        App.GetModule<QuestModule>().AddQuest(Resources.Load<Quest>("Quests/TestQuest"));
        App.GetModule<QuestModule>().AddQuest(Resources.Load<Quest>("Quests/TestQuest"));
        App.GetModule<QuestModule>().AddQuest(Resources.Load<Quest>("Quests/TestQuest"));

        transform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        App.GetModule<InputModule>().SystemController.MapControlls.Move.performed += ctx => MoveStart(ctx);
        App.GetModule<InputModule>().SystemController.MapControlls.Move.canceled += ctx => MoveEnd();
        App.GetModule<InputModule>().SystemController.MapControlls.ZoomIn.started += _ => Zoom(true);
        App.GetModule<InputModule>().SystemController.MapControlls.ZoomOut.started += _ => Zoom(false);
        App.GetModule<InputModule>().SystemController.MapControlls.Open.performed += _ => ToggleMap();
    }

    private void OnDisable()
    {
        App.GetModule<InputModule>().SystemController.MapControlls.Move.performed -= ctx => MoveStart(ctx);
        App.GetModule<InputModule>().SystemController.MapControlls.Move.canceled -= ctx => MoveEnd();
        App.GetModule<InputModule>().SystemController.MapControlls.ZoomIn.started -= _ => Zoom(true);
        App.GetModule<InputModule>().SystemController.MapControlls.ZoomOut.started -= _ => Zoom(false);
        App.GetModule<InputModule>().SystemController.MapControlls.Open.performed -= _ => ToggleMap();
    }

    private void ToggleMap()
    {
        if (open)
        {
            CloseMap();
            App.GetModule<InputModule>().PlayerController.Enable();
            open = false;
        }
        else
        {
            DrawMap();
            App.GetModule<InputModule>().PlayerController.Disable();
            open = true;
        }
    }

    public void DrawMap()
    {
        m_gridInfo = App.GetModule<LevelModule>().MetaGrid.gridInfo;
        m_links = App.GetModule<LevelModule>().MetaGrid.nodeOverrides;
        Grid<GridNode> currentRoom = App.GetModule<LevelModule>().CurrentRoom;
        int currentRoomIndex = App.GetModule<LevelModule>().LevelManager.StepController.m_currentRoomIndex;

        transform.anchoredPosition = Vector3.zero;
        CloseMap();

        DrawRooms(currentRoom, currentRoomIndex);

        DrawLinks(currentRoom);

        DrawQuestMarker(currentRoom);
    }

    private void DrawRooms(Grid<GridNode> currentRoom, int currentRoomIndex)
    {
        int i = 0;
        foreach (GridInfo grid in m_gridInfo)
        {
            // Draw and place the room box
            GameObject tempRoom = new GameObject("Room");
            Image image = tempRoom.AddComponent<Image>();
            RectTransform rect = tempRoom.GetComponent<RectTransform>();
            tempRoom.transform.SetParent(transform.GetChild(0));
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(grid.width, grid.height);
            rect.localPosition = grid.originPosition + (new Vector3(grid.width, grid.height, 0) / 2) - currentRoom.OriginPosition;

            if (i == currentRoomIndex)
            {
                image.color = Color.red;
            }

            m_squares.Add(tempRoom);
            i++;
        }
    }

    private void DrawLinks(Grid<GridNode> currentRoom)
    {
        //Draw room connections
        foreach (GridLink link in m_links.gridLinks)
        {
            Vector3 linkStart = m_gridInfo[link.grid1.index].ToWorld(link.grid1.position);
            Vector3 linkEnd = m_gridInfo[link.grid2.index].ToWorld(link.grid2.position);

            // Length the link should be
            float length = Vector3.Distance(linkStart, linkEnd);

            // The angle at witch the link shall sit
            float angle = Mathf.Atan2(linkEnd.y - linkStart.y, linkEnd.x - linkStart.x);
            if (angle < 5 && angle > -5)
                angle = 0;

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
            float width = link.width / 2.0f;
            width -= 0.5f;

            switch (angle)
            {
                case 0:
                    rect.localPosition -= new Vector3(0, width, 0);
                    break;

                case 45:
                    rect.localPosition += new Vector3(width, -width, 0);
                    break;

                case 90:
                    rect.localPosition += new Vector3(width, 0, 0);
                    break;

                case 135:
                    rect.localPosition += new Vector3(width, width, 0);
                    break;

                case 180:
                    rect.localPosition += new Vector3(0, width, 0);
                    break;

                case 225:
                    rect.localPosition += new Vector3(-width, width, 0);
                    break;

                case 270:
                    rect.localPosition -= new Vector3(width, 0, 0);
                    break;

                case 315:
                    rect.localPosition += new Vector3(-width, width, 0);
                    break;

                default:
                    break;
            }

            m_squares.Add(tempLink);
        }
    }

    public void DrawQuestMarker(Grid<GridNode> currentRoom)
    {
        foreach (GameObject go in m_quests)
        {
            Destroy(go);
        }
        m_quests.Clear();

        foreach (Quest quest in App.GetModule<QuestModule>().ActiveQuests)
        {
            if (quest.showMarker && quest.markerScene == SceneManager.GetActiveScene().name)
            {
                GridInfo grid = m_gridInfo[quest.markerLocations[0]];
                Vector2 pos = grid.originPosition + (new Vector3(grid.width, grid.height, 0) / 2) - currentRoom.OriginPosition;
                CreateQuestMarker(currentRoom, quest, pos);
            }
            else if (quest.showMarker)
            {
                foreach (LevelTransitionInformation sceneLink in m_links.sceneLinks)
                {
                    if (sceneLink.targetSceneName == quest.markerScene)
                    {
                        Vector2 pos = m_gridInfo[sceneLink.myRoomIndex].ToWorld(sceneLink.myNodeIndex);

                        var angle = (sceneLink.travelDirection + 2) * 45;
                        float width = sceneLink.width / 2.0f;
                        width -= 0.5f;

                        switch (angle)
                        {
                            case 0:
                                pos -= new Vector2(0, width);
                                break;

                            case 45:
                                pos += new Vector2(width, -width);
                                break;

                            case 90:
                                pos += new Vector2(width, 0);
                                break;

                            case 135:
                                pos += new Vector2(width, width);
                                break;

                            case 180:
                                pos += new Vector2(0, width);
                                break;

                            case 225:
                                pos += new Vector2(-width, width);
                                break;

                            case 270:
                                pos -= new Vector2(width, 0);
                                break;

                            case 315:
                                pos += new Vector2(-width, width);
                                break;

                            default:
                                break;
                        }
                        CreateQuestMarker(currentRoom, quest, pos);
                    }
                }
            }
        }
    }

    private void CreateQuestMarker(Grid<GridNode> currentRoom, Quest quest, Vector2 position)
    {
        GameObject Go = new GameObject(quest.name);
        Image image = Go.AddComponent<Image>();
        image.color = Color.green;

        Go.transform.SetParent(transform.GetChild(2));

        RectTransform rect = (RectTransform)Go.transform;
        rect.sizeDelta = Vector2.one * 3;
        rect.localScale = Vector3.one;

        if (quest.markerLocations != null)
        {
            rect.anchoredPosition = position;
        }
        else
        {
            Debug.LogWarning("Attemping to show quest marker without location set");
        }

        ToolTip tooltip = Go.AddComponent<ToolTip>();
        tooltip.m_toolTip = m_toolTip;
        tooltip.m_questName = quest.name;
        tooltip.m_questContent = quest.activeDescription;

        m_quests.Add(Go);
    }

    public void CloseMap()
    {
        foreach (GameObject go in m_squares)
        {
            Destroy(go);
        }
        m_squares.Clear();

        foreach (GameObject go in m_quests)
        {
            Destroy(go);
        }
        m_quests.Clear();
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