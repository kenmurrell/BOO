
namespace BtrfsObjectOpener
{
    class Program
    {
        private FileStream fs;

        public Program(string path)
        {
            this.fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        static void Main(string[] args)
        {
            Program program = new Program(args[0]);
            program.Parse();
        }

        public void Parse()
        {
            SuperBlock sb = new SuperBlock();
            byte[] buffer = new byte[sb.Size];
            ReadAtOffset(buffer, 0x10000);

            sb.ReadFrom(buffer, 0);

            var chunkTreeCache = ChunkTreeCache.Bootstrap(sb);
            ReadChunkTree(ReadChunkTreeRoot(chunkTreeCache, sb), chunkTreeCache, sb);
        }

        private void ReadAtOffset(byte[] buffer, ulong offset)
        {
            fs.Seek((long)offset, 0);
            fs.Read(buffer, 0, buffer.Length);
        }

        private BtrfsItem[] ParseLeaf(BtrfsHeader header, byte[] buffer)
        {
            var items = new BtrfsItem[header.nitems];
            var offset = header.Size;
            for (int i = 0; i < header.nitems; i++)
            {
                var item = new BtrfsItem();
                item.ReadFrom(buffer, offset);
                offset += item.Size;
                items[i] = item;
            }

            return items;
        }
        
        private BtrfsKeyPtr[] ParseNode(BtrfsHeader header, byte[] buffer)
        {
            var items = new BtrfsKeyPtr[header.nitems];
            var offset = header.Size;
            for (int i = 0; i < header.nitems; i++)
            {
                var item = new BtrfsKeyPtr();
                item.ReadFrom(buffer, offset);
                offset += item.Size;
                items[i] = item;
            }

            return items;
        }

        private byte[] ReadChunkTreeRoot(ChunkTreeCache chunkTreeCache, SuperBlock superBlock)
        {
            var size = chunkTreeCache.GetMapping(superBlock.chunck_root)!.Value.Item1.size;
            var physical = chunkTreeCache.GetPhysicalOffset(superBlock.chunck_root);

            var buffer = new byte[size];
            ReadAtOffset(buffer, physical!.Value);
            
            return buffer;
        }

        private void ReadChunkTree(byte[] root, ChunkTreeCache chunkTreeCache, SuperBlock superBlock)
        {
            var header = new BtrfsHeader();
            header.ReadFrom(root, 0);

            if (header.level == 0)
            {
                var items = ParseLeaf(header, root);

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
                var pointers = ParseNode(header, root);
                foreach (var ptr in pointers)
                {
                    var physical = chunkTreeCache.GetPhysicalOffset(ptr.block_number)!.Value;
                    var nodes = new byte[superBlock.nodesize];
                    ReadAtOffset(nodes, physical);
                    ReadChunkTree(nodes, chunkTreeCache, superBlock);
                }
            }
        }
    }
}