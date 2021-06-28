using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Threading.Tasks;

namespace blu.FileIO
{
    public class FileLoaderStaticUtility
    {
        public static bool CreateDirectory(string dir)
        {
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.DirectoryInfo info = System.IO.Directory.CreateDirectory(dir);
            }
            return true; // TODO - check return value of System.IO.Directory.CreateDirectory(dir)
        }

        public static bool DestroyDirectory(string dir)
        {
            if (System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.Delete(dir, true);
            }
            return true;
        }

        public static string[] GetFilesInDirectory(string dir)
        {
            return System.IO.Directory.GetFiles(dir);
        }
    }

    abstract public class BaseFileLoader<T> : FileLoaderStaticUtility where T : class, IFileFormat
    {
        public static bool createConsoleLogs = false;

        protected string m_path = null;

        // takes full file path from root
        protected BaseFileLoader(string in_path)
        {
            m_path = in_path;
            if (createConsoleLogs)
            {
                UnityEngine.Debug.Log("creating file handle [" + in_path + "]");
            }
        }

        public bool FileExists()
        {
            if (System.IO.File.Exists(m_path))
                return true;
            else
                return false;
        }

        public abstract T ReadData();

        public abstract bool WriteData(T in_data);

        public Task<T> ReadDataAsync()
        {
            return Task<T>.Run(() => { return ReadData(); });
        }

        public Task<bool> WriteDataAsync(T in_data)
        {
            return Task<bool>.Run(() => { return WriteData(in_data); });
        }

        public bool DeleteFile()
        {
            if (FileExists())
            {
                System.IO.File.Delete(m_path);
            }
            return true;
        }
    }

    public class DebugFileLoader<T> : BaseFileLoader<T> where T : class, IFileFormat
    {
        public DebugFileLoader(string in_path) : base(in_path)
        {
        }

        public override T ReadData()
        {
            T data = null;

            if (FileExists())
            {
                string json = System.IO.File.ReadAllText(m_path);
                data = JsonUtility.FromJson<T>(json);
            }
            return data;
        }

        public override bool WriteData(T in_data)
        {
            string json = JsonUtility.ToJson(in_data);
            System.IO.File.WriteAllText(m_path, json);
            return true;
        }
    }

    public class BinaryFileLoader<T> : BaseFileLoader<T> where T : class, IFileFormat
    {
        public BinaryFileLoader(string in_path) : base(in_path)
        {
        }

        public override T ReadData()
        {
            T data = null;

            if (FileExists())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                System.IO.FileStream stream = new System.IO.FileStream(m_path, System.IO.FileMode.Open);

                data = formatter.Deserialize(stream) as T;
                stream.Close();
            }
            return data;
        }

        public override bool WriteData(T in_data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.FileStream stream = new System.IO.FileStream(m_path, System.IO.FileMode.Create);
            formatter.Serialize(stream, in_data);
            stream.Close();
            return true;
        }
    }

    public class EncryptedFileLoader<T> : BaseFileLoader<T> where T : class, IFileFormat
    {
        public EncryptedFileLoader(string in_path, Encryptor encryptor) : base(in_path)
        {
            m_encryptor = encryptor;
        }

        private Encryptor m_encryptor;

        public override T ReadData()
        {
            T data = null;

            if (FileExists())
            {
                byte[] encryptedJson = System.IO.File.ReadAllBytes(m_path);
                string json = m_encryptor.Decrypt(encryptedJson);
                data = JsonUtility.FromJson<T>(json);
            }
            return data;
        }

        public override bool WriteData(T in_data)
        {
            if (m_encryptor == null)
                return false;

            string json = JsonUtility.ToJson(in_data);
            byte[] encryptedJson = m_encryptor.Encrypt(json);
            System.IO.File.WriteAllBytes(m_path, encryptedJson);
            return true;
        }
    }
}