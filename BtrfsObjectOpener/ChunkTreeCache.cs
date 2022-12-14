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
}