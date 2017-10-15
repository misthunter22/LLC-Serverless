using Amazon;
using Amazon.CloudWatch.Model;
using Amazon.DynamoDBv2.Model;
using Amazon.S3.Model;
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

        long TableCount(string tableName);

        List<SourceModel> Sources(string tableName, string bucketTableName);

        SourceModel Source(string tableName, string bucketTableName, string id, SourceSearchType type);

        List<SettingModel> Settings(string tableName);

        List<InvalidLinksModel> InvalidLinks(string tableName);

        List<WarningLinksModel> WarningLinks(string tableName);

        void AddUrlToWarningLinks(List<WarningLinksModel> links, string tableName);

        List<BucketLocationsModel> BucketLocations(BucketLocationsRequest m, string objectLinksTable, string objectsTable, string bucketsTable, string statsTable);

        List<BucketsModel> Buckets(string tableName);

        ListVersionsResponse ObjectVersions(string bucket, string key);

        GetObjectResponse ObjectGet(string bucket, string key);

        GetMetricStatisticsResponse BucketCount(string bucket);

        Task<long> IncrementMetaTableKey(string tableName, string key, long diff);

        Task<long> SetMetaTableKey(string tableName, string key, long set);

        Task<string> QueryCountBool(string tableName, string column, bool b);

        Task<AttributeValue> QueryDataAttribute(string tableName, string key, string field);

        Task<string> QueryCountContains(string tableName, string column, string s);

        Task<List<Dictionary<string, AttributeValue>>> QueryTableAll(string tableName);
    }
}
