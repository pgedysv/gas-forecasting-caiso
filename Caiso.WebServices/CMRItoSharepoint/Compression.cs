using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace CMRItoSharepoint
{
    class Compression
    {
        public static byte[] Compress(byte[] input)
        {
            byte[] output;
            using (var memStr = new MemoryStream())
            {
                using (var gs = new GZipStream(memStr, CompressionMode.Compress))
                {
                    gs.Write(input, 0, input.Length);
                    gs.Close();
                    output = memStr.ToArray();
                }
            }
            return output;
        }

        public static byte[] Decompress(byte[] input)
        {
            using (var inputMemStream = new MemoryStream(input))
            {
                using (var stream = new GZipStream(inputMemStream, CompressionMode.Decompress))
                {
                    int size = 4096;
                    byte[] buffer = new byte[size];
                    using (var memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        } while (count > 0);
                        return memory.ToArray();
                    }
                }
            }
        }
    }
}
