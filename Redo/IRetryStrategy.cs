namespace Spritely.Redo
{
    public interface IRetryStrategy
    {
        bool ShouldQuit();

        void Wait();
    }
}
