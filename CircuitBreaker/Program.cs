using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    class Program
    {
        //http://patrickdesjardins.com/blog/how-to-create-a-simple-circuit-breaker-in-c
        //https://msdn.microsoft.com/en-us/library/dn589784.aspx

        static void Main(string[] args)
        {
            // Set the threshold for the circuit breaker, aka. number of consecutive failures before the circuit is "flipped".
            int threshold = 4;

            // Set the timeout for the circuit breaker, aka. the time to wait before the circuit breaker flips from Open to Half-Closed
            TimeSpan timeout = TimeSpan.FromSeconds(15);

            StartWork(threshold, timeout).GetAwaiter().GetResult();

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        public static async Task StartWork(int threshold, TimeSpan timeout)
        {
            CircuitBreaker circuitBreaker = new CircuitBreaker(threshold, timeout);
            DateTime processStart = DateTime.UtcNow;
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();
            bool ok = false;
            int maxRetryTimeSeconds = 60 * 5;
            int count = 0;

            while (!ok && elapsed.Elapsed.TotalSeconds < maxRetryTimeSeconds)
            {
                ok = await circuitBreaker.AttemptCall(async () =>
                {
                    await SendMessage(count++);
                });

                if (ok)
                {
                    Console.WriteLine("Message was sent ok!");
                }
                else if (circuitBreaker.IsOpen)
                {
                    Console.WriteLine("Send failure. The circuit breaker is open, so wait until the timeout has elapsed.");
                    await Task.Delay((int)circuitBreaker.TimeoutRemaining() + 1);
                }
                else
                {
                    Console.WriteLine("Send failure. The circuit breaker is closed so try again shortly.");
                    await Task.Delay(50);
                }
            }
        }

        public static async Task SendMessage(int iteration)
        {
            Console.Write("Sending message...");
            await Task.Delay(3000);

            if (iteration < 6)
            {
                Console.WriteLine("timeout.");
                throw new Exception("Error when sending message!");
            }
            else
                Console.WriteLine("OK.");
        }

    }
}
