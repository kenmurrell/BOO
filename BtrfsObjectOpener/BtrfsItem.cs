using System;

namespace BtrfsObjectOpener
{
    // Reference: https://btrfs.wiki.kernel.org/index.php/Btrfs_design#Btree_Data_structures
    // Corresponds to data type 'btrfs_item'
    public class BtrfsItem : DiskObject
    {
        public int Size => key.Size + 0x08;

        // Key
        public BtrfsDiskKey key;

        // Offset of data in this block.
        public uint offset;

        // Size of the data pointed to by offset
        public uint size;

        public BtrfsItem()
        {
            key = new BtrfsDiskKey();
        }

        public int ReadFrom(byte[] data, int offset)
        {
            if(Size > data.Length - offset)
            {
                return 0;
            }

            key.ReadFrom(data, offset);
            this.offset = BitConverter.ToUInt32(data, offset + key.Size);
            size = BitConverter.ToUInt32(data, offset + key.Size + 0x04);

            return Size;
        }
    }
}


