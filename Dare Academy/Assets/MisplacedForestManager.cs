using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;

public class MisplacedForestManager : MonoBehaviour
{
    // Start is called before the first frame update

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
            switch (Random.Range(0, 4))
            {
                case 0:
                    App.GetModule<LevelModule>().persistantSceneData._direction = Vector2Int.down;

                    App.GetModule<LevelModule>().persistantSceneData._soundEmitter.transform.position = new Vector3(0, -App.GetModule<LevelModule>().CurrentRoom.Width / 2, 0);
                    break;

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
    }
}