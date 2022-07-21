using System.IO;

namespace DvMod.RadioBridge
{
    public class SplitStream : Stream
    {
        private readonly Stream[] streams;
        private bool _disposed;

        public SplitStream(params Stream[] streams)
        {
            this.streams = streams;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new System.NotImplementedException();

        public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override void Flush()
        {
            foreach (var stream in streams)
                stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            foreach (var stream in streams)
                stream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                foreach (var stream in streams)
                    stream.Dispose();
            }
            _disposed = true;
        }
    }
}
