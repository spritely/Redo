# Redo
Provides .NET exception handling retry logic via a fluent interface in a recipe format (source code install).

[![Build status](https://ci.appveyor.com/api/projects/status/u36afr8go5b9wgd8?svg=true)](https://ci.appveyor.com/project/Spritely/redo)

[![NuGet Status](http://nugetstatus.com/Spritely.Redo.png)](http://nugetstatus.com/packages/Spritely.Redo)

Let's get right to it:

```csharp
// Run myObject.CallThatThrowsSometimes() and retry every 5 seconds if it throws any exceptions.
Using.ConstantBackOff(TimeSpan.FromSeconds(5))
    .Run(() => myObject.CallThatThrowsSometimes())
    .Now();

// Query database retrying every 5 seconds, only catch SqlException, and return the query results.
// This will only catch SqlException.
var users = Using.ConstantBackOff(TimeSpan.FromSeconds(5))
    .RetryOn<SqlException>()
    .Run(() => myDb.GetUsers())
    .Now();

// Retry query on any SqlExceptions until the results are not null.
var users = Using.ConstantBackOff(TimeSpan.FromSeconds(5))
    .RetryOn<SqlException>()
    .Run(() => myDb.GetUsers())
    .UntilNotNull()
    .Now();

// Use RunAsync instead of Run for asynchronous code
var users = await Using.ConstantBackOff(TimeSpan.FromSeconds(5))
    .RetryOn<SqlException>()
    .RunAsync(() => myDb.GetUsersAsync())
    .UntilNotNull()
    .Now();

// Query until the results are not null and some users are returned. Until recieves the value
// returned from the Run call and true terminates the execution while false continues retrying.
var users = Using.ConstantBackOff(TimeSpan.FromSeconds(5))
    .RetryOn<SqlException>()
    .Run(() => myDb.GetUsers())
    .UntilNotNull()
    .Until(users => users.Any())
    .Now();

// Now we will only handle FileNotFoundExceptions, all other exceptions will continue to be thrown
// and the wait time between tries will be 500ms, 1000ms, 1500ms, 2000ms, etc.
var file = Using.LinearBackOff(TimeSpan.FromMilliseconds(500))
    .RetryOn<FileNotFoundException>()
    .Run(() => File.OpenRead("SomeFile.txt"))
    .Now();

// Set the maximum number of retries to 5
// Executes up to 6 times: initial try and 5 waits and retries
var file = Using.LinearBackOff(TimeSpan.FromMilliseconds(500))
    .WithMaxRetries(5)
    .RetryOn<FileNotFoundException>()
    .Run(() => File.OpenRead("SomeFile.txt"))
    .Now();

// Handle more than one type of exception but explicitly throw others. FileNotFoundException derives
// from IOException so it would be retried except that throw on takes priority over retry on.
var file = Using.LinearBackOff(TimeSpan.FromMilliseconds(500))
    .RetryOn<IOException>()
    .RetryOn<UnauthorizedAccessException>()
    .ThrowOn<FileNotFoundException>
    .Run(() => File.OpenRead("SomeFile.txt"))
    .Now();

// Want to log the exceptions that occur while retrying?
var file = Using.LinearBackOff(TimeSpan.FromMilliseconds(500))
    .WithReporter(ex => MyLogger.Write(ex))
    .Run(() => File.OpenRead("SomeFile.txt"))
    .Now();

// Linear back-off with different delta 500ms, 600ms (500 + 100), 700ms (500 + 200), 800ms (500 + 300), etc.
var file = Using.LinearBackOff(TimeSpan.FromMilliseconds(500), delta: TimeSpan.FromMilliseconds(100))
    .Run(() => File.OpenRead("SomeFile.txt"))
    .Now();

// Progressive back-offs 100ms, 200ms (100 + 100 * 1), 300ms (100 + 100 * 2), 400ms (100 + 100 * 3), etc.
var file = Using.ProgressiveBackOff(TimeSpan.FromMilliseconds(100))
    .Run(() => File.OpenRead("SomeFile.txt"))
    .Now();

// Progressive is not the same as Linear. It looks like linear above because the scaleFactor defaults to 1.
// 100ms, 1100ms (100 + 100 * 10), 2100ms (100 + 100 * 20), 3100ms (100 + 100 * 30), etc.
var file = Using.ProgressiveBackOff(TimeSpan.FromMilliseconds(100), scaleFactor: 10)
    .Run(() => File.OpenRead("SomeFile.txt"))
    .Now();

// Expenential: 10ms, 30ms (10 + 10 * 2), 50ms (10 + 10 * 4), 90ms (10 + 10 * 8), 170ms (10 + 10 * 16)
var file = Using.ExponentialBackOff(TimeSpan.FromMilliseconds(10), scaleFactor: 2 /* this is the default */)
    .Run(() => File.OpenRead("SomeFile.txt"))
    .Now();
