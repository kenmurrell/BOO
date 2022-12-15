namespace BtrfsObjectOpener;

public record ChunkKey(ulong start, ulong size);

public record ChunkValue(ulong offset);

public class ChunkTreeCache
{
    private List<(ChunkKey, ChunkValue)> cache = new();
    
    public void Insert(ChunkKey key, ChunkValue value) {
        cache.Add((key, value));
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

    public static ChunkTreeCache? Bootstrap(SuperBlock superBlock)
    {
        var arraySize = superBlock.sys_chunk_array_size;
        var offset = 0;
        var cache = new ChunkTreeCache();
        var sys_array = superBlock.sys_chunk_array;
        
        while (offset < arraySize)
        {
            var key = new BtrfsDiskKey();
            key.ReadFrom(sys_array, offset);
            if (key.type != BtrfsChunkItem.Key)
            {
                Console.WriteLine($"Invalid item of type {key.type} inside sys_array at offset {offset}");
                return null;
            }
            offset += key.Size;

            var chunk = new BtrfsChunkItem();
            chunk.ReadFrom(sys_array, offset);
            if (chunk.stripeCount == 0)
            {
                Console.WriteLine("Stripe count can't be zero");
                return null;
            } if (chunk.stripeCount > 1)
            {
                Console.WriteLine($"Warning: {chunk.stripeCount} stripes detected, but only using the first");
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