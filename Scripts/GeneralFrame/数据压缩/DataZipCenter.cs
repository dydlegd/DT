using UnityEngine;
using System;
using System.Collections;
using System.IO;
//using System.IO.Compression;
using SevenZip.Compression.LZMA;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;

//------------------------数据压缩与解压---------------------//
namespace DYD
{
    public class DataZipCenter
    {
        //public static byte[] CompressGZIP(byte[] io)
        //{
        //    System.IO.MemoryStream basestream = new System.IO.MemoryStream();
        //    using (System.IO.Compression.GZipStream compressstream = new GZipStream(basestream, CompressionMode.Compress, true))
        //    {
        //        compressstream.Write(io, 0, io.Length);
        //        compressstream.Flush();
        //        compressstream.Close();
        //    }
        //    basestream.Position = 0;
        //    return basestream.GetBuffer();
        //}

        //public static System.IO.StringReader DeCompressGZIP(byte[] str)
        //{
        //    System.IO.MemoryStream stream = new System.IO.MemoryStream();
        //    stream.Write(str, 0, str.Length);
        //    stream.Position = 0;
        //    GZipStream zip = new GZipStream(stream, CompressionMode.Decompress);
        //    System.IO.StreamReader rd = new System.IO.StreamReader(zip);
        //    return new System.IO.StringReader(rd.ReadToEnd());
        //}

        public static void CompressFileLZMA(string inFile, string outFile)
        {
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Write the encoder properties
            coder.WriteCoderProperties(output);

            // Write the decompressed file size.
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);

            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);
            output.Flush();
            output.Close();
            input.Close();
        }

        public static void DecompressFileLZMA(string inFile, string outFile)
        {
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            // Decompress the file.
            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            output.Close();
            input.Close();
        }

        public static byte[] CompressByGZIP(byte[] binary)
        {
            MemoryStream ms = new MemoryStream();
            GZipOutputStream gzip = new GZipOutputStream(ms);
            gzip.Write(binary, 0, binary.Length);
            gzip.Close();
            //byte[] press = ms.ToArray();
            //Debug.Log(Convert.ToBase64String(press) + "  " + press.Length);
            return ms.ToArray();
        }

        public static byte[] DeCompressByGZIP(byte[] press)
        {
            GZipInputStream gzi = new GZipInputStream(new MemoryStream(press));

            MemoryStream re = new MemoryStream();
            int count = 0;
            byte[] data = new byte[4096];
            while ((count = gzi.Read(data, 0, data.Length)) != 0)
            {
                re.Write(data, 0, count);
            }
            //byte[] depress = re.ToArray();
            //Debug.Log(Encoding.UTF8.GetString(depress));
            return re.ToArray();
        }
    }
}

