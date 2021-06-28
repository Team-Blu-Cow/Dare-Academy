using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace blu.FileIO
{
    public interface IFileFormat
    {
        public string FileExtension();
    }
}