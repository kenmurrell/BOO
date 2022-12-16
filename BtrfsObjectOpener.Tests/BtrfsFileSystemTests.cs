using System;
using System.IO;
using System.Text;
using Xunit;

namespace BtrfsObjectOpener.Tests
{
    public class BtrfsFileSystemTests
    {
        [Fact]
        public void TestBasicFileSystem()
        {
            var stream = new BufferedStream(new FileStream("../../../TestData/BtrfsDisk.vhd", FileMode.Open, FileAccess.Read));
            var fs = new BtrfsFileSystem(stream);
            var c = fs.ChunkTreeCache;
            var offset = c.GetPhysicalOffset(fs.TreeRoot);
            Assert.True(offset.HasValue);
            Assert.Equal<ulong>(0x2548000, offset.Value);
        }
    }
}