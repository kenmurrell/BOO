
namespace BtrfsObjectOpener
{
    public class ChunkTreeCacheFactory
    {
        private ulong _chunk_root;
        private uint _node_size;
        private byte[] _sys_chunk_array; 
        private uint _sys_chunk_array_size;

        public ChunkTreeCacheFactory(SuperBlock superblock)
        {
            _chunk_root = superblock.chunck_root;
            _node_size = superblock.nodesize;
            _sys_chunk_array = superblock.sys_chunk_array;
            _sys_chunk_array_size = superblock.sys_chunk_array_size;
        }


        public ChunkTreeCache Create(Stream stream)
        {
            var chunkTreeCache = ChunkTreeCache.Bootstrap(_sys_chunk_array, _sys_chunk_array_size);
            if(chunkTreeCache == null)
            {
                throw new Exception("Error bootstrapping ChunkTreeCache");
            }
            
            // Load the root
            var size = chunkTreeCache.GetMapping(_chunk_root)!.Value.Item1.size;
            var physical = chunkTreeCache.GetPhysicalOffset(_chunk_root);
            
            var root = new byte[size];
            Utils.ReadAtOffset(stream, root, physical!.Value);

            // Load the tree
            ReadChunkTree(stream, root, chunkTreeCache);

            return chunkTreeCache;
        }

        private void ReadChunkTree(Stream stream, byte[] root, ChunkTreeCache chunkTreeCache)
        {
            var header = new BtrfsHeader();
            header.ReadFrom(root, 0);

            if (header.level == 0)
            {
                var items = Utils.ParseLeaf(header, root);

                foreach (var item in items)
                {
                    if (item.key.type != BtrfsChunkItem.Key)
                    {
                        continue;
                    }

                    var chunk = new BtrfsChunkItem();
                    chunk.ReadFrom(root, (int)(header.Size + item.offset));

                    chunkTreeCache.Insert(new ChunkKey(item.key.offset, chunk.chunkSize), new ChunkValue(chunk.stripes[0].offset));
                }
            }
            else
            {
                var pointers = Utils.ParseNode(header, root);
                foreach (var ptr in pointers)
                {
                    var physical = chunkTreeCache.GetPhysicalOffset(ptr.block_number)!.Value;
                    var nodes = new byte[_node_size];
                    Utils.ReadAtOffset(stream, nodes, physical);
                    ReadChunkTree(stream, nodes, chunkTreeCache);
                }
            }
        }

        
    }


}