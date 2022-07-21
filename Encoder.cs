using LameDLLWrap;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DvMod.RadioBridge
{
    public class Encoder
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFilename);

        private Impl impl;

        static Encoder()
        {
            Main.DebugLog(() => $"Calling LoadLibrary");
            var result = LoadLibrary(Path.Combine(Main.mod.Path, "libmp3lame.64.dll"));
            Main.DebugLog(() => $"libmp3lame result from LoadLibrary={result}");
/*
            result = LoadLibrary(Path.Combine(Main.mod.Path, "msvcrt.dll"));
            Main.DebugLog(() => $"msvcrt result from LoadLibrary={result}");
*/
        }

        public Encoder()
        {
            impl = new Impl();
        }

        public void Encode(byte[] pcm, int length, Stream output)
        {
            impl.Encode(pcm, length, output);
        }

        public void Flush(Stream output)
        {
            impl.Flush(output);
        }

        private class Impl
        {
            private readonly LibMp3Lame lame = new LibMp3Lame();
            private readonly short[] uncompressed = new short[16384];
            private readonly byte[] compressed = new byte[16384];

            public Impl()
            {
                lame.InputSampleRate = 48000;
                lame.NumChannels = 2;
                lame.BitRate = 160;
                if (!lame.InitParams())
                    throw new Exception("Failed to initialize LibMp3Lame");
            }

            public void Encode(byte[] pcm, int length, Stream output)
            {
                Main.DebugLog($"Entering Encoder.Encode with {length} bytes of input");
                for (int i = 0; i < length / 2; i++)
                    uncompressed[i] = BitConverter.ToInt16(pcm, i * 2);
                Main.DebugLog($"Translated to {length / 2} shorts");
                int mp3ByteCount = lame.Write(uncompressed, length / 2, compressed, compressed.Length, false);
                if (mp3ByteCount < 0)
                    throw new Exception("Error from LAME: {bytesWritten");
                Main.DebugLog($"received {mp3ByteCount} from lame.Write");
                output.Write(compressed, 0, mp3ByteCount);
            }

            public void Flush(Stream output)
            {
                Main.DebugLog("Flushing LAME encoder");
                int mp3ByteCount = lame.Flush(compressed, compressed.Length);
                if (mp3ByteCount < 0)
                    throw new Exception("Error from LAME: {bytesWritten");
                Main.DebugLog($"received {mp3ByteCount} from lame.Write");
                output.Write(compressed, 0, mp3ByteCount);
            }
        }
    }
}
