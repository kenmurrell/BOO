using System;
using System.IO;
using System.Text;

namespace BOO
{
    class Program
    {
        static void Main(string[] args)
        {
            var stream = new BufferedStream(new FileStream("superblock.bin", FileMode.Open, FileAccess.Read));
            byte[] data = new byte[0x1000];
            stream.Read(data, 0, data.Length);
            var superblock = new SuperBlock();
            superblock.ReadFrom(data, 0);
            var magic_arr = BitConverter.GetBytes(superblock.magic);
            Console.WriteLine($"Superblock Magic: {Encoding.Default.GetString(magic_arr)}");
        }
    }
}