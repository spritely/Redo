# Redo
Provides .NET exception handling retry logic via a fluent interface.

[![Build status](https://ci.appveyor.com/api/projects/status/u36afr8go5b9wgd8?svg=true)](https://ci.appveyor.com/project/Spritely/redo)

[![NuGet Status](http://nugetstatus.com/Spritely.Redo.png)](http://nugetstatus.com/packages/Spritely.Redo)

Let's get right to it:

```csharp
Try.Running(() => myObject.CallThatThrowsSometimes())
   .Until(value => value == "success");

// This will run the myObject.CallThatThrowsSometimes and catch all exceptions that are thrown,
// pause and retry until the Until lambda expression returns true. Until receives the value
// returned from myObject.CallThatThrowsSometimes() so you can make keep going if it hasn't
// returned the value you were expecting yet.

// If you find yourself just needing to make sure a value isn't null:
Try.Running(() => myObject.CallThatThrowsSometimes())
   .Until(value => value != null);

// You can write it more concisely like this:
Try.Running(() => myObject.CallThatThrowsSometimes())
   .UntilNotNull();

// This assumes that myObject.CallThatThrowsSomtimes() returns a value (i.e. is a Func<T>)
// If you have a void return type, it's no problem, but you can't use UntilNotNull because
// there is nothing to check for against null. You can still use Until though, but you'll
// be checking some other property as follows:
Try.Running(() => myObject.CallThatThrowsSometimes())
   .Until(() => myObject.WasExpectationMet());

try
{
    myCall.ThatThrowsSometimes()
}
catch (Exception ex)
{
    // This isn't what you always want is it?
}

// So we can do this instead
Try.Running(() => File.OpenRead("SomeoneElseCreatesMe.txt"))
   .Handle<FileNotFoundException>()
   .UntilNotNull();
   
// Now we will only handle FileNotFoundExceptions, all other exceptions will continue to be thrown.

// More than one type to catch? No problem...

Try.Running(() => File.OpenRead("SomeoneElseCreatesMe.txt"))
   .Handle<FileNotFoundException>()
   .Handle<DirectoryNotFoundException>()
   .UntilNotNull();

// Want to log the exceptions that occur?

Try.Running(() => File.OpenRead("SomeoneElseCreatesMe.txt"))
   .Report(ex => MyLogger.Write(ex))
   .UntilNotNull();

// Okay, so that's all well and good, what about if you want to control the retry strategy?

Try.Running(() => File.OpenRead("SomeoneElseCreatesMe.txt"))
   .With(new ConstantDelayRetryStrategy(maxRetries: 3, delay: TimeSpan.FromSeconds(30))
   .UntilNotNull();
   
// There are several different kinds of pre-built strategies available:
Try.Running(() => File.OpenRead("SomeoneElseCreatesMe.txt"))
   .With(new LinearDelayRetryStrategy(scaleFactor: 100, maxRetries: 3, delay: TimeSpan.FromMilliseconds(100))
   .UntilNotNull();

// Linear adds the scaleFactor to the total delay each retry

// Progressive uses scaleFactor to multiple the delay with each retry
Try.Running(() => File.OpenRead("SomeoneElseCreatesMe.txt"))
   .With(new ProgressiveDelayRetryStrategy(scaleFactor: 10, maxRetries: 3, delay: TimeSpan.FromMilliseconds(100))
   .UntilNotNull();
   
// Exponetial uses a function like scaleFactor^attempt * delay to exponentially decay with each retry
Try.Running(() => File.OpenRead("SomeoneElseCreatesMe.txt"))
   .With(new ExponentialDelayRetryStrategy(scaleFactor: 2, maxRetries: 3, delay: TimeSpan.FromMilliseconds(100))
   .UntilNotNull();

// You can also create your own decay strategy by implementing this simple interface:
public interface IRetryStrategy
{
    bool ShouldQuit(long attempt);

    void Wait(long attempt);
}

// If you want to globally alter default settings for Try, modify them using TryDefault (the values here are the defaults):

TryDefault.MaxRetries = 30;
TryDefault.Delay = TimeSpan.FromSeconds(1);
TryDefault.RetryStrategy = new ConstantDelayRetryStrategy();

// This is empty by default
TryDefault.ExceptionListeners.Add(ex => MyLogger.Write(ex));

TryDefault.AddHandle<Exception>();
```
