using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;

[RequireComponent(typeof(VoxelTerrain))]
[ExecuteInEditMode]
public class VoxelLoader : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void Load()
    {
        var terrain = GetComponent<VoxelTerrain>();
        terrain.Clear();
        Load(terrain);
        terrain.RenderAll();
    }

    public static string LevelPath(string path)
    {
        return Path.Combine(@"Assets/Levels", path);
    }

    public static void Load(VoxelTerrain terrain)
    {
        foreach (var file in Directory.GetFiles(LevelPath(terrain.LevelName), "*.chunk"))
        {
            Debug.Log("Related files: " + file);
            Vector3 chunkPosition;
            if (!ChunkSerializer.GetChunkPosition(file, out chunkPosition))
            {
                return;
            }

            var chunk = terrain.GetChunk((int)chunkPosition.x, (int)chunkPosition.y, (int)chunkPosition.z);
            ChunkSerializer.Deserialize(chunk, file);
        }
    }

    public void Save(bool onlyDirty = false)
    {
        var terrain = GetComponent<VoxelTerrain>();
        Save(terrain, onlyDirty);
    }

    public static void Save(VoxelTerrain terrain, bool onlyDirty)
    {
        var baseFolder = LevelPath(terrain.LevelName);
        Directory.CreateDirectory(baseFolder);
        foreach (var chunk in terrain.Chunks.Values)
        {
            if (onlyDirty && !chunk.IsDataDirty)
            {
                continue;
            }

            var pos = chunk.ChunkPosition;
            var filename = Path.Combine(baseFolder, string.Format("{0}.{1}.{2}.chunk", pos.x, pos.y, pos.z));
            Debug.Log("Saving chunk: " + filename);
            ChunkSerializer.Serialize(chunk, filename);
            chunk.IsDataDirty = false;
        }
    }
}
