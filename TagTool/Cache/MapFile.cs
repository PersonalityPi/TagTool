﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TagTool.Common;
using TagTool.IO;
using TagTool.Serialization;

namespace TagTool.Cache
{
    public enum MapFileVersion : int
    {
        Invalid = 0x0,
        HaloXbox = 0x5,
        HaloPC = 0x7,
        Halo2 = 0x8,
        Halo3Beta = 0x9,
        Halo3 = 0xB,
        HaloReach = 0xC,
        HaloOnline = 0x12,
    }

    public class MapFile : IMapFile
    {
        private static readonly Tag Head = new Tag("head");
        private static readonly Tag Foot = new Tag("foot");
        private static readonly int MapFileVersionOffset = 0x4;

        public readonly CacheVersion Version;
        public readonly MapFileVersion MapVersion;
        public readonly EndianFormat EndianFormat;

        private IMapFileHeader Header;

        public MapFile(EndianReader reader)
        {
            EndianFormat = DetectEndianFormat(reader);
            MapVersion = GetMapFileVersion(reader);
            Version = DetectCacheVersion(reader);
            var deserializer = new TagDeserializer(Version);
            reader.SeekTo(0);
            var dataContext = new DataSerializationContext(reader);
            Header = (MapFileHeader)deserializer.Deserialize(dataContext, typeof(MapFileHeader));
        }

        private static EndianFormat DetectEndianFormat(EndianReader reader)
        {
            reader.SeekTo(0);
            reader.Format = EndianFormat.LittleEndian;
            if (reader.ReadTag() == Head)
                return EndianFormat.LittleEndian;
            else
            {
                reader.SeekTo(0);
                reader.Format = EndianFormat.BigEndian;
                if (reader.ReadTag() == Head)
                    return EndianFormat.BigEndian;
                else
                    throw new Exception("Invalid map file header tag!");
            }
        }

        private static bool IsHalo2Vista(EndianReader reader)
        {
            reader.SeekTo(0x24);
            if (reader.ReadInt32() == -1)
                return true;
            else
                return false;
        }

        private static string GetBuildDate(EndianReader reader, MapFileVersion version)
        {
            var buildDataLength = 0x20;
            switch (version)
            {
                case MapFileVersion.HaloPC:
                case MapFileVersion.HaloXbox:
                    reader.SeekTo(0x40);
                    break;
                case MapFileVersion.Halo2:
                    if (IsHalo2Vista(reader))
                        reader.SeekTo(0x12C);
                    else
                        reader.SeekTo(0x120);
                    break;

                case MapFileVersion.Halo3Beta:
                case MapFileVersion.Halo3:
                case MapFileVersion.HaloReach:
                    reader.SeekTo(0x11C);
                    break;

                default:
                    throw new Exception("Map file version not supported (build date)!");
            }
            return reader.ReadString(buildDataLength);
        }

        private static MapFileVersion GetMapFileVersion(EndianReader reader)
        {
            reader.SeekTo(MapFileVersionOffset);
            return (MapFileVersion)reader.ReadInt32();
        }

        private static CacheVersion DetectCacheVersion(EndianReader reader)
        {
            var version = GetMapFileVersion(reader);
            var buildDate = GetBuildDate(reader, version);
            return CacheVersionDetection.GetFromBuildName(buildDate);
        }

        //
        // Interface methods
        //

        public CacheFileInterop GetInterop() => Header.GetInterop();

        public CacheFilePartition[] GetPartitions() => Header.GetPartitions();

        public int GetMemoryBufferSize() => Header.GetMemoryBufferSize();

        public int GetHeaderSize(CacheVersion version) => Header.GetHeaderSize(version);

        public void ApplyMagic(int magic) => Header.ApplyMagic(magic);

        public uint GetTagIndexAddress() => Header.GetTagIndexAddress();

        public void SetTagIndexAddress(uint newAddress) => Header.SetTagIndexAddress(newAddress);

        public int GetStringIDsIndicesOffset() => Header.GetStringIDsIndicesOffset();

        public CacheIndexHeader GetIndexHeader(EndianReader reader, int magic)
        {
            switch (Version)
            {
                case CacheVersion.Halo3Retail:
                case CacheVersion.Halo3ODST:
                case CacheVersion.HaloReach:
                    reader.SeekTo(GetTagIndexAddress());
                    return new CacheIndexHeader
                    {
                        TagGroupCount = reader.ReadInt32(),
                        TagGroupsOffset = reader.ReadInt32() - magic,
                        TagCount = reader.ReadInt32(),
                        TagsOffset = reader.ReadInt32() - magic,
                        TagInfoHeaderCount = reader.ReadInt32(),
                        TagInfoHeaderOffset = reader.ReadInt32() - magic,
                        TagInfoHeaderCount2 = reader.ReadInt32(),
                        TagInfoHeaderOffset2 = reader.ReadInt32() - magic
                    };

                default:
                    throw new NotImplementedException();
            }
        }

        public int GetTagNamesIndicesOffset() => Header.GetTagNamesIndicesOffset();

        public int GetTagNamesBufferOffset() => Header.GetTagNamesBufferOffset();

        public int GetTagNamesBufferSize() => Header.GetTagNamesBufferSize();

        public int GetStringIDsBufferOffset() => Header.GetStringIDsBufferOffset();

        public int GetStringIDsBufferSize() => Header.GetStringIDsBufferSize();

        public int GetStringIDsCount() => Header.GetStringIDsCount();
    }

    public interface IMapFile : IMapFileHeader
    {
        CacheIndexHeader GetIndexHeader(EndianReader reader, int magic);
    }

    public interface IMapFileHeader
    {
        CacheFileInterop GetInterop();
        CacheFilePartition[] GetPartitions();
        int GetMemoryBufferSize();
        int GetHeaderSize(CacheVersion version);
        void ApplyMagic(int magic);

        uint GetTagIndexAddress();
        void SetTagIndexAddress(uint newAddress);
        
        int GetTagNamesIndicesOffset();
        int GetTagNamesBufferOffset();
        int GetTagNamesBufferSize();

        int GetStringIDsIndicesOffset();
        int GetStringIDsBufferOffset();
        int GetStringIDsBufferSize();
        int GetStringIDsCount();
    }

}
