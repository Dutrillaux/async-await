using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
    public class DummyDelayResource
    {
        public async Task ReadFileAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] ReadFile - start");
            await Task.Delay(2000);
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] ReadFile - end");
            await Task.FromResult(string.Empty);
        }

        public async Task<int> GetRandomNumberAsync()
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] GetRandomNumber - start");
            await Task.Delay(2000);
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] GetRandomNumber - end");
            return new Random().Next();
        }

        public async Task<string> HttpPostAsync(string message)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] HttpPost - start");
            await Task.Delay(2000);
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] HttpPost - end");
            return string.IsNullOrEmpty(message) ? "<RIEN>" : message.ToUpper();
        }

        public async Task<string> WithExceptionAsync(string message, int delay = 0)
        {
            Console.WriteLine($"Thread id:[{Thread.CurrentThread.ManagedThreadId}] - [{DateTime.Now}] Exception {message} in {delay}");
            if (delay > 0)
            {
                await Task.Delay(delay);
            }
            throw new Exception("sblah !" + message);
            await Task.Delay(100);
            Console.WriteLine("should never happens", Thread.CurrentThread.ManagedThreadId, DateTime.Now);
        }
    }
}