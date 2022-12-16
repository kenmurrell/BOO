using System;

namespace BtrfsObjectOpener
{
    // Reference: https://btrfs.wiki.kernel.org/index.php/Btrfs_design#Btree_Data_structures
    // Corresponds to data type 'btrfs_disk_key'
    public class BtrfsDiskKey : DiskObject
    {
        public int Size => 0x11;

        // Object ID. Each tree has its own set of Object IDs.
        public ulong objectid;

        // Item type (see: https://btrfs.wiki.kernel.org/index.php/On-disk_Format#Item_Types)
        public byte type;

        // Offset. The meaning depends on the item type.
        public ulong offset;

        public int ReadFrom(byte[] data, int offset)
        {
            if(Size > data.Length - offset)
            {
                return 0;
            }

            objectid = BitConverter.ToUInt64(data, offset);
            type = data[offset + 0x08];
            this.offset = BitConverter.ToUInt64(data, offset + 0x09);

            return Size;
        }
    }
}


