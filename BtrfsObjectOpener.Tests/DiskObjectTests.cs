using System;
using System.IO;
using System.Text;
using Xunit;

namespace BtrfsObjectOpener.Tests
{
    public class DiskObjectTests
    {
        [Fact]
        public void TestSuperBlock()
        {
            var stream = new BufferedStream(new FileStream("../../../TestData/superblock.bin", FileMode.Open, FileAccess.Read));
            byte[] data = new byte[0x1000];
            stream.Read(data, 0, data.Length);
            var superblock = new SuperBlock();
            superblock.ReadFrom(data, 0);
            var magic_arr = BitConverter.GetBytes(superblock.magic);
            Assert.Equal("_BHRfS_M", Encoding.Default.GetString(magic_arr));
        }
    }
}