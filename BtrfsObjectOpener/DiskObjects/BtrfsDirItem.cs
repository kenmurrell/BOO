namespace BtrfsObjectOpener;


/// Reference: https://btrfs.wiki.kernel.org/index.php/Data_Structures#btrfs_dir_item
public class BtrfsDirItem : DiskObject
{
    public const ulong Key = 84;
    
    public int Size => 30;

    public BtrfsDiskKey location = new BtrfsDiskKey();

    public ulong transid;

    public ushort data_len;

    public ushort name_len;

    public byte type;
    
    
    public int ReadFrom(byte[] data, int offset)
    {
        if(Size > data.Length - offset)
        {
            return 0;
        }

        location.ReadFrom(data, offset);
        transid = BitConverter.ToUInt64(data, offset + 17);
        data_len = BitConverter.ToUInt16(data, offset + 25);
        name_len = BitConverter.ToUInt16(data, offset + 27);
        type = data[offset + 29];
            
        return Size;
    }
}