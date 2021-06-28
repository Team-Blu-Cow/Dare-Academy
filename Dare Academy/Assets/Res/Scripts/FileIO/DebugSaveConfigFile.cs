using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace blu.FileIO
{
    public class DebugSaveConfigFile : IFileFormat
    {
        public string FileExtension()
        { return "dscf"; }

        private static string m_kFilePath = "/Editor/DebugSaveConfig.dscf";
        public static string path { get => m_kFilePath; }

        public string debugFileLocation;
        public bool enable = false;
        public bool allowSaving = false;
        public bool loadOnStart = false;
    }
}