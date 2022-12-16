namespace BtrfsObjectOpener;


/// Reference: https://btrfs.wiki.kernel.org/index.php/On-disk_Format#CHUNK_ITEM_.28e4.29
/// Corresponds to a `CHUNK_ITEM`
public class BtrfsChunkItem : DiskObject
{
    public const byte Key = 228;
        
    public int Size => 0x30 + stripeCount * 0x20;
    
    /// Size of chunk
    public ulong chunkSize;
    
    /// Root referencing this chunk
    public ulong rootId;
    
    /// Stripe Length
    public ulong stripeLength;
    
    /// Type
    public ulong type;
    
    /// Optimal IO alignment
    public uint optimalIoAlign;
    
    /// Optimal Io Width
    public uint optimalIoWidth;
    
    /// Minimal Io Size
    public uint minimalIoSize;
    
    /// Number of stripes
    public ushort stripeCount;
    
    /// Only matters for RAID10
    public ushort subStripe;
    
    /// Contained stripes 
    public BtrfsStripe[] stripes = new BtrfsStripe[0];

    
    public int ReadFrom(byte[] data, int offset)
    {
        if(Size > data.Length - offset)
        {
            return 0;
        }

        chunkSize = BitConverter.ToUInt64(data, offset);
        rootId = BitConverter.ToUInt64(data, offset + 0x8);
        stripeLength = BitConverter.ToUInt64(data, offset + 0x10);
        type = BitConverter.ToUInt64(data, offset + 0x18);
        optimalIoAlign = BitConverter.ToUInt32(data, offset + 0x20);
        optimalIoWidth = BitConverter.ToUInt32(data, offset + 0x24);
        minimalIoSize = BitConverter.ToUInt32(data, offset + 0x28);
        stripeCount = BitConverter.ToUInt16(data, offset + 0x2c);
        subStripe = BitConverter.ToUInt16(data, offset + 0x2e);

        stripes = new BtrfsStripe[stripeCount];
        for (int i = 0; i < stripeCount; i++)
        {
            var stripe = new BtrfsStripe();
            stripe.ReadFrom(data, offset + 0x30 + stripe.Size*i);

            stripes[i] = stripe;
        }
            
        return Size;
    }
}

/// Reference: https://btrfs.wiki.kernel.org/index.php/On-disk_Format#CHUNK_ITEM_.28e4.29
/// Corresponds to a `CHUNK_ITEM`'s Stripe
public class BtrfsStripe : DiskObject
{
    public int Size => 0x20;

    public ulong deviceId;

    public ulong offset;

    public byte[] deviceUUID;

    public BtrfsStripe()
    {
        deviceUUID = new byte[0x10];
    }
    
    public int ReadFrom(byte[] data, int offset)
    {
        if(Size > data.Length - offset)
        {
            return 0;
        }

        deviceId = BitConverter.ToUInt64(data, offset);
        this.offset = BitConverter.ToUInt64(data, offset + 0x8);
        Utils.CopyArray(data, offset + 0x10, deviceUUID, 0, deviceUUID.Length);

        return Size;
    }
}