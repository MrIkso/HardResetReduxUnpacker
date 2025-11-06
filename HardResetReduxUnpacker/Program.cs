
namespace HardResetReduxUnpacker
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("Hard Reset Redux .bin Unpacker");

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: HardResetUnpacker.exe <path_to_bin_file>");
                return;
            }

            string binFilePath = args[0];

            if (!File.Exists(binFilePath))
            {
                Console.WriteLine($"Error: File not found at '{binFilePath}'");
                return;
            }

            string outputDirectory = Path.Combine(Path.GetDirectoryName(binFilePath) ?? "", Path.GetFileNameWithoutExtension(binFilePath) + "_unpacked");
            Directory.CreateDirectory(outputDirectory);

            try
            {
                BinUpacker.Unpack(binFilePath, outputDirectory);
                Console.WriteLine("Unpacking completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
    }
}