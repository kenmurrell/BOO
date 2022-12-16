using System;

namespace BtrfsObjectOpener
{
    public class Utils
    {
        public static int CopyArray(byte[] srcArray, int srcOff, byte[] destArray, int destOff, int length)
        {
            if(destArray == null)
            {
                destArray = new byte[destOff + length];
            }
            if(srcArray == null)
            {
                throw new ArgumentException("Source array is null");
            }
            for(int c = 0; c < length; c++)
            {
                try
                {
                    destArray[destOff + c] = srcArray[srcOff + c];
                }
                catch (IndexOutOfRangeException)
                {
                    return c;
                }
            }
            return length;
        }

        public static void ReadAtOffset(Stream stream, byte[] buffer, ulong offset)
        {
            stream.Seek((long)offset, 0);
            stream.Read(buffer, 0, buffer.Length);
        }

        public static BtrfsItem[] ParseLeaf(BtrfsHeader header, byte[] buffer)
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

        public static BtrfsKeyPtr[] ParseNode(BtrfsHeader header, byte[] buffer)
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
    }
}