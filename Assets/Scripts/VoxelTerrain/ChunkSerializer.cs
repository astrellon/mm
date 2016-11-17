using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public static class ChunkSerializer
{
    public static readonly byte[] Identifier = new byte[] { (byte)'m', (byte)'m', (byte)'c' };
    public const byte Version = 2;
    public static void Serialize(Chunk chunk, string filepath)
    {
        using (var file = File.Create(filepath))
        using (var writer = new BinaryWriter(file, Encoding.UTF8))
        {
            writer.Write(Identifier);
            writer.Write(Version);
            writer.Write((int)chunk.ChunkPosition.x);
            writer.Write((int)chunk.ChunkPosition.y);
            writer.Write((int)chunk.ChunkPosition.z);
    
            SerializeChunksVersion2(chunk, writer);
        }
    }

    private static void SerializeChunksVersion1(Chunk chunk, BinaryWriter writer)
    {
        for (var i = 0; i < chunk.Voxels.Length; i++)
        {
            SerializeVoxel(chunk.Voxels[i], writer);
        }
    }

    private static void SerializeChunksVersion2(Chunk chunk, BinaryWriter writer)
    {
        var countedVoxels = CountVoxels(chunk);
        foreach (var cv in countedVoxels)
        {
            if (cv.Count == 0)
            {
                continue;
            }

            var count = cv.Count;
            while (count > 255)
            {
                writer.Write((byte)255);
                SerializeVoxel(cv.Voxel, writer);
                count -= 255;
            }

            if (count > 0)
            {
                writer.Write((byte)count);
                SerializeVoxel(cv.Voxel, writer);
            }
        }
    }

    private class CountedVoxel
    {
        public int Count;
        public readonly Voxel Voxel;

        public CountedVoxel(Voxel voxel)
        {
            this.Count = 0;
            this.Voxel = voxel;
        }

        public override string ToString()
        {
            return "Counted Voxel: " + this.Count + " " + this.Voxel;
        }
    }

    private static List<CountedVoxel> CountVoxels(Chunk chunk)
    {
        var result = new List<CountedVoxel>();
        var currentVoxel = new CountedVoxel(Voxel.Empty);
        result.Add(currentVoxel);

        for (var i = 0; i < chunk.Voxels.Length; i++)
        {
            var nextVoxel = chunk.Voxels[i];

            if (!nextVoxel.Equals(currentVoxel.Voxel))
            {
                currentVoxel = new CountedVoxel(nextVoxel);
                currentVoxel.Count++;
                result.Add(currentVoxel);
            }
            else
            {
                currentVoxel.Count++;
            }
        }

        return result;
    }

    private static void SerializeVoxel(Voxel voxel, BinaryWriter writer)
    {
        writer.Write(voxel.ShapeData);
        writer.Write(voxel.BlockType);
    }

    public static void Deserialize(Chunk chunk, string filepath)
    {
        using (var file = File.OpenRead(filepath))
        using (var reader = new BinaryReader(file))
        {
            var version = ValidateChunkFile(reader);

            var chunkX = (float)reader.ReadInt32();
            var chunkY = (float)reader.ReadInt32();
            var chunkZ = (float)reader.ReadInt32();

            chunk.ChunkPosition = new Vector3(chunkX, chunkY, chunkZ);

            if (version == 1)
            {
                DeserializeChunksVersion1(chunk, reader);
            }
            else if (version >= 2)
            {
                DeserializeChunksVersion2(chunk, reader);
            }
            Debug.Log("(DONE) Loading at pos: " + reader.BaseStream.Position);
        }
    }

    public static bool GetChunkPosition(string filepath, out Vector3 result)
    {
        using (var file = File.OpenRead(filepath))
        using (var reader = new BinaryReader(file))
        {
            try
            {
                ValidateChunkFile(reader);
            }
            catch (Exception exp)
            {
                Debug.Log("Failed to get chunk position: " + exp.Message);
                result = Vector3.zero;
                return false;
            }
            
            var chunkX = (float)reader.ReadInt32();
            var chunkY = (float)reader.ReadInt32();
            var chunkZ = (float)reader.ReadInt32();

            result = new Vector3(chunkX, chunkY, chunkZ);
            return true;
        }
    }

    public static bool MatchesIdentifier(byte[] input)
    {
        return input.Length == Identifier.Length &&
            input[0] == Identifier[0] &&
            input[1] == Identifier[1] &&
            input[2] == Identifier[2];
    }

    private static byte ValidateChunkFile(BinaryReader reader)
    {
        var identifier = reader.ReadBytes(3);
        if (!MatchesIdentifier(identifier))
        {
            throw new Exception("Chunk deserialize error: invalid file identifier");
        }

        var version = reader.ReadByte();
        if (version > Version)
        {
            var error = string.Format("Chunk deserialize error: Version {0} is newer than supported {1}", version, Version);
            throw new Exception(error);
        }

        return version;
    }
        

    private static Voxel DeserializeVoxel(BinaryReader reader)
    {
        var shapeData = reader.ReadByte();
        var blockType = reader.ReadUInt16();
        return new Voxel(shapeData, blockType);
    }

    private static void DeserializeChunksVersion1(Chunk chunk, BinaryReader reader)
    {
        for (var i = 0; i < chunk.Voxels.Length; i++)
        {
            chunk.Voxels[i] = DeserializeVoxel(reader);
        }
    }

    private static void DeserializeChunksVersion2(Chunk chunk, BinaryReader reader)
    {
        var pos = 0;
        using (new Chunk.BulkEdit(chunk))
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var voxelCount = reader.ReadByte();
            var voxel = DeserializeVoxel(reader);

            for (var i = 0; i < voxelCount; i++)
            {
                chunk.Voxels[pos + i] = voxel;
            }
            pos += voxelCount;
        }
    }
}
