using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int TotalVoxels = 8 * 8 * 8;
    public readonly Voxel[] Voxels = new Voxel[TotalVoxels];
    public Vector3 ChunkPosition = Vector3.zero;
    private bool isDirty = true;
    public bool IsDirty 
    {
        get { return this.isDirty; }
        internal set { this.isDirty = value; }
    }

    private bool isDataDirty = true;
    public bool IsDataDirty 
    {
        get { return this.isDataDirty; }
        internal set { this.isDataDirty = value; }
    }
    public bool Loaded = false;

    public VoxelTerrain Parent;

    public void SetVoxel(uint x, uint y, uint z, Voxel voxel)
    {
        if (x >= 8 || y >= 8 || z >= 8)
        {
            throw new System.Exception("Out of bounds set voxel");
        }
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

    public class BulkEdit : System.IDisposable
    {
        public readonly Chunk Chunk;

        public BulkEdit(Chunk chunk)
        {
            this.Chunk = chunk;
        }
        public void Dispose()
        {
            this.Chunk.IsDirty = true;
            this.Chunk.IsDataDirty = true;
        }
    }
}

