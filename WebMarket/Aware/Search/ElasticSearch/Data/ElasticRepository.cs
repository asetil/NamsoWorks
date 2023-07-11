using System;
using System.Collections.Generic;
using System.Linq;
using Aware.Search.ElasticSearch.Model;
using Aware.Util;
using Aware.Util.Log;
using Elasticsearch.Net;
using Nest;
using Aware.Util.Enums;

namespace Aware.Search.ElasticSearch.Data
{
    public class ElasticRepository : IElasticRepository
    {
        private ElasticClient _client;
        private ElasticStatus _status;

        private readonly ILogger _logger;

        public ElasticRepository(ILogger logger)
        {
            _logger = logger;
        }

        public ElasticStatus Status
        {
            get
            {
                try
                {
                    if (_status == ElasticStatus.Check)
                    {
                        _status = ElasticStatus.NotAvailable;
                        if (Client != null)
                        {
                            var response = Client.Ping();
                            _status = response != null && response.IsValid ? ElasticStatus.Active : ElasticStatus.NotAvailable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _status = ElasticStatus.NotAvailable;
                }
                return _status;
            }
        }

        public bool CreateIndex(string indexName)
        {

            if (!string.IsNullOrEmpty(indexName))
            {
                var index = Client.IndexExists(Indices.Parse(indexName));
                if (!index.Exists)
                {
                    var response = Client.CreateIndex(indexName, c => c
                        .Settings(s => s.NumberOfReplicas(0).NumberOfShards(5).Analysis(GetAnalysis).RefreshInterval(-1))
                        .Mappings(m => m.Map<ElasticProduct>(WithConfiguration)
                                        .Map<ElasticItem>(WithConfiguration)));

                    if (response == null || !response.IsValid)
                    {
                        throw new Exception(string.Format("CreateElasticIndex Error Type:{0}", indexName));
                    }

                    Client.Flush(Indices.All);
                    return response.IsValid;
                }
                return true;
            }
            return false;
        }

        public bool DeleteIndex(string indexName, bool deleteAll = false)
        {
            if (!string.IsNullOrEmpty(indexName))
            {
                var result = Client.DeleteIndex(indexName);
                return result.IsValid;
            }

            if (deleteAll)
            {
                var result = Client.DeleteIndex(Indices.All);
                return result.IsValid;
            }
            return false;
        }

        public bool IndexExists(string indexName)
        {
            var result = Client.IndexExists(Indices.Parse(indexName));
            return result != null && result.Exists;
        }

        public T Get<T>(int id) where T : class
        {
            var response = Client.Get<T>(id);
            return response.Source;
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            ISearchResponse<T> searchResults = Client.Search<T>(s => s.MatchAll());
            return searchResults.Documents;
        }

        public IEnumerable<T> GetMany<T>(IEnumerable<int> idList) where T : class
        {
            var response = Client.GetMany<T>(idList.Select(i => i.ToString()));
            return response.Select(i => i.Source);
        }

        public IIndexResponse Index<T>(T entity) where T : class
        {
            return Client.Index(entity);
        }

        public IBulkResponse InsertMany<T>(IEnumerable<T> entityList, string indexName, int bulkSize = 1000, bool isBulk = false) where T : class
        {
            if (entityList == null || !entityList.Any())
            {
                return null;
            }

            if (bulkSize == 0)
            {
                return Client.IndexMany(entityList, indexName);
            }

            List<T> list;
            var pageNumber = 0;
            IBulkResponse response = null;
            do
            {
                list = new List<T>(0);
                try
                {
                    list = entityList.Skip(pageNumber * bulkSize).Take(bulkSize).ToList();
                    if (list.Any())
                    {
                        if (isBulk)
                        {
                            response = Client.Bulk(b => b
                                .IndexMany(entityList, (a, i) => a.Index(indexName))
                                .Refresh(true));
                        }
                        else
                        {
                            response = Client.IndexMany(list, indexName);
                            Refresh(indexName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("ElasticRepository > InsertMany - Failed", ex);
                }

                pageNumber++;
            } while (list.Any());

            Refresh(indexName);
            return response;
        }

        public IUpdateResponse<T> Update<T>(T entity) where T : class
        {
            var path = new DocumentPath<T>(entity);
            return Client.Update(path, f => f.RetryOnConflict(3).Refresh());
        }

        public IDeleteResponse Delete<T>(int id) where T : class
        {
            return Client.Delete<T>(id);
        }

        public IDeleteResponse Delete<T>(T entity) where T : class
        {
            var path = new DocumentPath<T>(entity);
            return Client.Delete(path);
        }

        public IBulkResponse DeleteMany<T>(IEnumerable<T> entityList) where T : class
        {
            return Client.DeleteMany(entityList);
        }

        public void DeleteAll<T>() where T : class
        {
            var request = new DeleteByQueryRequest(Indices.All, Types.Type<T>());
            Client.DeleteByQuery(request);
        }

        public ISearchResponse<T> Find<T>(ISearchRequest searchDescriptor) where T : class
        {
            return Client.Search<T>(searchDescriptor);
        }

        public void Refresh(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                Client.Refresh(Indices.All);
            }
            else
            {
                Client.Refresh(Indices.Parse(indexName));
            }
        }

        private ElasticClient Client
        {
            get { return _client ?? (_client = Connect()); }
        }

        private ElasticClient Connect()
        {
            try
            {
                var isDebugMode = Config.IsDebugMode;
                var uri = new Uri(Config.ElasticUrl);
                var pool = new SingleNodeConnectionPool(uri);
                var settings = new ConnectionSettings(pool);
                settings.DisableDirectStreaming(isDebugMode)
                    .PrettyJson(isDebugMode)
                   .ThrowExceptions();

                var client = new ElasticClient(settings);
                var pingResult = client.Ping();
                if (pingResult.IsValid)
                {
                    _status = ElasticStatus.Active;
                }
                return client;
            }
            catch (Exception ex)
            {
                _status = ElasticStatus.NotAvailable;
                _logger.Error("ElasticHelper > Connect - failed", ex);
            }
            return null;
        }

        #region Helper
        private ITypeMapping WithConfiguration<TX>(TypeMappingDescriptor<TX> arg) where TX : class
        {
            return arg.AutoMap(3)
                      .AllField(a => a.Enabled(false))
                      .Dynamic(false)
                      .DateDetection()
                      .NumericDetection()
                      .DynamicDateFormats(new[] { "dateOptionalTime", "yyyy-MM-dd'T'HH:mm:ss.SSS" });
        }

        private static AnalysisDescriptor GetAnalysis(AnalysisDescriptor analysisDescriptor)
        {
            return analysisDescriptor.Analyzers(GetAnalyzers).CharFilters(GetCharFilters).TokenFilters(GetTokenFilters);
        }

        private static IPromise<ITokenFilters> GetTokenFilters(TokenFiltersDescriptor arg)
        {
            return arg.Stop("stopword_turkish", f => f.StopWords(new List<string> { "acaba", "altmis", "alti", "ama", "ancak", "arada", "aslinda", 
                        "ayrica", "bana", "bazi", "belki", "ben", "benden", "beni", "benim", "beri", "bes", "bile", "bin", "bir", "bircok", "biri", "birkac", 
                        "birkez", "birsey", "birseyi", "biz", "bize", "bizden", "bizi", "bizim", "boyle", "boylece", "bu", "buna", "bunda", "bundan", "bunlar", 
                        "bunlari", "bunlarin", "bunu", "bunun", "burada", "cok", "cunku", "da", "daha", "dahi", "de", "defa", "degil", "diger", "diye", "doksan", 
                        "dokuz", "dolayi", "dolayisiyla", "dort", "edecek", "eden", "ederek", "edilecek", "ediliyor", "edilmesi", "ediyor", "eger", "elli", "en", 
                        "etmesi", "etti", "ettigi", "ettigini", "gibi", "gore", "halen", "hangi", "hatta", "hem", "henuz", "hep", "hepsi", "her", "herhangi", 
                        "herkesin", "hic", "hicbir", "icin", "iki", "ile", "ilgili", "ise", "iste", "itibaren", "itibariyle", "kadar", "karsin", "katrilyon", 
                        "kendi", "kendilerine", "kendini", "kendisi", "kendisine", "kendisini", "kez", "ki", "kim", "kimden", "kime", "kimi", "kimse", "kirk", 
                        "milyar", "milyon", "mu", "mu", "mi", "nasil", "ne", "neden", "nedenle", "nerde", "nerede", "nereye", "niye", "nicin", "o", "olan", 
                        "olarak", "oldu", "oldugu", "oldugunu", "olduklarini", "olmadi", "olmadigi", "olmak", "olmasi", "olmayan", "olmaz", "olsa", "olsun", 
                        "olup", "olur", "olursa", "oluyor", "on", "ona", "ondan", "onlar", "onlardan", "onlari", "onlarin", "onu", "onun", "otuz", "oysa", "oyle", 
                        "pek", "ragmen", "sadece", "sanki", "sekiz", "seksen", "sen", "senden", "seni", "senin", "siz", "sizden", "sizi", "sizin", "sey", "seyden", 
                        "seyi", "seyler", "soyle", "su", "suna", "sunda", "sundan", "sunlari", "sunu", "tarafindan", "trilyon", "tum", "uc", "uzere", "var", "vardi", 
                        "ve", "veya", "ya", "yani", "yapacak", "yapilan", "yapilmasi", "yapiyor", "yapmak", "yapti", "yaptigi", "yaptigini", "yaptiklari", "yedi", 
                        "yerine", "yetmis", "yine", "yirmi", "yoksa", "yuz", "zaten" }.ToArray()))
                    .Stemmer("stemmer_turkish", f => f.Language("turkish"));
        }

        private static IPromise<ICharFilters> GetCharFilters(CharFiltersDescriptor arg)
        {
            return arg.Mapping("turkish_mapping", f => f.Mappings(new List<string> { "ı => i", "ü => u", "ş => s", "ç => c", "ğ => g", "ö => o" }))
                      .PatternReplace("custom_replace_char_filter", f => f.Pattern(@"[^a-zA-Z0-9ıçöşğüİIÇÖŞĞÜ,]").Replacement(string.Empty));
        }

        private static IPromise<IAnalyzers> GetAnalyzers(AnalyzersDescriptor arg)
        {
            return arg.Custom("turkish_stemmer_custom", f => f
                            .Filters(new List<string> { "asciifolding", "lowercase", "stopword_turkish", "stemmer_turkish" })
                            .CharFilters(new List<string> { "html_strip", "turkish_mapping" })
                            .Tokenizer("whitespace"))

                      .Custom("turkish_custom", f => f
                            .Filters(new List<string> { "asciifolding", "lowercase", "stopword_turkish" })
                            .CharFilters(new List<string> { "html_strip", "turkish_mapping" })
                            .Tokenizer("whitespace"))

                      .Custom("sort_custom", f => f
                            .Filters(new List<string> { "asciifolding", "lowercase" })
                            .CharFilters(new List<string> { "html_strip", "turkish_mapping", "custom_replace_char_filter" })
                            .Tokenizer("standard"))

                      .Custom("exact_custom", f => f
                            .Filters(new List<string> { "asciifolding", "lowercase" })
                            .CharFilters(new List<string> { "html_strip", "turkish_mapping", "custom_replace_char_filter" })
                            .Tokenizer("standard"));
        }
        #endregion
    }
}
