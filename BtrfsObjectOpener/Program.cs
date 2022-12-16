

namespace BtrfsObjectOpener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var fsImagePath = args[0];
            var stream = new BufferedStream(new FileStream(fsImagePath, FileMode.Open, FileAccess.Read));
            var filesystem = new BtrfsFileSystem(stream);

            Console.WriteLine("Found these files:");
            filesystem.PrintAllFiles();
        }
    }
}