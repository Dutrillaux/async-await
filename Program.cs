using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
    internal class Program
    {
        private enum TestIds
        {
            SequentialImplementationAsync = 0,
            SequentialImplementationWithReturnValueAsync = 1,
            ParallelizationAttemptAsync = 2,
            FullParallelizationAsync = 3,
            PrettyFullParallelizationAsync = 4,
            PrettyFullParallelizationWithTimeOutAsync = 5,
            FireAndForget = 6,
            FireAndForgetAsync = 7,
            FullParallelizationWithExceptionAsync = 8,
            FullParallelizationWithMultipleExceptionAsync = 9
        }

        public static event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private static void Main(string[] args)
        {
            //TaskScheduler.UnobservedTaskException += Handler;
            //AppDomain.CurrentDomain.UnhandledException += Handler;
            //            UnobservedTaskException += Program_UnobservedTaskException;

            TaskScheduler.UnobservedTaskException += OnTaskSchedulerOnUnobservedTaskException;

            try
            {
                Run(TestIds.FullParallelizationWithMultipleExceptionAsync);
                //Run(TestIds.PrettyFullParallelizationAsync);
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private static void OnTaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs eventArgs)
        {
            eventArgs.SetObserved();
            ((AggregateException)eventArgs.Exception).Handle(ex =>
           {
               Console.WriteLine("Exception type: {0}", ex.GetType());
               return true;
           });
        }

        private static void Handler(Object sender, EventArgs e)
        {
            Console.WriteLine("I'm so lonely, won't anyone call me?");
        }

        private static void Program_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine("toto");
        }

        private static async void Run(TestIds testId)
        {
            try
            {
                var start = DateTime.Now;
                Console.WriteLine("[{0}] DEBUT", start);
                string result = string.Empty;
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
                    default:
                        throw new ArgumentOutOfRangeException("testId", testId, null);
                }

                var end = DateTime.Now;
                Console.WriteLine("[{0}] Sortie: {1}", end, result);
                Console.WriteLine("[{0}] TOUTES LES TACHES SONT TERMINEES - Temps global: {1}", end, end - start);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task<string> FullParallelizationWithExceptionAsync()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting FullParallelizationWithExceptionAsync Implementation ...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();
            await resource.ReadFileAsync();
            await resource.GetRandomNumberAsync();
            await resource.WithExceptionAsync("unique");

            Console.WriteLine("[{0}] End FullParallelizationWithExceptionAsync Implementation.", DateTime.Now);
            return string.Empty;
        }

        private static async Task<string> FullParallelizationWithMultipleExceptionAsync()
        {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("Thread id:[{0}] - [{1}] Starting FullParallelizationWithMultipleExceptionAsync Implementation ...", threadId, DateTime.Now);

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
                    if (t.IsFaulted || !t.IsCanceled)
                    {
                        Console.WriteLine("------------------  FAULTED -------------------");

                        if (t.Exception != null) Console.WriteLine(t.Exception.ToString());
                    }
                }

                throw;
            }

            Console.WriteLine("[{0}] End FullParallelizationWithMultipleExceptionAsync Implementation.", DateTime.Now);

            return string.Empty;
        }

        static async Task<string> SequentialImplementationAsync(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting Sequential Implementation ...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();
            await resource.ReadFileAsync();
            await resource.GetRandomNumberAsync();
            await resource.HttpPostAsync(message);

            Console.WriteLine("[{0}] End Sequential Implementation.", DateTime.Now);
            return string.Empty;
        }

        static async Task<string> SequentialImplementationWithReturnValueAsync(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting Sequential Implementation With Return Value ...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();
            await resource.ReadFileAsync();
            var number = await resource.GetRandomNumberAsync();
            var upper = await resource.HttpPostAsync(message);

            Console.WriteLine("[{0}] End Sequential Implementation With Return Value.", DateTime.Now);
            return string.Format("{0}-{1}", number, upper);
        }

        static async Task<string> ParallelizationAttemptAsync(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting Parallelization Attempt...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();
            var emailTask = resource.ReadFileAsync();
            var number = await resource.GetRandomNumberAsync();
            var upper = await resource.HttpPostAsync(message);
            await emailTask;

            Console.WriteLine("[{0}] End Parallelization Attempt.", DateTime.Now);
            return string.Format("{0}-{1}", number, upper);
        }

        static async Task<string> FullParallelizationAsync(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting Full Parallelization...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();
            var emailTask = resource.ReadFileAsync();
            var numberTask = resource.GetRandomNumberAsync();
            var upperTask = resource.HttpPostAsync(message);

            var number = await numberTask;
            var upper = await upperTask;
            await emailTask;

            Console.WriteLine("[{0}] End Full Parallelization.", DateTime.Now);
            //return string.Format("{0}-{1}", number, upper);
            return default(string);
        }

        private static async Task<string> PrettyFullParallelizationAsync(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting Full Parallelization...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();

            var descriptors = new List<Task>
            {
                resource.ReadFileAsync(),
                resource.GetRandomNumberAsync(),
                resource.HttpPostAsync(message)
            };

            await Task.WhenAll(descriptors);

            Console.WriteLine("[{0}] End Full Parallelization.", DateTime.Now);
            return default(string);
        }

        private static async Task<string> PrettyFullParallelizationWithTimeOutAsync(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting Full Parallelization With Timeout 1 sec...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();

            var descriptors = new List<Task>
            {
                resource.ReadFileAsync(),
                resource.GetRandomNumberAsync(),
                resource.HttpPostAsync(message)
            };

            await Task.WhenAny(Task.WhenAll(descriptors), Task.Delay(1000));

            Console.WriteLine("[{0}] End Full Parallelization With Timeout 1 sec.", DateTime.Now);
            return default(string);
        }

        private static async Task<string> FireAndForgetAsync(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting FireAndForget Async ...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();
            await Task.Run(() => resource.ReadFileAsync());
            await Task.Run(() => resource.GetRandomNumberAsync());
            await Task.Run(() => resource.HttpPostAsync(message));

            Console.WriteLine("[{0}] End FireAndForget Async.", DateTime.Now);
            return default(string);
        }

        private static string FireAndForget(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Starting FireAndForget ...", threadId, DateTime.Now);

            var resource = new DummyDelayResource();
            Task.Run(() => resource.ReadFileAsync());
            Task.Run(() => resource.GetRandomNumberAsync());
            Task.Run(() => resource.HttpPostAsync(message));

            Console.WriteLine("[{0}] End FireAndForget.", DateTime.Now);
            return default(string);
        }
    }
}
