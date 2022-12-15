using System;

namespace BtrfsObjectOpener
{
    // Reference: https://btrfs.wiki.kernel.org/index.php/Data_Structures#btrfs_root_item
    // Corresponds to btrfs_root_item
    public class BtrfsRootItem : DiskObject
    {
        public int Size => 438;

        // Several fields are initialized but only flags is interpreted at runtime.
        // generation=1, size=3,nlink=1, nbytes=<leafsize>, mode=040755
        // flags depends on kernel version, see below.
        public BtrfsInodeItem inode;

        // TODO: The docs on the wiki for this item say "transid of the transaction that created this root." Should this be 'transid'?
        public ulong generation;

        // For file trees, the objectid of the root directory in this tree (always 256). Otherwise, 0.
        public ulong root_dirid;

        // 	The disk offset in bytes for the root node of this tree.
        public ulong bytenr;

        // 	Unused. Always 0.
        public ulong byte_limit;

        // 	Unused.
        public ulong bytes_used;

        // 	The last transid of the transaction that created a snapshot of this root.
        public ulong last_snapshot;

        // 	Flags
        public ulong flags;

        // 	Originally indicated a reference count. In modern usage, it is only 0 or 1.
        public uint refs;

        // Contains key of last dropped item during subvolume removal or relocation. Zeroed otherwise.
        public BtrfsDiskKey drop_progress;

        // 	The tree level of the node described in drop_progress.
        public byte drop_level;

        // The height of the tree rooted at bytenr.
        public byte level;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // The following fields depend on the subvol_uuids+subvol_times features

        // If equal to generation, indicates validity of the following fields.
        // If the root is modified using an older kernel, this field and generation will become out of sync. This is normal and recoverable.
        public ulong generation_v2;

        // This subvolume's UUID.
        public byte[] uuid;

        // The parent's UUID (for use with send/receive).
        public byte[] parent_uuid;

        // The received UUID (for used with send/receive).
        public byte[] received_uuid;

        // 	The transid of the last transaction that modified this tree, with some exceptions (like the internal caches or relocation).
        public ulong ctransid;

        // 	The transid of the transaction that created this tree.
        public ulong otransid;

        // 	The transid for the transaction that sent this subvolume. Nonzero for received subvolume.
        public ulong stransid;

        // 	The transid for the transaction that received this subvolume. Nonzero for received subvolume.
        public ulong rtransid;

        // TODO: Replace these arrays with btrfs_timespec structs
        // Timestamp for ctransid.
        public byte[] ctime;

        // Timestamp for otransid.
        public byte[] otime;

        // Timestamp for stransid.
        public byte[] stime;

        // Timestamp for rtransid.
        public byte[] rtime;

        // Reserved for future use
        public byte[] reserved;

        public BtrfsRootItem()
        {
            inode = new BtrfsInodeItem();
            drop_progress = new BtrfsDiskKey();
            reserved = new byte[8 * 8];
            ctime = new byte[0x12];
            otime = new byte[0x12];
            stime = new byte[0x12];
            rtime = new byte[0x12];
            uuid = new byte[16];
            parent_uuid = new byte[16];
            received_uuid = new byte[16];
        }

        public int ReadFrom(byte[] data, int offset)
        {
            if(Size > data.Length - offset)
            {
                return 0;
            }

            inode.ReadFrom(data, offset);
            generation = BitConverter.ToUInt64(data, offset + 160);
            root_dirid = BitConverter.ToUInt64(data, offset + 168);
            bytenr = BitConverter.ToUInt64(data, offset + 176);
            byte_limit = BitConverter.ToUInt64(data, offset + 184);
            bytes_used = BitConverter.ToUInt64(data, offset + 192);
            last_snapshot = BitConverter.ToUInt64(data, offset + 200);
            flags = BitConverter.ToUInt64(data, offset + 208);
            refs = BitConverter.ToUInt32(data, offset + 216);
            drop_progress.ReadFrom(data, offset + 220);
            drop_level = data[offset + 237];
            level = data[offset + 238];
            generation_v2 = BitConverter.ToUInt64(data, offset + 239);
            Utils.CopyArray(data, offset + 247, uuid, 0, uuid.Length);
            Utils.CopyArray(data, offset + 263, parent_uuid, 0, parent_uuid.Length);
            Utils.CopyArray(data, offset + 279, received_uuid, 0, received_uuid.Length);
            ctransid = BitConverter.ToUInt64(data, offset + 295);
            otransid = BitConverter.ToUInt64(data, offset + 303);
            stransid = BitConverter.ToUInt64(data, offset + 311);
            rtransid = BitConverter.ToUInt64(data, offset + 319);
            Utils.CopyArray(data, offset + 327, ctime, 0, ctime.Length);
            Utils.CopyArray(data, offset + 339, otime, 0, otime.Length);
            Utils.CopyArray(data, offset + 351, stime, 0, stime.Length);
            Utils.CopyArray(data, offset + 363, rtime, 0, rtime.Length);
            Utils.CopyArray(data, offset + 375, reserved, 0, reserved.Length);

            return Size;
        }
    }
}


