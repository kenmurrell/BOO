namespace BtrfsObjectOpener
{
    public interface DiskObject 
    {
        int Size { get; }

        int ReadFrom(byte[] data, int offset);
    }
}

