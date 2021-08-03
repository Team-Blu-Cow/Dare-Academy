using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace blu.FileIO
{
    public class SaveSlotData
    {
        public string m_filepath = null;
        public blu.LevelID levelId = blu.LevelID._default;
        public double playtime = 0;
    }
}