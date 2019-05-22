
namespace perf
{
    public interface IServiceManager
    {
        string UserAgent  { get; set; }
        void   SendPayload(Payload payload);
    }
}