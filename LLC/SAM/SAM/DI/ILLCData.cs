using Amazon;
using Amazon.DynamoDBv2.Model;
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

        string TableCount(string tableName);

        List<SourceModel> Sources(string tableName, string bucketTableName);

        SourceModel Source(string tableName, string bucketTableName, string id);

        List<SettingModel> Settings(string tableName);

        List<InvalidLinksModel> InvalidLinks(string tableName);

        List<WarningLinksModel> WarningLinks(string tableName);

        void AddUrlToWarningLinks(List<WarningLinksModel> links, string tableName);

        List<BucketLocationsModel> BucketLocations(BucketLocationsRequest m, string objectLinksTable, string objectsTable, string bucketsTable, string statsTable);

        List<BucketsModel> Buckets(string tableName);

        void SubmitMetaTableQueue(string tableName, string key, int diff);

        Task<int> IncrementMetaTableKey(string tableName, string key, int diff);

        Task<string> QueryCountBool(string tableName, string column, bool b);

        Task<AttributeValue> QueryDataAttribute(string tableName, string key, string field);

        Task<string> QueryCountContains(string tableName, string column, string s);

        Task<List<Dictionary<string, AttributeValue>>> QueryTableAll(string tableName);
    }
}
