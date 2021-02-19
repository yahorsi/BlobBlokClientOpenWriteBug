using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Text;

namespace BlobClientOpenWrite
{
    class Program
    {
        static string accountName = "";
        static string accountKey = "";
        static string container = "";

        static string endpointSuffix = "core.windows.net";
        static Uri blobServiceUri = new Uri($"https://{accountName}.blob.{endpointSuffix}");

        static string UploadZipBlob(BlobServiceClient blobServiceClient, string folder, int size)
        {
            var time = DateTime.Now.ToString("HH mm");
            var fileName = size + "-" + Guid.NewGuid().ToString();
            var blobFilePath =  $"{folder}\\blob-{time}-{fileName}";
            var diskFilePath = $"C:\\!!!!!!!!!!!!xxx\\disk-{time}-{fileName}";

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
            var blockBlobClient = blobContainerClient.GetBlockBlobClient($"{blobFilePath}.txt");

            using (var outputStream = blockBlobClient.OpenWrite(true))
            {
                using (var writer = new StreamWriter(outputStream, Encoding.ASCII))
                {
                    writer.Write(new string('A', 100));
                    writer.Flush();

                    writer.Write(new string('B', 50));
                    writer.Flush();

                    writer.Write(new string('C', 25));
                    writer.Flush();
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
