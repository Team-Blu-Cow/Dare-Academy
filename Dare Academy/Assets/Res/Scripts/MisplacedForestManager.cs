using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using UnityEngine.Tilemaps;

public class MisplacedForestManager : MonoBehaviour
{
    [SerializeField] private GameObject[] m_tilesets;
    [SerializeField] private Tilemap[] m_Collisiontilesets;

    private void Awake()
    {
        LevelManager lvlManager = FindObjectOfType<LevelManager>();
        List<Tilemap> collisionMap = lvlManager.Grid.tileData.tilemaps;
        //collisionMap.Clear();

        collisionMap.Add(m_Collisiontilesets[App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter]);

        for(int i = 0; i < 4; i++)
        {
            if(m_tilesets[i] != null)
                m_tilesets[i].SetActive((i == App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter)? true : false);
        }
        
    }

    private void Start()
    {
        App.GetModule<LevelModule>().persistantSceneData._soundEmitter = FindObjectOfType<MisplacedForestHint>().gameObject;
        App.GetModule<LevelModule>().persistantSceneData._switching = false;

        if (App.GetModule<LevelModule>().persistantSceneData._MisplacedForestCounter == 0)
        {
            switch (Random.Range(1, 4))
            {
                case 1:
                    App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.left;
                    App.GetModule<LevelModule>().persistantSceneData._soundEmitter.transform.position = new Vector3(App.GetModule<LevelModule>().CurrentRoom.OriginPosition.x, 0, 0);
                    break;

                case 2:
                    App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.up;

                    App.GetModule<LevelModule>().persistantSceneData._soundEmitter.transform.position = new Vector3(0, App.GetModule<LevelModule>().CurrentRoom.Height / 2, 0);
                    break;

                case 3:
                    App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.right;

                    App.GetModule<LevelModule>().persistantSceneData._soundEmitter.transform.position = new Vector3(App.GetModule<LevelModule>().CurrentRoom.Width / 2, 0, 0);
                    break;

                default:
                    App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.zero;
                    Debug.LogError("Invalid Misplaced Woods start direction");
                    break;
            }
        }
        else
        {
            bool validDirection = false;
            do
                switch (Random.Range(0, 4))
                {
                    case 0:
                        if (App.GetModule<LevelModule>().persistantSceneData._direction == Vector2Int.up)
                            break;

                        App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.down;

                        App.GetModule<LevelModule>().persistantSceneData._soundEmitter.transform.position = new Vector3(0, -App.GetModule<LevelModule>().CurrentRoom.Width / 2, 0);
                        validDirection = true;
                        break;

                    case 1:
                        if (App.GetModule<LevelModule>().persistantSceneData._direction == Vector2Int.right)
                            break;
                        App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.left;

                        App.GetModule<LevelModule>().persistantSceneData._soundEmitter.transform.position = new Vector3(App.GetModule<LevelModule>().CurrentRoom.OriginPosition.x, 0, 0);
                        validDirection = true;
                        break;

                    case 2:
                        if (App.GetModule<LevelModule>().persistantSceneData._direction == Vector2Int.down)
                            break;
                        App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.up;

                        App.GetModule<LevelModule>().persistantSceneData._soundEmitter.transform.position = new Vector3(0, App.GetModule<LevelModule>().CurrentRoom.Height / 2, 0);
                        validDirection = true;
                        break;

                    case 3:
                        if (App.GetModule<LevelModule>().persistantSceneData._direction == Vector2Int.left)
                            break;
                        App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.right;

                        App.GetModule<LevelModule>().persistantSceneData._soundEmitter.transform.position = new Vector3(App.GetModule<LevelModule>().CurrentRoom.Width / 2, 0, 0);
                        validDirection = true;
                        break;

                    default:
                        App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.zero;
                        Debug.LogError("Invalid Misplaced Woods start direction");
                        break;
                }
            while (!validDirection);
        }
    }
}