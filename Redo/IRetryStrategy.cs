namespace Spritely.ControlFlow
{
    public interface IRetryStrategy
    {
        bool ShouldQuit();

        void Wait();
    }
}
