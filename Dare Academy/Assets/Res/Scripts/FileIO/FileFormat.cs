using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace blu.FileIO
{
    public abstract class BaseFileFormat
    {
        // this will be in a human readable json format
        abstract protected string EditorFileExtension();

        // this will be in json then encrypted using AES-256
        abstract protected string ReleaseFileExtension();

        public string FileExtension()
        {
            if (UnityEngine.Application.isEditor)
            { return EditorFileExtension(); }
            else
            { return ReleaseFileExtension(); }
        }
    }

    public class SaveData : BaseFileFormat
    {
        protected override string EditorFileExtension()
        { return "uwu-debug"; }

        protected override string ReleaseFileExtension()
        { return "uwu"; }

        public string displayName = null;
    }

    public class DebugSaveConfigFile : BaseFileFormat
    {
        protected override string EditorFileExtension()
        { return "dscf"; }

        protected override string ReleaseFileExtension()
        { return "dscf"; }

        private static string m_kFilePath = "/Editor/DebugSaveConfig.dscf";
        public static string path { get => m_kFilePath; }

        public string debugFileLocation;
        public bool enable = false;
        public bool allowSaving = false;
        public bool loadOnStart = false;
    }
}