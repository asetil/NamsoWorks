namespace Aware.Search.ElasticSearch
{
    public interface IElasticBuilder
    {
        ElasticHelper<TX> GetElasticHelper<TX>() where TX:class;
        int Skip { get; }
        int Size { get; }
    }
}
