using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
    internal class AsyncMethods
    {
        public enum TestIds
        {
            SequentialImplementationAsync,
            SequentialImplementationWithReturnValueAsync,
            ParallelizationAttemptAsync,
            FullParallelizationAsync,
            PrettyFullParallelizationAsync,
            PrettyFullParallelizationWithTimeOutAsync,
            FireAndForget,
            FireAndForgetAsync,
            FullParallelizationWithExceptionAsync,
            FullParallelizationWithMultipleExceptionAsync,
            FullParallelizationWithMultipleExceptionWithoutCatchAsync,
            ConfigureAwaitFalseAsync,
            ConfigureAwaitTrueAsync,
            ConfigureAwaitTrueWithAwaitAsync,
            ConfigureAwaitFalseWithAwaitAsync
        }

        public async void Run(TestIds testId)
        {
            try
            {
                var start = DateTime.Now;
                Console.WriteLine($"[{start}] DEBUT");

                string result;
                switch (testId)
                {
                    case TestIds.SequentialImplementationAsync:
                        result = await SequentialImplementationAsync("test0");
                        break;
                    case TestIds.SequentialImplementationWithReturnValueAsync:
                        result = await SequentialImplementationWithReturnValueAsync("test1");
                        break;
                    case TestIds.ParallelizationAttemptAsync:
                        result = await ParallelizationAttemptAsync("test2");
                        break;
                    case TestIds.FullParallelizationAsync:
                        result = await FullParallelizationAsync("test2");
                        break;
                    case TestIds.PrettyFullParallelizationAsync:
                        result = await PrettyFullParallelizationAsync("test2");
                        break;
                    case TestIds.PrettyFullParallelizationWithTimeOutAsync:
                        result = await PrettyFullParallelizationWithTimeOutAsync("");
                        break;
                    case TestIds.FireAndForget:
                        result = FireAndForget("");
                        break;
                    case TestIds.FireAndForgetAsync:
                        result = await FireAndForgetAsync("");
                        break;
                    case TestIds.FullParallelizationWithExceptionAsync:
                        result = await FullParallelizationWithExceptionAsync();
                        break;
                    case TestIds.FullParallelizationWithMultipleExceptionAsync:
                        result = await FullParallelizationWithMultipleExceptionAsync();
                        break;
                    case TestIds.FullParallelizationWithMultipleExceptionWithoutCatchAsync:
                        result = await FullParallelizationWithMultipleExceptionWithoutCatchAsync();
                        break;
                    case TestIds.ConfigureAwaitFalseAsync:
                        result = await ConfigureAwaitFalseAsync();
                        break;
                    case TestIds.ConfigureAwaitTrueAsync:
                        result = await ConfigureAwaitTrueAsync();
                        break;
                    case TestIds.ConfigureAwaitTrueWithAwaitAsync:
                        result = await ConfigureAwaitTrueWithAwaitAsync();
                        break;
                    case TestIds.ConfigureAwaitFalseWithAwaitAsync:
                        result = await ConfigureAwaitFalseWithAwaitAsync();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(testId), testId, null);
                }

                var end = DateTime.Now;
                Console.WriteLine($"[{end}] Sortie: {result}");
                Console.WriteLine($"[{end}] TOUTES LES TACHES SONT TERMINEES - Temps global: {end - start}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private Task<string> ConfigureAwaitTrueAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting ConfigureAwaitTrueAsync Implementation ...");

            var resource = new DummyDelayResource();
            resource.ReadFileAsync().ConfigureAwait(true);
            resource.GetRandomNumberAsync().ConfigureAwait(true);

            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] End ConfigureAwaitTrueAsync Implementation.");
            return Task.FromResult("As Fire and Forget ...");
        }

        private Task<string> ConfigureAwaitFalseAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting ConfigureAwaitFalseAsync Implementation ...");

            var resource = new DummyDelayResource();
            resource.ReadFileAsync().ConfigureAwait(false);
            resource.GetRandomNumberAsync().ConfigureAwait(false);

            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] End ConfigureAwaitFalseAsync Implementation.");
            return Task.FromResult("As Fire and Forget ...");
        }

        private async Task<string> ConfigureAwaitTrueWithAwaitAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting ConfigureAwaitTrueWithAwaitAsync Implementation ...");

            var resource = new DummyDelayResource();
            await resource.ReadFileAsync().ConfigureAwait(true);
            await resource.GetRandomNumberAsync().ConfigureAwait(true);

            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] End ConfigureAwaitTrueWithAwaitAsync Implementation.");
            return string.Empty;
        }

        private async Task<string> ConfigureAwaitFalseWithAwaitAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting ConfigureAwaitFalseWithAwaitAsync Implementation ...");

            var resource = new DummyDelayResource();
            await resource.ReadFileAsync().ConfigureAwait(false);
            await resource.GetRandomNumberAsync().ConfigureAwait(false);

            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] End ConfigureAwaitFalseWithAwaitAsync Implementation.");
            return string.Empty;
        }

        private async Task<string> FullParallelizationWithExceptionAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting FullParallelizationWithExceptionAsync Implementation ...");

            var resource = new DummyDelayResource();
            await resource.ReadFileAsync();
            await resource.GetRandomNumberAsync();
            await resource.WithExceptionAsync("unique");

            Console.WriteLine($"[{DateTime.Now}] End FullParallelizationWithExceptionAsync Implementation.");
            return string.Empty;
        }

        private async Task<string> FullParallelizationWithMultipleExceptionAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting FullParallelizationWithMultipleExceptionAsync Implementation ...");

            var resource = new DummyDelayResource();
            var descriptors = new List<Task>
            {
                resource.WithExceptionAsync("1", 1000),
                resource.WithExceptionAsync("2",1),
                resource.ReadFileAsync(),
                resource.GetRandomNumberAsync()
            };

            try
            {
                await Task.WhenAll(descriptors);
            }
            catch (Exception)
            {
                foreach (var t in descriptors)
                {
                    if (!t.IsFaulted && t.IsCanceled) continue;

                    if (t.IsFaulted)
                        Console.WriteLine("------------------ FAULTED -------------------");
                    if (t.IsCanceled)
                        Console.WriteLine("------------------ CANCELLED -------------------");

                    if (t.Exception != null) Console.WriteLine(t.Exception.ToString());
                }

                throw;
            }

            Console.WriteLine($"[{DateTime.Now}] End FullParallelizationWithMultipleExceptionAsync Implementation.");

            return string.Empty;
            /*
                Thread id:[9] - [17/01/2017 13:39:59] Starting FullParallelizationWithMultipleExceptionAsync Implementation ...
                Thread id:[9] - [17/01/2017 13:39:59] Exception 1 in 1000
                Thread id:[9] - [17/01/2017 13:39:59] Exception 2 in 1
                Thread id:[9] - [17/01/2017 13:39:59] ReadFile - start
                Thread id:[9] - [17/01/2017 13:39:59] GetRandomNumber - start
                Thread id:[9] - [17/01/2017 13:40:07] GetRandomNumber - end
                Thread id:[9] - [17/01/2017 13:40:07] ReadFile - end
                ------------------  FAULTED -------------------
                System.AggregateException: Une ou plusieurs erreurs se sont produites. ---> System.Exception: sblah !1
                   à AsyncAwait.DummyDelayResource.<WithExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\DummyDelayResource.cs:ligne 44
                --- Fin de la trace de la pile à partir de l'emplacement précédent au niveau duquel l'exception a été levée ---
                   à System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.GetResult()
                   à AsyncAwait.AsyncMethods.<FullParallelizationWithMultipleExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\AsyncMethods.cs:ligne 112
                   --- Fin de la trace de la pile d'exception interne ------> (Exception interne #0) System.Exception: sblah !1
                   à AsyncAwait.DummyDelayResource.<WithExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\DummyDelayResource.cs:ligne 44
                --- Fin de la trace de la pile à partir de l'emplacement précédent au niveau duquel l'exception a été levée ---
                   à System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.GetResult()
                   à AsyncAwait.AsyncMethods.<FullParallelizationWithMultipleExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\AsyncMethods.cs:ligne 112<---

                ------------------  FAULTED -------------------
                System.AggregateException: Une ou plusieurs erreurs se sont produites. ---> System.Exception: sblah !2
                   à AsyncAwait.DummyDelayResource.<WithExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\DummyDelayResource.cs:ligne 44
                   --- Fin de la trace de la pile d'exception interne ---
                ---> (Exception interne #0) System.Exception: sblah !2
                   à AsyncAwait.DummyDelayResource.<WithExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\DummyDelayResource.cs:ligne 44<---

                ------------------  FAULTED -------------------
                ------------------  FAULTED -------------------
                System.Exception: sblah !1
                   à AsyncAwait.DummyDelayResource.<WithExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\DummyDelayResource.cs:ligne 44
                --- Fin de la trace de la pile à partir de l'emplacement précédent au niveau duquel l'exception a été levée ---
                   à System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.GetResult()
                   à AsyncAwait.AsyncMethods.<FullParallelizationWithMultipleExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\AsyncMethods.cs:ligne 125
                --- Fin de la trace de la pile à partir de l'emplacement précédent au niveau duquel l'exception a été levée ---
                   à System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
                   à AsyncAwait.AsyncMethods.<Run>d__1.MoveNext() dans C:\temp\AsyncAwait.7z\AsyncMethods.cs:ligne 63
             */
        }

        private async Task<string> FullParallelizationWithMultipleExceptionWithoutCatchAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting FullParallelizationWithMultipleExceptionWithoutCatchAsync Implementation ...");

            var resource = new DummyDelayResource();
            var descriptors = new List<Task>
            {
                resource.WithExceptionAsync("1", 1000),
                resource.WithExceptionAsync("2",1),
                resource.ReadFileAsync(),
                resource.GetRandomNumberAsync()
            };


            await Task.WhenAll(descriptors);

            Console.WriteLine($"[{DateTime.Now}] End FullParallelizationWithMultipleExceptionAsync Implementation.");

            return string.Empty;

            /*
                Thread id:[9] - [17/01/2017 13:37:45] Starting FullParallelizationWithMultipleExceptionWithoutCatchAsync Implementation ...
                Thread id:[9] - [17/01/2017 13:37:45] Exception 1 in 1000
                Thread id:[9] - [17/01/2017 13:37:45] Exception 2 in 1
                Thread id:[9] - [17/01/2017 13:37:45] ReadFile - start
                Thread id:[9] - [17/01/2017 13:37:45] GetRandomNumber - start
                Thread id:[9] - [17/01/2017 13:37:47] GetRandomNumber - end
                Thread id:[9] - [17/01/2017 13:37:47] ReadFile - end
                System.Exception: sblah !1
                   à AsyncAwait.DummyDelayResource.<WithExceptionAsync>d__3.MoveNext() dans C:\temp\AsyncAwait.7z\DummyDelayResource.cs:ligne 44
                --- Fin de la trace de la pile à partir de l'emplacement précédent au niveau duquel l'exception a été levée ---
                   à System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.GetResult()
                   à AsyncAwait.AsyncMethods.<FullParallelizationWithMultipleExceptionWithoutCatchAsync>d__4.MoveNext() dans C:\temp\AsyncAwait.7z\AsyncMethods.cs:ligne 148
                --- Fin de la trace de la pile à partir de l'emplacement précédent au niveau duquel l'exception a été levée ---
                   à System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                   à System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
                   à AsyncAwait.AsyncMethods.<Run>d__1.MoveNext() dans C:\temp\AsyncAwait.7z\AsyncMethods.cs:ligne 66
             */
        }

        private async Task<string> SequentialImplementationAsync(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting Sequential Implementation ...");

            var resource = new DummyDelayResource();
            await resource.ReadFileAsync();
            await resource.GetRandomNumberAsync();
            await resource.HttpPostAsync(message);

            Console.WriteLine($"[{DateTime.Now}] End Sequential Implementation.");
            return string.Empty;
        }

        private async Task<string> SequentialImplementationWithReturnValueAsync(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting Sequential Implementation With Return Value ...");

            var resource = new DummyDelayResource();
            await resource.ReadFileAsync();
            var number = await resource.GetRandomNumberAsync();
            var upper = await resource.HttpPostAsync(message);

            Console.WriteLine($"[{DateTime.Now}] End Sequential Implementation With Return Value.");
            return $"{number}-{upper}";
        }

        private async Task<string> ParallelizationAttemptAsync(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting Parallelization Attempt...");

            var resource = new DummyDelayResource();
            var emailTask = resource.ReadFileAsync();
            var number = await resource.GetRandomNumberAsync();
            var upper = await resource.HttpPostAsync(message);
            await emailTask;

            Console.WriteLine($"[{DateTime.Now}] End Parallelization Attempt.");
            return $"{number}-{upper}";
        }

        private async Task<string> FullParallelizationAsync(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting Full Parallelization...");

            var resource = new DummyDelayResource();
            var emailTask = resource.ReadFileAsync();
            var numberTask = resource.GetRandomNumberAsync();
            var upperTask = resource.HttpPostAsync(message);

            var number = await numberTask;
            var upper = await upperTask;
            await emailTask;

            Console.WriteLine($"[{DateTime.Now}] End Full Parallelization.");
            return default(string);
        }

        private async Task<string> PrettyFullParallelizationAsync(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting Full Parallelization...");

            var resource = new DummyDelayResource();

            var descriptors = new List<Task>
            {
                resource.ReadFileAsync(),
                resource.GetRandomNumberAsync(),
                resource.HttpPostAsync(message)
            };

            await Task.WhenAll(descriptors);

            Console.WriteLine($"[{DateTime.Now}] End Full Parallelization.");
            return default(string);
        }

        private async Task<string> PrettyFullParallelizationWithTimeOutAsync(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting Full Parallelization With Timeout 1 sec...");

            var resource = new DummyDelayResource();

            var descriptors = new List<Task>
            {
                resource.ReadFileAsync(),
                resource.GetRandomNumberAsync(),
                resource.HttpPostAsync(message)
            };

            await Task.WhenAny(Task.WhenAll(descriptors), Task.Delay(1000));

            Console.WriteLine($"[{DateTime.Now}] End Full Parallelization With Timeout 1 sec.");
            return default(string);
        }

        private async Task<string> FireAndForgetAsync(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting FireAndForget Async ...");

            var resource = new DummyDelayResource();
            await Task.Run(() => resource.ReadFileAsync());
            await Task.Run(() => resource.GetRandomNumberAsync());
            await Task.Run(() => resource.HttpPostAsync(message));

            Console.WriteLine($"[{DateTime.Now}] End FireAndForget Async.");
            return default(string);
        }

        private string FireAndForget(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Starting FireAndForget ...");

            var resource = new DummyDelayResource();
            Task.Run(() => resource.ReadFileAsync());
            Task.Run(() => resource.GetRandomNumberAsync());
            Task.Run(() => resource.HttpPostAsync(message));

            Console.WriteLine($"[{DateTime.Now}] End FireAndForget.");
            return default(string);
        }
    }
}