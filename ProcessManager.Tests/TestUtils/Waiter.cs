using System;
using System.Threading.Tasks;

namespace ProcessManager.Tests.TestUtils
{
    public class Waiter
    {
        public static async Task WaitAsync(Func<bool> predicate, TimeSpan timeout, TimeSpan step)
        {
            try
            {
                while (!predicate() || timeout.TotalMilliseconds > 0)
                {
                    await Task.Delay(step);
                    timeout = timeout.Subtract(step);
                }
            }
            catch
            {
                //no catch
            }
        }

        public static async Task WaitAsync(Func<bool> predicate, TimeSpan timeout)
        {
            await WaitAsync(predicate, timeout, TimeSpan.FromMilliseconds(100));
        }

        public static void Wait(Func<bool> predicate, TimeSpan timeout, TimeSpan step)
        {
            WaitAsync(predicate,timeout,step).Wait();
        }
        
        public static void Wait(Func<bool> predicate, TimeSpan timeout)
        {
            WaitAsync(predicate,timeout).Wait();
        }
    }
}