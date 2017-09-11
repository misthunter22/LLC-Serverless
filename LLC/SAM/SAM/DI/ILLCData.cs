using Amazon;
using Amazon.DynamoDBv2;
using SAM.Models;
using SAM.Models.Admin;
using SAM.Models.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAM.DI
{
    public interface ILLCData
    {
        RegionEndpoint Region();

        string TableCount(AmazonDynamoDBClient client, string tableName);

        List<SourceModel> Sources(AmazonDynamoDBClient client, string tableName, string bucketTableName);

        SourceModel Source(AmazonDynamoDBClient client, string tableName, string bucketTableName, string id);

        List<SettingModel> Settings(AmazonDynamoDBClient client, string tableName);

        List<InvalidLinksModel> InvalidLinks(AmazonDynamoDBClient client, string tableName);

        List<WarningLinksModel> WarningLinks(AmazonDynamoDBClient client, string tableName);

        List<BucketLocationsModel> BucketLocations(AmazonDynamoDBClient client, string id, string objectLinksTable, string objectsTable, string bucketsTable);

        List<BucketsModel> Buckets(AmazonDynamoDBClient client, string tableName);

        Task<string> QueryCountBool(AmazonDynamoDBClient client, string tableName, string column, bool b);

        Task<string> QueryDataInt(AmazonDynamoDBClient client, string tableName, string key, string field);

        Task<string> QueryCountContains(AmazonDynamoDBClient client, string tableName, string column, string s);
    }
}
