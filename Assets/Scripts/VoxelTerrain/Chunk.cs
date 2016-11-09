using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    public const int TotalVoxels = 8 * 8 * 8;
    public readonly Voxel[] Voxels = new Voxel[TotalVoxels];
    public Vector3 ChunkPosition = Vector3.zero;
    public bool IsDirty = true;
    public bool IsDataDirty = true;
    public bool Loaded = false;

    public VoxelTerrain Parent;

    public void SetVoxel(uint x, uint y, uint z, Voxel voxel)
    {
        this.Voxels[(z << 6) + (y << 3) + x] = voxel;
        this.IsDirty = true;
        this.IsDataDirty = true;
    }
    public bool GetVoxel(uint x, uint y, uint z, ref Voxel voxel)
    {
        if (x >= 8 || y >= 8 || z >= 8)
        {
            return false;
        }
        voxel = this.Voxels[(z << 6) + (y << 3) + x];
        return true;
    }
}
