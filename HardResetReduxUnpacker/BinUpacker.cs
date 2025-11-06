using System.IO.Compression;
using System.Text;

namespace HardResetReduxUnpacker
{
    public class BinUpacker
    {

        public static void Unpack(string binFilePath, string outputDirectory)
        {
            using (FileStream fs = new FileStream(binFilePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                uint fileCount = reader.ReadUInt32();
                Console.WriteLine($"Archive contains {fileCount} files.");

                List<FileEntry> fileEntries = new List<FileEntry>();
                for (int i = 0; i < fileCount; i++)
                {
                    var entry = new FileEntry();

                    int nameLength = reader.ReadInt32();

                    byte[] nameBytes = reader.ReadBytes(nameLength);
                    entry.Name = Encoding.UTF8.GetString(nameBytes);

                    entry.DecompressedSize = reader.ReadUInt32();
                    entry.CompressedSize = reader.ReadUInt32();
                    entry.RelativeOffset = reader.ReadInt64();
                    entry.LastWriteTime = reader.ReadInt64();

                    fileEntries.Add(entry);
                }

                reader.ReadUInt32();
                long dataBlockStartOffset = reader.BaseStream.Position;

                Console.WriteLine($"Data block starts at offset: 0x{dataBlockStartOffset:X}");

                foreach (var entry in fileEntries)
                {
                    entry.AbsoluteOffset = dataBlockStartOffset + entry.RelativeOffset;

                    Console.WriteLine($"Extracting: {entry.Name} (Compressed: {entry.CompressedSize}, Decompressed: {entry.DecompressedSize})");
                    string outputFilePath = Path.Combine(outputDirectory, entry.Name.Replace('/', Path.DirectorySeparatorChar));
                    byte[] decompressedData;
                    reader.BaseStream.Seek(entry.AbsoluteOffset, SeekOrigin.Begin);
                    if (entry.CompressedSize > 0)
                    {
                        byte[] compressedData = reader.ReadBytes((int)entry.CompressedSize);
                        decompressedData = DecompressZlib(compressedData, entry.DecompressedSize);

                    }
                    else
                    {
                        decompressedData = reader.ReadBytes((int)entry.DecompressedSize);
                    }
                    string? fileDir = Path.GetDirectoryName(outputFilePath);
                    if (!string.IsNullOrEmpty(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    File.WriteAllBytes(outputFilePath, decompressedData);
                }
            }
        }

        public static byte[] DecompressZlib(byte[] compressedData, uint decompressedSize)
        {
            using (MemoryStream compressedStream = new MemoryStream(compressedData))
            using (ZLibStream zlibStream = new ZLibStream(compressedStream, CompressionMode.Decompress))
            using (MemoryStream resultStream = new MemoryStream())
            {
                zlibStream.CopyTo(resultStream);

                if (resultStream.Length != decompressedSize)
                {
                    Console.WriteLine($"Warning: Decompressed size mismatch. Expected {decompressedSize}, got {resultStream.Length}.");
                }

                return resultStream.ToArray();
            }
        }
    }
}
