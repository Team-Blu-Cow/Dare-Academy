using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using blu;
using JUtil.Grids;

namespace blu
{
    public class CamContoller : MonoBehaviour
    {
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _virtualCam;
        [SerializeField] private Camera _Cam;

        private PlayerEntity _player;
        private bool _keepPlayerInFrame = true;
        private Grid<GridNode> _currentRoom;
        [SerializeField] [Range(0, 10)] private float _tolerance = 1.5f;

        private void Start()
        {
            _player = FindObjectOfType<PlayerEntity>();
            if (_player != null)
                GetComponent<FMODUnity.StudioListener>().attenuationObject = _player.gameObject;
            else
                Debug.LogWarning("[CameraController]: could not find player");

            _Cam = GetComponent<Camera>();
            App.GetModule<LevelModule>().StepController.RoomChangeEvent += MoveToCurrentRoom;
            MoveToCurrentRoom();
        }

        private void OnDestroy()
        {
            App.GetModule<LevelModule>().StepController.RoomChangeEvent -= MoveToCurrentRoom;
        }

        public void KeepPlayerInFrame(bool in_bool = true)
        {
            _keepPlayerInFrame = in_bool;
        }

        public void ToggleKeepPlayerInFrame()
        {
            _keepPlayerInFrame = !_keepPlayerInFrame;
        }

        public void MoveToCurrentRoom()
        {
            Vector3 TargetPosition;
            Grid<GridNode> TargetGrid = App.GetModule<LevelModule>().CurrentRoom;
            _tolerance = App.GetModule<LevelModule>().MetaGrid.gridInfo[App.GetModule<LevelModule>().StepController.m_currentRoomIndex].cameraPadding;
            TargetPosition = TargetGrid.OriginPosition;
            TargetPosition.y += TargetGrid.Height / 2;
            TargetPosition.x += TargetGrid.Width / 2;

            Move(TargetPosition);
        }

        public void MoveToRoomByIndex(int in_index)
        {
            Vector3 TargetPosition;
            Grid<GridNode> TargetGrid = App.GetModule<LevelModule>().Grid(in_index);
            TargetPosition = TargetGrid.OriginPosition;
            TargetPosition.y += TargetGrid.Height / 2;
            TargetPosition.x += TargetGrid.Width / 2;
            Move(TargetPosition);
        }

        public void MoveToPosition(Vector3 in_position)
        {
            Move(in_position);
        }

        public void MoveToPosition(Transform in_transform)
        {
            Move(in_transform.position);
        }

        private void Move(Vector3 in_to)
        {
            _virtualCam.Follow.position = in_to;
        }

        private void Update()
        {
            if (_keepPlayerInFrame)
            {
                //NOTE: if camera is acting weird, very likely due to the fact
                // that it assumes all grid cells are the size of 1.
                //
                // shout at @adam (or @matthew if youre feeling spicy) to fix it

                //_virtualCam.Follow.position = _player.transform.position;
                _virtualCam.Follow.position = new Vector3(
                    Mathf.Clamp(_player.transform.position.x,
                    App.GetModule<LevelModule>().CurrentRoom.OriginPosition.x + (_Cam.OrthographicBounds().extents.x - _tolerance),
                    App.GetModule<LevelModule>().CurrentRoom.OriginPosition.x + App.GetModule<LevelModule>().CurrentRoom.Width - (_Cam.OrthographicBounds().extents.x) + _tolerance),

                    Mathf.Clamp(_player.transform.position.y,
                    App.GetModule<LevelModule>().CurrentRoom.OriginPosition.y + (_Cam.OrthographicBounds().extents.y - _tolerance),
                    App.GetModule<LevelModule>().CurrentRoom.OriginPosition.y + App.GetModule<LevelModule>().CurrentRoom.Height - (_Cam.OrthographicBounds().extents.y) + _tolerance),
                    0);
            }
        }
    }
}