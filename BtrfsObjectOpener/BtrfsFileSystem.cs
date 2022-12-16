

namespace BtrfsObjectOpener
{
    public class BtrfsFileSystem
    {
        private SuperBlock _superBlock;
        private Stream _stream;
        private ChunkTreeCacheFactory _chunkTreefactory;

        public BtrfsFileSystem(Stream stream)
        {
            this._stream = stream;

            // Load superblock
            _superBlock = new SuperBlock();
            byte[] buffer = new byte[_superBlock.Size];
            Utils.ReadAtOffset(stream, buffer, 0x10000);
            _superBlock.ReadFrom(buffer, 0);

            _chunkTreefactory = new ChunkTreeCacheFactory(_superBlock);
            ChunkTreeCache = _chunkTreefactory.Create(_stream);
        }

        public ChunkTreeCache ChunkTreeCache { get; }

        public ulong TreeRoot => _superBlock.root;

        private byte[] ReadRootTreeRoot(ChunkTreeCache chunkTreeCache, SuperBlock superBlock)
        {
            var root_tree_root_size = chunkTreeCache.GetMapping(superBlock.root)!.Value.Item1.size;
            var root_tree_root_physical_address = chunkTreeCache.GetPhysicalOffset(superBlock.root);

            var buffer = new byte[root_tree_root_size];
            Utils.ReadAtOffset(_stream, buffer, root_tree_root_physical_address!.Value);

            return buffer;
        }

        private byte[] ReadFsTreeRoot(byte[] root)
        {
            var header = new BtrfsHeader();
            header.ReadFrom(root, 0);

            if (header.level != 0)
            {
                throw new ArgumentException("Root tree root is not a leaf node");
            }

            var items = Utils.ParseLeaf(header, root);
            foreach (var item in items.Reverse())
            {
                if (item.key.objectid != BtrfsRootItem.BTRFS_FS_TREE_OBJECTID ||
                    item.key.type != BtrfsRootItem.BTRFS_ROOT_ITEM_KEY)
                {
                    continue;
                }

                var btrfsRootItem = new BtrfsRootItem();
                btrfsRootItem.ReadFrom(root, (int)(header.Size + item.offset));

                var fs_tree_root_size = ChunkTreeCache.GetMapping(btrfsRootItem.bytenr)!.Value.Item1.size;
                var fs_tree_root_physical_address = ChunkTreeCache.GetPhysicalOffset(btrfsRootItem.bytenr);

                var buffer = new byte[fs_tree_root_size];
                Utils.ReadAtOffset(_stream, buffer, fs_tree_root_physical_address!.Value);

                return buffer;
            }

            throw new ArgumentException("Failed to parse FR tree root");
        }
    }
}