using System;
using System.Threading.Tasks;

namespace AsyncAwait
{
    internal class Launcher
    {
        //public static event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private static void Main(string[] args)
        {
            //TaskScheduler.UnobservedTaskException += Handler;
            //AppDomain.CurrentDomain.UnhandledException += Handler;
            //UnobservedTaskException += Program_UnobservedTaskException;

            //TaskScheduler.UnobservedTaskException += OnTaskSchedulerOnUnobservedTaskException;

            try
            {
                var testIdSelected = AsyncMethods.TestIds.ConfigureAwaitFalseAsync;

                new AsyncMethods().Run(testIdSelected);
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

        //private static void OnTaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs eventArgs)
        //{
        //    eventArgs.SetObserved();
        //    eventArgs.Exception.Handle(ex =>
        //   {
        //       Console.WriteLine($"Exception type: {ex.GetType()}");
        //       return true;
        //   });
        //}

        //private static void Handler(Object sender, EventArgs e)
        //{
        //    Console.WriteLine("I'm so lonely, won't anyone call me?");
        //}

        //private static void Program_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        //{
        //    Console.WriteLine("toto");
        //}
        //    private static void OnUnobservedTaskException(UnobservedTaskExceptionEventArgs e)
        //    {
        //        UnobservedTaskException?.Invoke(null, e);
        //    }
        }
    }
