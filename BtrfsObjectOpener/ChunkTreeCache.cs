namespace BtrfsObjectOpener;

public record ChunkKey(ulong start, ulong size);

public record ChunkValue(ulong offset);

public class ChunkTreeCache
{
    private List<(ChunkKey, ChunkValue)> cache = new();
    
    public void Insert(ChunkKey key, ChunkValue value) {
        if (!Exists(key))
        {
            cache.Add((key, value));
        }
    }

    private bool Exists(ChunkKey key)
    {
        return cache.Exists(x => x.Item1.start == key.start);
    }

    public ulong? GetPhysicalOffset(ulong logical)
    {
        foreach (var (k, v) in cache)
        {
            if (logical >= k.start && logical < k.start + k.size)
            {
                return v.offset + logical - k.start;
            }
        }

        return null;
    }

    public (ChunkKey, ChunkValue)? GetMapping(ulong logical)
    {
        foreach (var (k, v) in cache)
        {
            if (logical >= k.start && logical < k.start + k.size)
            {
                return (k, v);
            }
        }

        return null; 
    }

    public static ChunkTreeCache? Bootstrap(byte[] sys_chunk_array,  uint sys_chunk_array_size)
    {
        var offset = 0;
        var cache = new ChunkTreeCache();
        
        while (offset < sys_chunk_array_size)
        {
            var key = new BtrfsDiskKey();
            key.ReadFrom(sys_chunk_array, offset);
            if (key.type != BtrfsChunkItem.Key)
            {
                Console.WriteLine($"Invalid item of type {key.type} inside sys_array at offset {offset}");
                return null;
            }
            offset += key.Size;

            var chunk = new BtrfsChunkItem();
            chunk.ReadFrom(sys_chunk_array, offset);
            if (chunk.stripeCount == 0)
            {
                Console.WriteLine("Stripe count can't be zero");
                return null;
            }

            var stripe = chunk.stripes.First();
            var logical = key.offset;

            if (cache.GetPhysicalOffset(logical) == null)
            {
                cache.Insert(new ChunkKey(logical, chunk.chunkSize), new ChunkValue(stripe.offset));
            }

            offset += chunk.Size;
        }

        return cache;
    }
}