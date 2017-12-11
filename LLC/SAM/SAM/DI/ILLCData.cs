using Amazon;
using Amazon.S3.Model;
using DbCore.Models;
using SAM.Models.Auth;
using SAM.Models.Admin;
using SAM.Models.EF;
using SAM.Models.Reports;
using System.Collections.Generic;
using System.Security.Claims;

namespace SAM.DI
{
    public interface ILLCData
    {
        RegionEndpoint Region();

        User User(IEnumerable<Claim> claim);

        List<StatsExt> Stats();

        #region Objects

        int ObjectsCount(string bucket);

        Objects Object(string id);

        Objects ObjectFromKey(string key);

        Objects SetObject(Objects obj);

        List<ObjectsExt> LinkExtractor(string bucket, int offset, int maximum);

        #endregion

        #region Links

        int LinksCount(string source);

        Links Link(string id);

        List<ReportsExt> InvalidLinks();

        List<ReportsExt> WarningLinks();

        Links LinkFromUrl(string url);

        Links SetLink(Links link);

        List<LinksExt> LinkChecker(string source, int offset, int maximum);

        #endregion

        #region Sources

        List<SourcesExt> Sources();

        SourcesExt Source(string id, SearchType type);

        Save SaveSource(SourcesExt source);

        Save DeleteSource(SourcesExt source);

        #endregion

        #region Settings

        List<SettingsExt> Settings();

        SettingsExt Setting(string id, SearchType type);

        Save SaveSetting(SettingsExt setting, User user);

        Save DeleteSetting(SettingsExt setting);

        #endregion

        #region Queue

        void EnqueueObjects<T>(List<T> objects) where T : Models.Dynamo.ReceiptBase;

        List<T> DequeueObjects<T>() where T : Models.Dynamo.ReceiptBase;

        void RemoveObjectsFromQueue<T>(List<T> objects) where T : Models.Dynamo.ReceiptBase;

        bool QueueEmpty();

        #endregion

        List<Buckets> Buckets();

        List<BucketLocationsModel> BucketLocations(BucketLocationsRequest m);

        GetObjectResponse ObjectGet(string bucket, string key);

        PutObjectResponse ObjectPut<T>(string bucket, string key, T obj);
    }
}
