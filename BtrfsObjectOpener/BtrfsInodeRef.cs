using System;

namespace BtrfsObjectOpener
{
    // Reference: https://btrfs.wiki.kernel.org/index.php/Data_Structures#btrfs_inode_ref
    // Corresponds to data type 'btrfs_inode_ref'
    public class BtrfsInodeRef : DiskObject
    {
        public int Size => 10;

        // Index of the inode this item's referencing in the directory
        public ulong index;

        // length of the name, following this item
        public ushort name_len;

        public BtrfsInodeRef()
        {

        }

        public int ReadFrom(byte[] data, int offset)
        {
            if(Size > data.Length - offset)
            {
                return 0;
            }

            index = BitConverter.ToUInt64(data, offset);
            name_len = BitConverter.ToUInt16(data, offset + 8);

            return Size;
        }
    }
}


