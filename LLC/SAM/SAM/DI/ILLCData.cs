using Amazon;
using Amazon.CloudWatch.Model;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Model;
using SAM.Models;
using SAM.Models.Admin;
using SAM.Models.Dynamo;
using SAM.Models.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAM.DI
{
    public interface ILLCData
    {
        RegionEndpoint Region();

        long TableCount(string tableName);

        List<Sources> Sources();

        Sources Source(string id, SourceSearchType type);

        List<Buckets> Buckets();

        List<Settings> Settings();

        List<InvalidLinks> InvalidLinks();

        List<WarningLinks> WarningLinks();

        void AddUrlToWarningLinks(List<WarningLinks> links);

        List<BucketLocationsModel> BucketLocations(BucketLocationsRequest m);

        GetObjectResponse ObjectGet(string bucket, string key);

        Task<T> SetTableRow<T>(T row);

        List<T> GetTableScan<T>(string column, string id);

        List<T> GetTableQuery<T>(string column, string id, string keyIndex);

        Task<long> IncrementMetaTableKey(string key, long diff);

        Task<long> SetMetaTableKey(string key, long set);

        Task<string> QueryCountBool(string tableName, string column, bool b);

        Task<AttributeValue> QueryDataAttribute(string tableName, string key, string field);

        Task<Dictionary<string, AttributeValue>> QueryDataAttributes(string tableName, string key);

        Task<Dictionary<string, AttributeValue>> QueryDataAttributes(string tableName, AttributeValue key, string field, string index);

        Task<string> QueryCountContains(string tableName, string column, string s);

        Task<List<Dictionary<string, AttributeValue>>> QueryTableAll(string tableName);
    }
}
