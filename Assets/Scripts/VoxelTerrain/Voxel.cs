﻿using System;
using UnityEngine;
using System.Collections;

public struct Voxel : IEquatable<Voxel>
{
    #region Statics {{{
    public enum MeshShapeType : byte
    {
        None = 0x00,
        Cube = 0x01,
        Ramp = 0x02,
        SmallCorner = 0x03,
        LargeCorner = 0x04,
        MiterConvex = 0x05,
        MiterConcave = 0x06,
        Reserved = 0x07
    }
    public enum RotationType : byte
    {
        North = 0x00 << 0x03,
        East = 0x01 << 0x03,
        South = 0x02 << 0x03,
        West = 0x03 << 0x03
    }
    public const byte UpsideDownFlag = 0x01 << 0x05;
    public static readonly Voxel Empty = new Voxel(MeshShapeType.None, 0);

    public static byte CalcShape(MeshShapeType meshShape, RotationType rotation, bool upsideDown)
    {
        var result = (byte)((byte)meshShape | (byte)rotation);
        if (upsideDown)
        {
            result |= UpsideDownFlag;
        }
        return result;
    }
    #endregion }}}

    #region Fields {{{
    public readonly byte ShapeData;
    public readonly ushort BlockType;
    #endregion }}}

    #region Constructors {{{
    public Voxel(MeshShapeType meshShape, RotationType rotation, bool upsideDown, ushort blockType)
    {
        ShapeData = CalcShape(meshShape, rotation, upsideDown);
        BlockType = blockType;
    }
    public Voxel(MeshShapeType meshShape, ushort blockType)
    {
        ShapeData = CalcShape(meshShape, RotationType.North, false);
        BlockType = blockType;
    }
    public Voxel(byte shapeData, ushort blockType)
    {
        ShapeData = shapeData;
        BlockType = blockType;
    }
    #endregion }}}

    #region Properties {{{
    public MeshShapeType MeshShape
    {
        get { return (MeshShapeType)(ShapeData & 0x07); }
    }
    public RotationType Rotation
    {
        get { return (RotationType)(ShapeData & 0x18); }
    }
    public bool IsUpsideDown
    {
        get { return (ShapeData & UpsideDownFlag) > 0; }
    }
    #endregion }}}

    #region Methods {{{
    public Voxel ChangeShape(MeshShapeType meshShape, RotationType rotation, bool upsideDown)
    {
        return new Voxel(meshShape, rotation, upsideDown, BlockType);
    }
    public Voxel ChangeBlockType(ushort blockType)
    {
        return new Voxel(ShapeData, blockType);
    }

    public override string ToString()
    {
        return string.Format("Voxel {0} {1} {2}", MeshShape, Rotation, IsUpsideDown ? "upside down" : "upwards");
    }
    #endregion }}}

    public bool Equals(Voxel other)
    {
        return this.ShapeData == other.ShapeData && this.BlockType == other.BlockType;
    }
}
