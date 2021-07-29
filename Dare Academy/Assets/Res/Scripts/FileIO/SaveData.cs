using System.Collections.Generic;
using blu.FileIO;

namespace blu
{
    // All data that should be written to disk should be stored within SaveData
    // Everything in here must be tagged with System.Serializable
    public class SaveData : IFileFormat
    {
        public string FileExtension()
        {
            return "uwu-debug";
        }

        public int saveSlot = 0;
        public LevelID levelId = LevelID._default;
        public System.Int32 gameEventFlags = 0;
        public int respawnRoomID = -1;
        public double playtime = 0;
        public int maxHealth = 3;
        public int maxEnergy = 3;
        public int partsCollected = 0;

        public List<Quest.StringListIntPair> m_roomsTraveled = new List<Quest.StringListIntPair>();

        public List<StrippedQuest> activeQuests = new List<StrippedQuest>();
        public List<StrippedQuest> completedQuests = new List<StrippedQuest>();
    }
}