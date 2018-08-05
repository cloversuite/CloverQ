using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestRouter
{
    /// <summary>
    /// Clase que utilizo para almacenar la configuracion del sistema
    /// Esta clase se deserealiza del un archivo para poder obtener la conf del sistema desde 
    /// un archivo JSON
    /// </summary>
    public class SystemConfiguration
    {
        string fileName = "cloverq-conf.json";
        List<ConfHost> callManagers = new List<ConfHost>();
        List<ConfHost> stateProviders = new List<ConfHost>();
        List<ConfHost> loginProviders = new List<ConfHost>();


        public SystemConfiguration(string fileName)
        {
            this.fileName = fileName;
        }

        public ConfQueueLog QueueLog { get; set; }
        public List<ConfHost> CallManagers { get { return callManagers; } set { callManagers = value; } }
        public List<ConfHost> StateProviders { get { return stateProviders; } set { stateProviders = value; } }
        public List<ConfHost> LoginProviders { get { return loginProviders; } set { loginProviders = value; } }

        public void SaveConf() {
            // serialize JSON to a string and then write string to a file
            File.WriteAllText(fileName, JsonConvert.SerializeObject(this,Formatting.Indented));

            // serialize JSON directly to a file
            //using (StreamWriter file = File.CreateText(@"cloverq-conf.json"))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    serializer.Serialize(file, this);
            //}
        }

        public static SystemConfiguration GetConf(string fileName) {
            // read file into a string and deserialize JSON to a type
            return  JsonConvert.DeserializeObject<SystemConfiguration>(File.ReadAllText(fileName));

            // deserialize JSON directly from a file
            //using (StreamReader file = File.OpenText(@"c:\movie.json"))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    SystemConfiguration conf = (SystemConfiguration)serializer.Deserialize(file, typeof(SystemConfiguration));
            //}
        }
    }
}
