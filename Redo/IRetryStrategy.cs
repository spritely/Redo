namespace Spritely.Redo
{
    public interface IRetryStrategy
    {
        bool ShouldQuit(long attempt);
        void Wait(long attampt);
    }
}
