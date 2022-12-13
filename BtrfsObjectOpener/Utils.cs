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
    }
}