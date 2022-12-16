

namespace BtrfsObjectOpener
{
    public class BtrfsFileSystem
    {
        private const ulong BtrfsFtRegFile = 1;
        
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

        private byte[] ReadRootTreeRoot()
        {
            var root_tree_root_size = ChunkTreeCache.GetMapping(_superBlock.root)!.Value.Item1.size;
            var root_tree_root_physical_address = ChunkTreeCache.GetPhysicalOffset(_superBlock.root);

            var buffer = new byte[root_tree_root_size];
            Utils.ReadAtOffset(_stream, buffer, root_tree_root_physical_address!.Value);

            return buffer;
        }

        private byte[] ReadFsTreeRoot()
        {
            var root = ReadRootTreeRoot();
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

        private void WalkFsTree(byte[] node)
        {
            var header = new BtrfsHeader();
            header.ReadFrom(node, 0);

            if (header.level == 0)
            {
                var items = Utils.ParseLeaf(header, node);
                foreach (var item in items)
                {
                    if (item.key.type != BtrfsDirItem.Key)
                    {
                        continue;
                    }

                    var dirItem = new BtrfsDirItem();
                    dirItem.ReadFrom(node, (int)(header.Size + item.offset));

                    if (dirItem.type != BtrfsFtRegFile)
                    {
                        continue;
                    }

                    var name = System.Text.Encoding.UTF8.GetString(node, (int)(header.Size + item.offset + dirItem.Size), dirItem.name_len);
                    Console.WriteLine(name);
                }
            }
            else {
                var pointers = Utils.ParseNode(header, node);
                foreach (var ptr in pointers)
                {
                    var physical = ChunkTreeCache.GetPhysicalOffset(ptr.block_number)!.Value;
                    var nodes = new byte[_superBlock.nodesize];
                    Utils.ReadAtOffset(_stream, nodes, physical);
                    WalkFsTree(nodes);
                }
            }
        }

        public void PrintAllFiles()
        {
            var root = ReadFsTreeRoot();
            WalkFsTree(root);
        }
    }
}