using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AssignmentWebAPI
{
    public class ServerConfigurationEntry
    {
        public string Server_ID { get; set; }
        public short Server_Type { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string URL { get; set; }
        public bool Enabled { get; set; }
        public bool OnDemand { get; set; }
        public TimeSpan TimeDuration { get; set; }
        public bool IsLive { get; set; }
    }


    public class Program
    {
        public static Semaphore ConfigFileSemaphore;
        public static Semaphore ServerStatusFileSemaphore;
        public static Dictionary<string, int> LocateServerEntry;
        public static List<ServerConfigurationEntry> ServerConfigurationList;
        public static int CountForServerEntry;
        public static Semaphore RequestFileSemaphore;
        public static Semaphore ResponseFileSemaphore;

        public static void Main(string[] args)
        {
            ConfigFileSemaphore = new Semaphore(1, 1, "ConfigFileSemaphore");
            ServerStatusFileSemaphore = new Semaphore(1, 1, "ServerStatusFileSemaphore");
            LocateServerEntry = new Dictionary<string, int>();
            ServerConfigurationList = new List<ServerConfigurationEntry>();
            CountForServerEntry = 0;
            RequestFileSemaphore = new Semaphore(1, 1, "RequestFileSemaphore");
            ResponseFileSemaphore = new Semaphore(1, 1, "ResponseFileSemaphore");
            //ConfigFileSemaphore.Release();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }

    /*
    public class RequestResponse
    {
        private static Semaphore RequestSemaphore;
        public RequestResponse()
        {
            RequestSemaphore = new Semaphore(1, 1, "RequestSemaphore");
        }
        public string MakeARequest(int Code, string Parameter="")
        {
            RequestSemaphore.WaitOne();
            using (StreamWriter SR = new StreamWriter(@"C:\Users\User\Documents\Request\Request.txt"))
            {
                SR.WriteLine(Code.ToString() + "\n" + Parameter);
            }
            RequestSemaphore.Release(1);

        }
    }


    public class Response
    {
        private static Semaphore ResponseSemaphore;
        public Response()
        {
            ResponseSemaphore = new Semaphore(1, 1, "ResponseSemaphore");
        }
    }*/
}