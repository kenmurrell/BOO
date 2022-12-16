using System;

namespace BtrfsObjectOpener
{
    // Reference: https://btrfs.wiki.kernel.org/index.php/On-disk_Format#Internal_Node
    public class BtrfsKeyPtr : DiskObject
    {
        public int Size => 0x21;

        // Key
        public BtrfsDiskKey key;

        // Block number
        public ulong block_number;

        // Generation number
        public ulong generation;

        public BtrfsKeyPtr()
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
            this.block_number = BitConverter.ToUInt64(data, offset + 0x11);
            generation = BitConverter.ToUInt64(data, offset + 0x19);

            return Size;
        }
    }
}


