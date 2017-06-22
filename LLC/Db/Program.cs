using Db.Extensions;
using Db.Models;
using System;

namespace Db
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage <bucket name> <access key> <secret key>");
                return 1;
            }

            // Assume the arguments are in the correct order
            // If the access/secret keys are wrong, it won't
            // matter anyway
            string bucket = args[0];
            string aKey   = args[1];
            string sKey   = args[2];

            // First, create all of the folders in the bucket
            // that do not use DynamoDB
            using (var client = S3Helper.S3Client(aKey, sKey))
            {
                S3Helper.CreateBucket(client, bucket, "Packages");
                S3Helper.CreateBucket(client, bucket, "Screenshots");
            }

            // Now, move the data into S3/DynamoDB
            using (var db = new LORLinkCheckerEntities())
            {

            }

            return 0;
        }
    }
}
