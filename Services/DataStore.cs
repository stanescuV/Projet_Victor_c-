
using System.Text.Json;

namespace Projet_Victor_c_
{
    //un service pour stocker et getter principalement les données 
    public class DataStore
    {
        public List<Host> Hosts { get; set; } = new();
        public List<NetworkInterface> Interfaces { get; set; } = new();
        public List<FirewallRule> Rules { get; set; } = new();

        private string _path;
        public DataStore(string path)
        {
            _path = path;
            Load();
        }

        public void Load()
        {
            try
            {
                if (!File.Exists(_path)) return;
                var txt = File.ReadAllText(_path);
                var ds = JsonSerializer.Deserialize<DataStore>(txt);
                if (ds != null)
                {
                    Hosts = ds.Hosts ?? new List<Host>();
                    Interfaces = ds.Interfaces ?? new List<NetworkInterface>();
                    Rules = ds.Rules ?? new List<FirewallRule>();
                }
            }
            catch { }
        }

        public void Save()
        {
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var txt = JsonSerializer.Serialize(this, opts);
            File.WriteAllText(_path, txt);
        }
    }
}
