using CUE4Parse.UE4.Versions;
using Galaxy_Swapper_v2.Workspace.Swapping.Providers;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace CUE4Parse.UE4.Readers
{
    public class FStreamArchive : FArchive
    {
        private readonly Stream _baseStream;

        public FStreamArchive(string name, Stream baseStream, VersionContainer? versions = null) : base(versions)
        {
            _baseStream = baseStream;
            Name = name;
            CProvider.OpenedStreamers.Add(new Galaxy_Swapper_v2.Workspace.Generation.Formats.StreamData() { Name = System.IO.Path.GetFileNameWithoutExtension(name), Path = name, Stream = baseStream });
        }

        public override void Close() => _baseStream.Close();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count)
            => _baseStream.Read(buffer, offset, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin)
            => _baseStream.Seek(offset, origin);

        public override bool CanSeek => _baseStream.CanSeek;
        public override long Length => _baseStream.Length;
        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        public override string Name { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte[] ReadBytes(int length)
        {
            var result = new byte[length];
            _baseStream.Read(result, 0, length);
            return result;
        }

        public override object Clone()
        {
            return _baseStream switch
            {
                ICloneable cloneable => new FStreamArchive(Name, (Stream) cloneable.Clone(), Versions) {Position = Position},
                FileStream fileStream => new FStreamArchive(Name, File.Open(fileStream.Name, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite), Versions) {Position = Position},
                _ => new FStreamArchive(Name, _baseStream, Versions) {Position = Position}
            };
        }
    }
}
