using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
    public class DummyDelayResource
    {
        public async Task ReadFileAsync()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] ReadFile - start", threadId, DateTime.Now);
            await Task.Delay(2000);
            Console.WriteLine("Thread id:[{0}] - [{1}] ReadFile - end", threadId, DateTime.Now);
            await Task.FromResult(string.Empty);
        }

        public async Task<int> GetRandomNumberAsync()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] GetRandomNumber - start", threadId, DateTime.Now);
            await Task.Delay(2000);
            Console.WriteLine("Thread id:[{0}] - [{1}] GetRandomNumber - end", threadId, DateTime.Now);
            return (new Random()).Next();
        }

        public async Task<string> HttpPostAsync(string message)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] HttpPost - start", threadId, DateTime.Now);
            await Task.Delay(2000);
            Console.WriteLine("Thread id:[{0}] - [{1}] HttpPost - end", threadId, DateTime.Now);
            return string.IsNullOrEmpty(message) ? "<RIEN>" : message.ToUpper();
        }

        public async Task<string> WithExceptionAsync(string message, int delay = 0)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread id:[{0}] - [{1}] Exception " + message + " in " + delay, threadId, DateTime.Now);
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            throw new Exception("sblah !" + message);
            await Task.Delay(100);
            Console.WriteLine("should never happens", threadId, DateTime.Now);
        }
    }
}