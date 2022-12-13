using System;

namespace BtrfsObjectOpener
{
    public class SuperBlock : DiskObject
    {
        public int Size => 1000;

        // Checksum of everything past this field (from 20 to 1000)
        public byte[] csum;

        // Filesystem UUID
        public byte[] fsid;

        // Physical address of this block
        public ulong bytenr;

        // Filesystem flags
        public ulong flags;

        // Btrfs Magic
        public ulong magic;

        // Generation
        public ulong generation;

        // Logical address of the root tree root
        public ulong root;

        // Logical address of the chunk tree root
        public ulong chunck_root;

        // Logical address fo the log tree root
        public ulong log_root;

        public ulong log_root_transid;

        public ulong total_bytes;

        public ulong bytes_used;

        public ulong root_dir_objectid;

        public ulong num_devices;

        public uint sectorsize;

        public uint nodesize;

        public uint unused_leafsize;

        public uint stripesize;

        public uint sys_chunk_array_size;

        public ulong chunk_root_generation;

        public ulong compat_flags;

        public ulong compat_ro_flags;

        public ulong incompat_flags;

        public ushort csum_type;

        public byte root_level;

        public byte chunk_root_level;

        public byte log_root_level;

        public byte[] dev_item;

        public byte[] label;

        public ulong cache_generation;

        public ulong uuid_tree_generation;

        public byte[] reserved;

        public byte[] sys_chunk_array;

        public byte[] super_roots;

        public byte[] unused;

        public SuperBlock()
        {
            csum = new byte[0x20];
            fsid = new byte[0x10];
            dev_item = new byte[0x62];
            label = new byte[0x100];
            reserved = new byte[0xf0];
            sys_chunk_array = new byte[0x800];
            super_roots = new byte[0x2a0];
            unused = new byte[0x235];
        }


        public int ReadFrom(byte[] data, int offset)
        {
            if(Size > data.Length - offset)
            {
                return 0;
            }
            Utils.CopyArray(data, offset + 0x0, csum, 0, csum.Length);
            Utils.CopyArray(data, offset + 0x20, fsid, 0, fsid.Length);

            bytenr = BitConverter.ToUInt64(data, offset + 0x30);
            flags = BitConverter.ToUInt64(data, offset + 0x38);
            magic = BitConverter.ToUInt64(data, offset + 0x40);
            generation = BitConverter.ToUInt64(data, offset + 0x48);
            root = BitConverter.ToUInt64(data, offset + 0x50);
            chunck_root = BitConverter.ToUInt64(data, offset + 0x58);
            log_root = BitConverter.ToUInt64(data, offset + 0x60);
            log_root_transid = BitConverter.ToUInt64(data, offset + 0x68);
            total_bytes = BitConverter.ToUInt64(data, offset + 0x70);
            bytes_used = BitConverter.ToUInt64(data, offset + 0x78);
            root_dir_objectid = BitConverter.ToUInt64(data, offset + 0x80);
            num_devices = BitConverter.ToUInt64(data, offset + 0x88);

            sectorsize = BitConverter.ToUInt32(data, offset + 0x90);
            nodesize = BitConverter.ToUInt32(data, offset + 0x94);
            unused_leafsize = BitConverter.ToUInt32(data, offset + 0x98);
            stripesize = BitConverter.ToUInt32(data, offset + 0x9c);
            sys_chunk_array_size = BitConverter.ToUInt32(data, offset + 0xa0);

            chunk_root_generation = BitConverter.ToUInt64(data, offset + 0xa4);
            compat_flags = BitConverter.ToUInt64(data, offset + 0xac);
            compat_ro_flags = BitConverter.ToUInt64(data, offset + 0xb4);
            incompat_flags = BitConverter.ToUInt64(data, offset + 0xbc);

            csum_type = BitConverter.ToUInt16(data, offset + 0xc4);
            root_level = data[0xc6];
            chunk_root_level = data[0xc7];
            log_root_level = data[0xc8];

            Utils.CopyArray(data, offset + 0xc9, dev_item, 0, dev_item.Length);
            Utils.CopyArray(data, offset + 0x12b, label, 0, label.Length);

            cache_generation = BitConverter.ToUInt64(data, offset + 0x22b);
            uuid_tree_generation = BitConverter.ToUInt64(data, offset + 0x223);

            Utils.CopyArray(data, offset + 0x23b, reserved, 0, reserved.Length);
            Utils.CopyArray(data, offset + 0x32b, super_roots, 0, super_roots.Length);
            Utils.CopyArray(data, offset + 0xb2b, super_roots, 0, super_roots.Length);
            Utils.CopyArray(data, offset + 0xdcb, unused, 0, unused.Length);

            return Size;
        }
    }
}


