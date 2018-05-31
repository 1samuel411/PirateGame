using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

public static class ObjectSerializer
{

    public static byte[] Serialize(this Object obj)
    {
        if (obj == null)
        {
            return null;
        }

        using (var memoryStream = new MemoryStream())
        {
            var binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, obj);

            var compressed = Compress(memoryStream.ToArray());
            return compressed;
        }
    }

    public static Object DeSerialize(this byte[] arrBytes)
    {
        using (var memoryStream = new MemoryStream())
        {
            var binaryFormatter = new BinaryFormatter();
            var decompressed = Decompress(arrBytes);

            memoryStream.Write(decompressed, 0, decompressed.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return (Object)binaryFormatter.Deserialize(memoryStream);
        }
    }

    public static byte[] Compress(byte[] input)
    {
        byte[] compressesData;

        using (var outputStream = new MemoryStream())
        {
            using (var zip = new GZipStream(outputStream, CompressionMode.Compress))
            {
                zip.Write(input, 0, input.Length);
            }

            compressesData = outputStream.ToArray();
        }

        return compressesData;
    }

    public static byte[] Decompress(byte[] input)
    {
        byte[] decompressedData;

        using (var outputStream = new MemoryStream())
        {
            using (var inputStream = new MemoryStream(input))
            {
                using (GZipStream zip = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    CopyTo(zip, outputStream);
                }
            }

            decompressedData = outputStream.ToArray();
        }

        return decompressedData;
    }

    public static void CopyTo(Stream input, Stream outputStream)
    {
        byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
        int bytesRead;
        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            outputStream.Write(buffer, 0, bytesRead);
        }
    }
}
