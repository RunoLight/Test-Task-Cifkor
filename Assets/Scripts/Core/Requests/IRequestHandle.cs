namespace Core.Requests
{
    public interface IRequestHandle
    {
        string Id { get; }
        bool IsCompleted { get; }
        bool IsCanceled { get; }
        void Cancel();
    }
}