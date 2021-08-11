using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class FoliageTile : Tile
{
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        Vector2[] uvs = sprite.uv;
        Vector2 min_UV = uvs[0];
        Vector2 max_UV = uvs[uvs.Length-1];

        if (Application.isPlaying)
        {
            
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/CustomAssets/FoliageTile", false, 1)]
    static void CreateTreeTileAsset()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Foliage Tile", "New Foliage Tile", "Asset", "Save Foliage Tile", "Custom Assets/Tiles");
        // check that path exists in project
        if (path == "")
        {
            Debug.Log($"Path {path} is not available in Project to create a new MaskedTile Instance");
            return;
        }
        //myHeightTile.InitiateSlots(myTextures);
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FoliageTile>(), path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif 
}
