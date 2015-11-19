using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Lab_7.Model
{
    static class DataService<T> where T : new()
    {

        private const string FilePath = "data.dat";

        public class Options
        {
            public enum Modes { Binary, XML, JSON }   ;

            public Modes Mode { get; }
            public bool Compress { get; }
            public bool Encrypt { get; }

            public Options(Modes mode, bool compress, bool encrypt)
            {
                Mode = mode;
                Compress = compress;
                Encrypt = encrypt;
            }

        }

        public static T Load(Options options)
        {
            try
            {
                var file = File.Open(FilePath, FileMode.Open);
                var stream = new MemoryStream();
                file.CopyTo(stream);
                file.Close();

                stream.Position = 0;
                if (options.Compress)
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            zipStream.CopyTo(decompressedStream);
                        }

                        stream.Close();
                        stream = new MemoryStream(decompressedStream.ToArray());
                    }
                }

                if (options.Encrypt)
                {
                    var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                    var passwordBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                    using (var dencryptedStream = new MemoryStream())
                    using (var aes = new RijndaelManaged())
                    {
                        aes.KeySize = 256;
                        aes.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        aes.Key = key.GetBytes(aes.KeySize / 8);
                        aes.IV = key.GetBytes(aes.BlockSize / 8);

                        aes.Mode = CipherMode.CBC;

                        using (var cryptoStream = new CryptoStream(dencryptedStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            stream.CopyTo(cryptoStream);
                        }

                        stream.Close();
                        stream = new MemoryStream(dencryptedStream.ToArray());
                    }
                }

                if (options.Mode == Options.Modes.Binary)
                {
                    var binaryFormatter = new BinaryFormatter();
                    return (T) binaryFormatter.Deserialize(stream);
                }
                else if (options.Mode == Options.Modes.XML)
                {
                    var xmlSerializer = new XmlSerializer(typeof (T));
                    return (T) xmlSerializer.Deserialize(stream);
                }
                else
                {
                    var jsonSerializer = new DataContractJsonSerializer(typeof(T));
                    return (T) jsonSerializer.ReadObject(stream);
                }
            }
            catch (Exception)
            {
                return new T();
            }
        }

        public static void Save(T obj, Options options)
        {
            var stream = new MemoryStream();

            if (options.Mode == Options.Modes.Binary)
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, obj);
            }
            else if (options.Mode == Options.Modes.XML)
            {
                var serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(stream, obj);
            }
            else
            {
                var settings = new DataContractJsonSerializerSettings {UseSimpleDictionaryFormat = true};
                var jsonSerializer = new DataContractJsonSerializer(typeof(T), settings);
                jsonSerializer.WriteObject(stream, obj);
            }

            stream.Position = 0;
            if (options.Encrypt)
            {
                var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                var passwordBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using (var encryptedStream = new MemoryStream())
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cryptoStream = new CryptoStream(encryptedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        stream.CopyTo(cryptoStream);
                    }

                    stream.Close();
                    stream = new MemoryStream(encryptedStream.ToArray());
                }
            }

            if (options.Compress)
            {
                using (var compressedStream = new MemoryStream())
                {

                    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        stream.CopyTo(zipStream);
                    }

                    stream.Close();
                    stream = new MemoryStream(compressedStream.ToArray());
                }
            }

            var file = File.Create(FilePath);
            stream.CopyTo(file);
            stream.Close();
            file.Close();
        }

    }
}
