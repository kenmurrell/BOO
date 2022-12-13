using System;

namespace BtrfsObjectOpener
{
    // Reference: https://btrfs.wiki.kernel.org/index.php/On-disk_Format#Header
    public class InodeHeader : DiskObject
    {
        public int Size => 0x65;

        // Checksum of everything past this field (from 20 to 'Size'')
        public byte[] csum;

        // Filesystem UUID
        public byte[] fsid;

        // Logical address of this block
        public ulong logical_address;

        // Filesystem flags
        public byte[] flags;

        // Backref. Rev.: always 1 (MIXED) for new filesystems; 0 (OLD) indicates an old filesystem.
        public byte backref;

        // Chunk tree UUID
        public byte[] chunk_tree_uuid;

        // Generation
        public ulong generation;

        // 	The ID of the tree that contains this node
        public ulong tree_id;

        // Number of items (whatever that means)
        public uint nitems;

        // Level (0 for leaf nodes)
        public byte level;

        public InodeHeader()
        {
            csum = new byte[0x20];
            fsid = new byte[0x10];
            flags = new byte[0x01];
            chunk_tree_uuid = new byte[0x10];
        }

        public int ReadFrom(byte[] data, int offset)
        {
            if(Size > data.Length - offset)
            {
                return 0;
            }

            Utils.CopyArray(data, offset + 0x0, csum, 0, csum.Length);
            Utils.CopyArray(data, offset + 0x20, fsid, 0, fsid.Length);
            logical_address = BitConverter.ToUInt64(data, offset + 0x30);
            Utils.CopyArray(data, offset + 0x38, flags, 0, flags.Length);
            backref = data[offset + 0x3F];
            Utils.CopyArray(data, offset + 0x40, chunk_tree_uuid, 0, chunk_tree_uuid.Length);
            generation = BitConverter.ToUInt64(data, offset + 0x50);
            tree_id = BitConverter.ToUInt64(data, offset + 0x58);
            nitems = BitConverter.ToUInt32(data, offset + 0x60);
            level = data[offset + 0x64];

            return Size;
        }
    }
}


