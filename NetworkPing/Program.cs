using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace NetworkPing
{
    class Program
    {
        private static System.Timers.Timer jobbTimer;
        private static double timerSchedule;
        private static Ping ping;
        private static Serilog.Core.Logger log = new LoggerConfiguration().WriteTo.Console(Serilog.Events.LogEventLevel.Verbose).WriteTo.File(@"D:\NetLog\logMyGear.txt", Serilog.Events.LogEventLevel.Warning).CreateLogger();


        static async Task Main(string[] args)
        {
            log.Error("Starting");
            ping = new Ping();
            timerSchedule = 1000;
            jobbTimer = new System.Timers.Timer(timerSchedule);
            jobbTimer.Elapsed += ExicuteQueWork;
            jobbTimer.AutoReset = false;
            jobbTimer.Start();

            while (true)
            {
                //Keep program
            }
            log.Debug("close program");
        }
        private static async void ExicuteQueWork(object sender, ElapsedEventArgs e)
        {
            bool didGoCompleteJobb = false;
            try
            {
                didGoCompleteJobb = Ping();
            }
            catch (Exception ex)
            {
                didGoCompleteJobb = true;
                log.Fatal(ex,$"failed - try catch");
            }
            finally
            {
                //Start timer again to trigger jobb
                jobbTimer.Interval = didGoCompleteJobb ? timerSchedule : 10;
                jobbTimer.Start();
            }
        }

        private static bool Ping(bool hasFailedBefore = false)
        {
            bool didGoCompleteJobb;
            PingReply pingStatus = ping.Send("google.com");

            if (pingStatus.Status == IPStatus.Success)
            {
                if (hasFailedBefore)
                {
                    log.Fatal("Ok", pingStatus.Status);
                }
                else
                {
                    log.Information("ok");
                }
                didGoCompleteJobb = true;
            }
            else
            {
                log.Fatal($"failed Status:{pingStatus.Status} - RoundtripTime:{pingStatus?.RoundtripTime ?? 0} - Buffer:{pingStatus?.Buffer?.Count() ?? 0}", pingStatus.Status);
                didGoCompleteJobb = false;
                Thread.Sleep(500);
                Ping(true);
            }

            return didGoCompleteJobb;
        }
    }
}
