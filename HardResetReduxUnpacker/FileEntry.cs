namespace HardResetReduxUnpacker
{
    public class FileEntry
    {
        public string Name { get; set; }
        public uint DecompressedSize { get; set; }
        public uint CompressedSize { get; set; }
        public long RelativeOffset { get; set; }
        public long LastWriteTime { get; set; }
        public long AbsoluteOffset { get; set; }
    }
}
