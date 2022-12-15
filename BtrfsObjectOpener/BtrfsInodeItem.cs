using System;

namespace BtrfsObjectOpener
{
    // Reference: https://btrfs.wiki.kernel.org/index.php/Data_Structures#btrfs_inode_item
    public class BtrfsInodeItem : DiskObject
    {
        public int Size => 0x15A;

        // Generation
        public ulong generation;

        // transid
        public ulong transid;

        // stat.st_size; Size of the file in bytes.
        public ulong size;

        // stat.st_blocks in byte units. Size allocated to this file, in bytes;
        // Sum of the offset fields of all EXTENT_DATA items for this inode. For directories: 0.
        public ulong nbytes;

        // Unused for normal inodes. Contains byte offset of block group when used as a free space inode.
        public ulong block_group;

        // stat.st_nlink; Count of INODE_REF entries for the inode. When used outside of a file tree, 1.
        public uint nlink;

        // stat.st_uid
        public uint uid;

        // stat.st_gid
        public uint gid;

        // stat.st_mode
        public uint mode;

        // stat.st_rdev
        public ulong rdev;

        // Inode flags
        public ulong flags;

        // Sequence number used for NFS compatibility. Initialized to 0 and incremented each time mtime value is changed.
        public ulong sequence;

        // Reserved for future use
        public byte[] reserved;

        // TODO: Replace these arrays with btrfs_timespec structs
        // stat.st_atime
        public byte[] atime;

        // stat.st_ctime
        public byte[] ctime;

        // stat.st_mtime
        public byte[] mtime;

        // stat.st_otime
        public byte[] otime;

        public BtrfsInodeItem()
        {
            reserved = new byte[4 * 8];
            atime = new byte[0x12];
            ctime = new byte[0x12];
            mtime = new byte[0x12];
            otime = new byte[0x12];
        }

        public int ReadFrom(byte[] data, int offset)
        {
            if(Size > data.Length - offset)
            {
                return 0;
            }

            generation = BitConverter.ToUInt64(data, offset);
            transid = BitConverter.ToUInt64(data, offset + 0x08);
            size = BitConverter.ToUInt64(data, offset + 0x16);
            nbytes = BitConverter.ToUInt64(data, offset + 0x24);
            block_group = BitConverter.ToUInt64(data, offset + 0x32);
            nlink = BitConverter.ToUInt32(data, offset + 0x40);
            uid = BitConverter.ToUInt32(data, offset + 0x44);
            gid = BitConverter.ToUInt32(data, offset + 0x48);
            mode = BitConverter.ToUInt32(data, offset + 0x52);
            rdev = BitConverter.ToUInt64(data, offset + 0x56);
            flags = BitConverter.ToUInt64(data, offset + 0x64);
            sequence = BitConverter.ToUInt64(data, offset + 0x72);
            Utils.CopyArray(data, offset + 0x80, reserved, 0, reserved.Length);
            Utils.CopyArray(data, offset + 0x112, atime, 0, atime.Length);
            Utils.CopyArray(data, offset + 0x124, ctime, 0, ctime.Length);
            Utils.CopyArray(data, offset + 0x136, mtime, 0, mtime.Length);
            Utils.CopyArray(data, offset + 0x148, otime, 0, otime.Length);

            return Size;
        }
    }
}


