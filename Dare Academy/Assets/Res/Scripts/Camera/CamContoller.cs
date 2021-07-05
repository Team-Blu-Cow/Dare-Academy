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
        private bool _keepPlayerInFrame;

        private Vector3 _currentTarget { get => _virtualCam.Follow.position; }

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

        public void MoveToCurrentRoom()
        {
            Vector3 TargetPosition;
            Grid<GridNode> TargetGrid = App.GetModule<LevelModule>().CurrentRoom;
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
                Bounds cameraBounds;
                cameraBounds = _Cam.OrthographicBounds();

                for (int corner = 0; corner < 4; corner++)
                {
                    _player.GetComponent<SpriteRenderer>();
                }
            }
        }
    }
}