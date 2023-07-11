namespace Worchart.BL.Model
{
    public interface IEntity
    {
        int ID { get; set; }

        bool IsValid();
    }
}
