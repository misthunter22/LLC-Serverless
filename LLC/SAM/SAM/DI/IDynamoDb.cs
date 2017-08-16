using Amazon;
using Amazon.DynamoDBv2;
using SAM.Models.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAM.DI
{
    public interface IDynamoDb
    {
        RegionEndpoint Region();

        string TableCount(AmazonDynamoDBClient client, string tableName);

        List<SourceModel> Sources(AmazonDynamoDBClient client, string tableName, string bucketTableName);

        List<SettingModel> Settings(AmazonDynamoDBClient client, string tableName);

        SourceModel Source(AmazonDynamoDBClient client, string tableName, string bucketTableName, string id);

        Task<string> QueryCountBool(AmazonDynamoDBClient client, string tableName, string column, bool b);

        Task<string> QueryDataInt(AmazonDynamoDBClient client, string tableName, string key, string field);

        Task<string> QueryCountContains(AmazonDynamoDBClient client, string tableName, string column, string s);
    }
}
