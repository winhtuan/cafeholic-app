using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CAFEHOLIC.Utils
{
    public class S3Client
    {
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;
        private readonly string _folderPrefix;

        public S3Client(string accessKey, string secretKey, string bucketName, string folderPrefix = "")
        {
            _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.APSoutheast1);
            _bucketName = bucketName;
            _folderPrefix = folderPrefix?.TrimEnd('/') + "/";
        }

        public async Task<string> UploadFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            string fileName = Path.GetFileName(filePath);
            string s3Key = _folderPrefix + fileName;

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(filePath, _bucketName, s3Key);

            string url = $"https://{_bucketName}.s3.ap-southeast-1.amazonaws.com/{s3Key}";
            return url;
        }
    }
}