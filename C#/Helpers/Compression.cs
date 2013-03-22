#region
using System;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
#endregion

namespace Compression
{
    /// <summary>
    /// Compression/Décompression de fichiers et flux
    /// </summary>
    internal static class Compression
    {
        /// <summary>
        /// Décompression d'un flux compressé en gzip
        /// </summary>
        /// <param name="compressedStream"></param>
        /// <returns></returns>
        public static byte[] Decompress(Stream compressedStream)
        {
            // Will hold the compressed stream created from the destination stream
            var gzDecompressed = new GZipStream(compressedStream, CompressionMode.Decompress, true);

            // Retrieve the size of the file from the compressed archive's footer
            var bufferWrite = new byte[4];
            compressedStream.Position = (int) compressedStream.Length - 4;
            // Write the first 4 bytes of data from the compressed file into the buffer
            compressedStream.Read(bufferWrite, 0, 4);
            // Set the position back at the start
            compressedStream.Position = 0;
            int bufferLength = BitConverter.ToInt32(bufferWrite, 0);
            var buffer = new byte[bufferLength];
            int readOffset = 0;
            int totalBytes = 0;

            // Loop through the compressed stream and put it into the buffer
            while (true)
            {
                int bytesRead = gzDecompressed.Read(buffer, readOffset, Math.Min(100, bufferLength - totalBytes));
                // If we reached the end of the data
                if (bytesRead == 0)
                {
                    break;
                }

                readOffset += bytesRead;
                totalBytes += bytesRead;
            }

            //return Encoding.UTF8.GetString(buffer);
            return buffer;
        }

#if UnUsed
        public static byte[] Decompress(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                memoryStream.Write(gzBuffer, 4, gzBuffer.Length - 4);

                var buffer = new byte[msgLength];

                memoryStream.Position = 0;
                using (var zip = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return buffer;
            }
        }
#endif
        /// <summary>
        /// Compression d'un flux en gzip
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string Compress(byte[] buffer)
        {
            //byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (GZipStream zip = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressed = new byte[memoryStream.Length];
            memoryStream.Read(compressed, 0, compressed.Length);

            var gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

//TODO Zipper les fichiers : Utiliser SharpZipLib est-il la meilleure solution????
        #region Compression/Décompression en fichier zip
        /// <summary>
        /// Compression de fichier et répertoire dans un fichier zip
        /// </summary>
        /// <param name="pathOfFiles"></param>
        /// <param name="zipFileName"></param>
        /// <param name="filesToAdd"></param>
        /// <returns></returns>
        public static bool MakeZipFromFiles(string pathOfFiles, string zipFileName, string[] filesToAdd, out Exception exception)
        {
            try
            {
                string pathZipFile = FileManagment.FormatPath(pathOfFiles) + zipFileName;
                FileStream ostream;
                byte[] obuffer;
                int trimLength = (Directory.GetParent(pathZipFile)).ToString().Length;
                trimLength += 1; //remove '\'
                using (var oZipStream = new ZipOutputStream(File.Create(pathZipFile)))
                {
                    oZipStream.SetLevel(9); // maximum compression
                    ZipEntry oZipEntry;
                    foreach (string filename in filesToAdd)
                    {
                        var filePath = FileManagment.FormatPath(pathOfFiles) + filename;
                        oZipEntry = new ZipEntry(filePath.Remove(0, trimLength));
                        oZipStream.PutNextEntry(oZipEntry);

                        if (!filePath.EndsWith(@"/")) // if a file ends with '/' its a directory
                        {
                            ostream = File.OpenRead(filePath);
                            obuffer = new byte[ostream.Length];
                            ostream.Read(obuffer, 0, obuffer.Length);
                            oZipStream.Write(obuffer, 0, obuffer.Length);
                            ostream.Close();
                        }
                    }
                    oZipStream.Finish();
                    oZipStream.Close();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
            exception = null;
            return true;
        }

        /// <summary>
        /// Décompression d'un fcihier zip
        /// </summary>
        /// <param name="zipPathAndFile"></param>
        /// <param name="outputFolder"></param>
        /// <param name="deleteZipFile"></param>
        public static void UnZipFiles(string zipPathAndFile, string outputFolder, bool deleteZipFile)
        {
            ZipInputStream s = new ZipInputStream(File.OpenRead(zipPathAndFile));
            ZipEntry theEntry;
            string tmpEntry = String.Empty;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = outputFolder;
                string fileName = Path.GetFileName(theEntry.Name);
                // create directory 
                if (directoryName != "")
                {
                    Directory.CreateDirectory(directoryName);
                }
                if (fileName != String.Empty)
                {
                    if (theEntry.Name.IndexOf(".ini") < 0)
                    {
                        string fullPath = directoryName + "\\" + theEntry.Name;
                        fullPath = fullPath.Replace("\\ ", "\\");
                        string fullDirPath = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(fullDirPath))
                        {
                            Directory.CreateDirectory(fullDirPath);
                        }
                        FileStream streamWriter = File.Create(fullPath);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
            }
            s.Close();
            if (deleteZipFile)
            {
                File.Delete(zipPathAndFile);
            }
        }
        #endregion
    }
}