using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PasswordCrackerCentralized;
using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;

namespace PasswordCrackerSlave
{
    class Slave
    {
        public static List<UserInfo> UserList = new List<UserInfo>();
        public static List<string> ResultList = new List<string>();
        public static List<string> DictList = PasswordFileHandler.ReadDictFile("webster-dictionary.txt");
        public static int Indexer;
        public static int Endexer = Indexer + 4999;
        static void Main(string[] args)
        {
            TcpClient clientSocket = new TcpClient("192.168.3.110", 6789);
            Console.WriteLine("Client started");
            Stream ns = clientSocket.GetStream();


            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true; // enable automatic flushing

            //(List<string> DictList = PasswordFileHandler.ReadDictFile("webster-dictionary.txt");
            for (int i = 0; i < 100; i++)
            {
                string master = sr.ReadLine();

                if (master.StartsWith("\\")) //Gets the Userinfo and adding it to a static list
                {
                    Console.WriteLine("Server: " + master.Substring(1));
                    string line = master.Substring(1);
                    string[] parts = line.Split(":".ToCharArray());
                    UserInfo userInfo = new UserInfo(parts[0], parts[1]);
                    UserList.Add(userInfo);
                }
                else
                {
                    if (master == "RESULT") //Will send the cracked userinfo when master asks for it
                    {
                        //ResultList.Add("List of results");
                        //ResultList.Add("2nd line");
                        //ResultList.Add("3rd line");
                        foreach (string result in ResultList)
                        {
                            sw.WriteLine(result);
                        }
                        break;
                    }
                    Indexer = int.Parse(master); //Gets the chunk the master apointed

                    Console.WriteLine("Starting Cracking on chunk {0}",master);
                    Cracking cracker = new Cracking();
                    cracker.RunCracking();

                    Console.WriteLine("Done with chunk {0}",master);

                    Console.ReadKey();
                    sw.WriteLine("ready");
                }
            }


            ns.Close();
            clientSocket.Close();
        }
    }
}
