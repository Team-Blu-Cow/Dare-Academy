using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using JUtil.Grids;
using JUtil;
using TMPro;

public class MiniMapGen : MonoBehaviour, IScrollHandler, IDragHandler, IBeginDragHandler
{
    private GridInfo[] m_gridInfo;
    private NodeOverrides<GridNode> m_links;

    private List<GameObject> m_squares = new List<GameObject>();
    private List<GameObject> m_quests = new List<GameObject>();
    private Vector2 m_movePos;
    [SerializeField] private GameObject m_toolTip;
    private PlayerEntity player;

    private bool open = false;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    private RectTransform transform;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    private float[] bounds = new float[4];

    private void Start()
    {
        App.GetModule<LevelModule>().StepController.RoomChangeEvent += DrawMap;
        transform = GetComponent<RectTransform>();
        player = FindObjectOfType<PlayerEntity>();
    }

    private void OnEnable()
    {
        App.GetModule<InputModule>().SystemController.MapControlls.Move.performed += MoveStart;
        App.GetModule<InputModule>().SystemController.MapControlls.Move.canceled += MoveEnd;
        App.GetModule<InputModule>().SystemController.MapControlls.Zoom.started += Zoom;
        App.GetModule<InputModule>().SystemController.UI.Map.performed += ToggleMap;
    }

    private void OnDisable()
    {
        App.GetModule<InputModule>().SystemController.MapControlls.Move.performed -= MoveStart;
        App.GetModule<InputModule>().SystemController.MapControlls.Move.canceled -= MoveEnd;
        App.GetModule<InputModule>().SystemController.MapControlls.Zoom.started -= Zoom;
        App.GetModule<InputModule>().SystemController.UI.Map.performed -= ToggleMap;
    }

    private void ToggleMap(InputAction.CallbackContext context)
    {
        if (open)
        {
            App.CanvasManager.GetCanvasContainer("Map").CloseCanvas();
            App.GetModule<InputModule>().PlayerController.Enable();
            App.GetModule<InputModule>().SystemController.MapControlls.Disable();
            open = false;
        }
        else
        {
            App.CanvasManager.OpenCanvas("Map");
            App.GetModule<InputModule>().PlayerController.Disable();
            App.GetModule<InputModule>().SystemController.MapControlls.Enable();
            transform.anchoredPosition = Vector2.zero;
            open = true;
        }
    }

    public void DrawMap()
    {
        m_gridInfo = App.GetModule<LevelModule>().MetaGrid.gridInfo;
        m_links = App.GetModule<LevelModule>().MetaGrid.nodeOverrides;
        Grid<GridNode> currentRoom = App.GetModule<LevelModule>().CurrentRoom;

        transform.anchoredPosition = Vector3.zero;
        CloseMap();

        DrawRooms(currentRoom);

        DrawLinks(currentRoom);

        DrawQuestMarker(currentRoom);

        DrawSceneLinks(currentRoom);

        FindBounds(currentRoom);
    }

    private void FindBounds(Grid<GridNode> currentRoom)
    {
        int i = 0;
        Camera cam = App.CameraController.GetComponent<Camera>();

        foreach (GridInfo grid in m_gridInfo)
        {
            Vector3 screenPos = grid.originPosition - currentRoom.OriginPosition;
            if (player.m_dictRoomsTraveled.ContainsKey(SceneManager.GetActiveScene().name) && player.m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(i))
            {
                //Check  x+
                if (screenPos.x < bounds[0])
                    bounds[0] = screenPos.x;
                //Check  x-
                if (screenPos.x + grid.width > bounds[1])
                    bounds[1] = screenPos.x + grid.width;
                //Check  y+
                if (screenPos.y < bounds[2])
                    bounds[2] = screenPos.y;
                //Check  y-
                if (screenPos.y + grid.height > bounds[3])
                    bounds[3] = screenPos.y + grid.height;
            }
            i++;
        }
    }

    private void DrawRooms(Grid<GridNode> currentRoom)
    {
        int currentRoomIndex = App.GetModule<LevelModule>().LevelManager.StepController.m_currentRoomIndex;

        int i = 0;
        foreach (GridInfo grid in m_gridInfo)
        {
            if (player.m_dictRoomsTraveled.ContainsKey(SceneManager.GetActiveScene().name) && player.m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(i))
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
    }

    private void DrawLinks(Grid<GridNode> currentRoom)
    {
        //Draw room connections

        foreach (GridLink link in m_links.gridLinks)
        {
            if (player.m_dictRoomsTraveled.ContainsKey(SceneManager.GetActiveScene().name) &&
                player.m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(link.grid1.index)
                && player.m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(link.grid2.index))
            {
                Vector3 linkStart = m_gridInfo[link.grid1.index].ToWorld(link.grid1.position);
                Vector3 linkEnd = m_gridInfo[link.grid2.index].ToWorld(link.grid2.position);

                // Length the link should be
                float length = Vector3.Distance(linkStart, linkEnd);

                // The angle at witch the link shall sit
                Vector2 linkDir = new Vector2(linkEnd.x - linkStart.x, linkEnd.y - linkStart.y);
                float angle = linkDir.GetRotation();

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
                float width = (link.width / 2.0f) - 0.5f;

                //rect.localPosition = OffsetLink(rect.localPosition, angle, width);
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

                    case 360:
                        rect.localPosition -= new Vector3(0, width, 0);
                        break;

                    default:
                        break;
                }

                m_squares.Add(tempLink);
            }
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
            if (quest.showMarker && quest.markerScene == SceneManager.GetActiveScene().name /*&& player.m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(quest.markerLocations[0])*/) // commeted is to show if room has been discovered or not
            {
                GridInfo grid = m_gridInfo[quest.markerLocations[0]];
                Vector2 pos = grid.originPosition + (new Vector3(grid.width, grid.height, 0) / 2) - currentRoom.OriginPosition;
                CreateQuestMarker(currentRoom, quest, pos);
            }
            else if (quest.showMarker)
            {
                foreach (LevelTransitionInformation sceneLink in m_links.sceneLinks)
                {
                    if (player.m_dictRoomsTraveled.ContainsKey(SceneManager.GetActiveScene().name) && player.m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(sceneLink.myRoomIndex))
                    {
                        if (sceneLink.targetSceneName == quest.markerScene)
                        {
                            Vector2 pos = m_gridInfo[sceneLink.myRoomIndex].ToWorld(sceneLink.myNodeIndex);

                            var angle = (sceneLink.travelDirection + 2) * 45;
                            float width = (sceneLink.width / 2.0f) - 0.5f;
                            pos = OffsetLink(pos, angle, width);

                            CreateQuestMarker(currentRoom, quest, pos);
                        }
                    }
                }
            }
        }
    }

    private static Vector2 OffsetLink(Vector2 pos, int angle, float width)
    {
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

        return pos;
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

    private void DrawSceneLinks(Grid<GridNode> currentRoom)
    {
        foreach (LevelTransitionInformation link in m_links.sceneLinks)
        {
            if (player.m_dictRoomsTraveled.ContainsKey(SceneManager.GetActiveScene().name) &&
                player.m_dictRoomsTraveled[SceneManager.GetActiveScene().name].Contains(link.myRoomIndex))
            {
                Vector2 pos = m_gridInfo[link.myRoomIndex].ToWorld(link.myNodeIndex) - currentRoom.OriginPosition;

                var angle = (link.travelDirection + 2) * 45;
                float width = link.width / 2.0f;
                width -= 0.5f;

                pos = OffsetLink(pos, angle, width);

                GameObject Go = new GameObject(link.targetSceneName);

                Image image = Go.AddComponent<Image>();
                image.color = Color.blue;

                Go.transform.SetParent(transform.GetChild(1));

                RectTransform rect = (RectTransform)Go.transform;
                rect.sizeDelta = Vector2.one * 3;
                rect.localScale = Vector3.one;
                rect.anchoredPosition = pos;

                GameObject text = new GameObject("Text");
                text.transform.parent = Go.transform;
                TextMeshProUGUI tmp = text.AddComponent<TextMeshProUGUI>();
                tmp.text = link.targetSceneName;
                tmp.fontSize = 16;
                tmp.alignment = TextAlignmentOptions.Top;
                RectTransform textRect = (RectTransform)text.transform;
                textRect.anchoredPosition = new Vector2(0, -5);

                m_squares.Add(Go);
            }
        }
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

    public void Zoom(InputAction.CallbackContext ctx)
    {
        Vector3 scale = Vector3.one * 3 * Mathf.Sign(ctx.ReadValue<float>());

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

        float scale = transform.localScale.x;

        if (position.x < Screen.width / 2 - (bounds[0] * scale) && position.x > -Screen.width / 2 - (bounds[1] * scale)
            && position.y < Screen.height / 2 - (bounds[2] * scale) && position.y > -Screen.height / 2 - (bounds[3] * scale))
            transform.anchoredPosition = position;
    }

    private void MoveStart(InputAction.CallbackContext ctx)
    {
        m_movePos = ctx.ReadValue<Vector2>();
    }

    private void MoveEnd(InputAction.CallbackContext ctx)
    {
        m_movePos = Vector2.zero;
    }

    private void Update()
    {
        if (m_movePos != Vector2.zero)
        {
            Vector2 moveAmount = m_movePos * (5 * Mathf.Log10(transform.localScale.x));
            Vector2 newPos = transform.anchoredPosition + moveAmount;
            float scale = transform.localScale.x;

            if (newPos.x < Screen.width / 2 - (bounds[0] * scale) && newPos.x > -Screen.width / 2 - (bounds[1] * scale)
                && newPos.y < Screen.height / 2 - (bounds[2] * scale) && newPos.y > -Screen.height / 2 - (bounds[3] * scale))
                transform.anchoredPosition = newPos;
        }
    }
}