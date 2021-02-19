using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Text;

namespace BlobClientOpenWrite
{
    class Program
    {
        static string accountName = ""; // Put your values here
        static string accountKey = "";  // Put your values here
        static string container = "";   // Put your values here

        static string endpointSuffix = "core.windows.net";
        static Uri blobServiceUri = new Uri($"https://{accountName}.blob.{endpointSuffix}");

        static string UploadZipBlob(BlobServiceClient blobServiceClient, string folder, int size)
        {
            var time = DateTime.Now.ToString("HH mm");
            var fileName = size + "-" + Guid.NewGuid().ToString();
            var blobFilePath =  $"{folder}\\blob-{time}-{fileName}";
            var diskFilePath = $"C:\\!!!!!!!!!!!!xxx\\disk-{time}-{fileName}";

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
            var blockBlobClient = blobContainerClient.GetBlockBlobClient($"{blobFilePath}.csv.zip");

            using (var outputStream = blockBlobClient.OpenWrite(true))
            //using (var outputStream = new FileStream($"{diskFilePath}.csv.zip", FileMode.CreateNew, FileAccess.Write))
            {
                using (var outputZipStream = new ZipOutputStream(outputStream))
                {
                    using (var writer = new StreamWriter(outputZipStream, Encoding.UTF8, -1, true))
                    {
                        outputZipStream.PutNextEntry(new ZipEntry($"{fileName}.csv"));

                        writer.WriteLine("header1, header2, header3, header4, header5, header6, header7, header8, header9, header10");

                        writer.Write(new string('C', size));

                        writer.Flush();
                        writer.Close();

                        outputZipStream.CloseEntry();
                    }
                }
            }

            return fileName;
        }

        static void Main(string[] args)
        {
            var folder = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            var sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
            var blobServiceClient = new BlobServiceClient(blobServiceUri, sharedKeyCredential);

            var fileName = UploadZipBlob(blobServiceClient, folder, 1);

            Console.WriteLine("Hello World!");
        }
    }
}
