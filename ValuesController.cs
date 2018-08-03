using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AssignmentWebAPI
{ 
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public string Get()
        {
            string StatusOfAll=null;
            try
            {
                Program.ServerStatusFileSemaphore.WaitOne();
                using (StreamReader SR = new StreamReader(@"C:\Users\User\Documents\Assignment\Status\Status.txt"))
                {
                    StatusOfAll = SR.ReadToEnd();
                }
                Program.ServerStatusFileSemaphore.Release(1);
            }
            catch(Exception E)
            {
                Program.ServerStatusFileSemaphore.Release(1);
            }
            return StatusOfAll;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(string Server_ID)
        {
            string StatusOfID = null;
            int Index = Program.LocateServerEntry[Server_ID];
            if (!Program.ServerConfigurationList[Index].Enabled)
                return (-1).ToString();
            //Handle for OnDemand Server.
            /*else if(Program.ServerConfigurationList[Index].OnDemand)
            {
                try
                {
                    Program.RequestFileSemaphore.WaitOne();
                    using (StreamWriter SW = new StreamWriter(@"C:\Users\User\Documents\Assignment\Request"))
                    {
                        SW.WriteLine(Server_ID);
                    }
                    Program.RequestFileSemaphore.Release(1);
                }
                catch(Exception E)
                {
                    Program.ServerStatusFileSemaphore.Release(1);
                }
                
                //Wait for response.
            }*/
            try
            {
                Program.ServerStatusFileSemaphore.WaitOne();
                using (StreamReader SR = new StreamReader(@"C:\Users\User\Documents\Assignment\Status\Status.txt"))
                {
                    for (int i = 0; i < Index; i++)
                        SR.ReadLine();
                    StatusOfID = SR.ReadLine();
                }
                Program.ServerStatusFileSemaphore.Release(1);
            }
            catch(Exception E)
            {
                Program.ServerStatusFileSemaphore.Release(1);
            }
            return StatusOfID;
        }

        // POST api/<controller>
        [HttpPost]
        public string Post([FromBody]ServerConfigurationEntry _ServerConfigurationEntry)
        {
            //string value = null ;
            //ServerConfigurationEntry _ServerConfigurationEntry=JsonConvert.DeserializeObject<ServerConfigurationEntry>(value);
            string Server_ID = Guid.NewGuid().ToString();
            _ServerConfigurationEntry.Server_ID = Server_ID;
            string value = JsonConvert.SerializeObject(_ServerConfigurationEntry);
            Program.LocateServerEntry.Add(Server_ID, Program.CountForServerEntry);
            Program.ServerConfigurationList.Add(_ServerConfigurationEntry);
            Program.CountForServerEntry++;
            try
            {
                Program.ConfigFileSemaphore.WaitOne();
                using (StreamWriter SW = new StreamWriter(@"C:\Users\User\Documents\Assignment\Config\Config.txt", true))
                {
                    SW.WriteLine(value);
                }
                Program.ConfigFileSemaphore.Release(1);
                Console.WriteLine("Server Added Successfully");
            }
            catch(Exception E)
            {
                Program.ConfigFileSemaphore.Release(1);
                Console.WriteLine("Server NOT Added Successfully");
            }
            
            return Server_ID;
        }

        // PUT api/<controller>/5
        [HttpPut]
        public bool Put([FromBody]ServerConfigurationEntry _ServerConfigurationEntry)
        {
            //ServerConfigurationEntry _ServerConfigurationEntry = JsonConvert.DeserializeObject<ServerConfigurationEntry>(value);
            int Index = Program.LocateServerEntry[_ServerConfigurationEntry.Server_ID];
            Program.ServerConfigurationList[Index] = _ServerConfigurationEntry;
            string value = JsonConvert.SerializeObject(_ServerConfigurationEntry);
            string ConfigData = null;
            string temp = null;
            try
            {
                Program.ConfigFileSemaphore.WaitOne();
                using (StreamReader SR = new StreamReader(@"C:\Users\User\Documents\Assignment\Config\Config.txt"))
                {
                    for (int i = 0; i < Program.CountForServerEntry; i++)
                    {
                        temp = SR.ReadLine();
                        if (i != Index)
                            ConfigData += temp;
                        else
                            ConfigData += value;
                        ConfigData += Environment.NewLine;
                    }
                }
                using (StreamWriter SW = new StreamWriter(@"C:\Users\User\Documents\Assignment\Config\Config.txt"))
                {
                    SW.Write(ConfigData);
                }
                Program.ConfigFileSemaphore.Release(1);
                Console.WriteLine("Server Modified Successfully at {0} index", Index);
                return true;
            }
            catch(Exception E)
            {
                Program.ConfigFileSemaphore.Release(1);
                Console.WriteLine("Server Modified Successfully at {0} index", Index);
                return false;
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public bool Delete([FromBody]string Server_ID)
        {

            int Index = Program.LocateServerEntry[Server_ID];
            Program.ServerConfigurationList[Index].IsLive = false;
            string value = JsonConvert.SerializeObject(Program.ServerConfigurationList[Index]);
            string ConfigData = null;
            string temp = null;
            try
            {
                Program.ConfigFileSemaphore.WaitOne();
                using (StreamReader SR = new StreamReader(@"C:\Users\User\Documents\Assignment\Config\Config.txt"))
                {
                    for (int i = 0; i < Program.CountForServerEntry; i++)
                    {
                        temp = SR.ReadLine();
                        if (i != Index)
                            ConfigData += temp;
                        else
                            ConfigData += value;
                        ConfigData += Environment.NewLine;
                    }
                }
                using (StreamWriter SW = new StreamWriter(@"C:\Users\User\Documents\Assignment\Config\Config.txt"))
                {
                    SW.Write(ConfigData);
                }
                Program.ConfigFileSemaphore.Release(1);
                Console.WriteLine("Server Modified Successfully at {0} index", Index);
                return true;
            }
            catch (Exception E)
            {
                Program.ConfigFileSemaphore.Release(1);
                Console.WriteLine("Server Modified Successfully at {0} index", Index);
                return false;
            }

            //int Index = Program.LocateServerEntry[Server_ID];
            //Program.ServerConfigurationList[Index].IsLive=false;
            //string value = JsonConvert.SerializeObject(Program.ServerConfigurationList[Index]);
            //string ConfigData = null;
            //string temp = null;
            //try
            //{
            //    Program.ConfigFileSemaphore.WaitOne();
            //    using (StreamReader SR = new StreamReader(@"C:\Users\User\Documents\Assignment\Config\Config.txt"))
            //    {
            //        for (int i = 0; i < Program.CountForServerEntry; i++)
            //        {
            //            temp = SR.ReadLine();
            //            if (i != Index)
            //                ConfigData += temp;
            //            else
            //                ConfigData += value;
            //            ConfigData += "\n";
            //        }
            //    }
            //    using (StreamWriter SW = new StreamWriter(@"C:\Users\User\Documents\Assignment\Config\Config.txt"))
            //    {
            //        SW.Write(ConfigData);
            //    }
            //    Program.ConfigFileSemaphore.Release(1);
            //}
            //catch(Exception E)
            //{
            //    Program.ConfigFileSemaphore.Release(1);
            //}
            
        }

        //Semaphore RequestSemaphore = new Semaphore(1, 1, "Request");
        //Semaphore ReplySemaphore = new Semaphore(1, 1, "Reply");
        //FileSystemWatcher CatchChange = new FileSystemWatcher(@"C:\Users\User\Documents\Reply");
        //public static Dictionary<Guid, Server> Locate; 
    }
}
