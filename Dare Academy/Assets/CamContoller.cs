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

        private void Start()
        {
            App.GetModule<LevelModule>().StepController.RoomChangeEvent += MoveToCurrentRoom;
            MoveToCurrentRoom();
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

        private void Move(Vector3 in_to)
        {
            _virtualCam.Follow.position = in_to;
        }
    }
}