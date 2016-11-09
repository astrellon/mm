using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;

[RequireComponent(typeof(VoxelTerrain))]
[ExecuteInEditMode]
public class VoxelLoader : MonoBehaviour {

    public string FilePath;

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
        Load(terrain, FilePath);
        terrain.RenderAll();
    }

    public static string LevelPath(string path)
    {
        return Path.Combine(@"Assets/Levels", path);
    }

    public static void Load(VoxelTerrain terrain, string path)
    {
        var filename = Path.GetFileNameWithoutExtension(path);

        foreach (var file in Directory.GetFiles(LevelPath(Path.GetDirectoryName(path)), "test" + "*.chunk"))
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
        Save(terrain, FilePath, onlyDirty);
    }

    public static void Save(VoxelTerrain terrain, string path, bool onlyDirty)
    {
        foreach (var chunk in terrain.Chunks.Values)
        {
            if (onlyDirty && !chunk.IsDataDirty)
            {
                continue;
            }

            var pos = chunk.ChunkPosition;
            var filename = string.Format("{0}.{1}.{2}.{3}.chunk", LevelPath("test"), pos.x, pos.y, pos.z);
            Debug.Log("Saving chunk: " + filename);
            ChunkSerializer.Serialize(chunk, filename);
            chunk.IsDataDirty = false;
        }
    }
}
