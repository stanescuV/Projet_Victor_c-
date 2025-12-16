using System.Text.Json;
using System.Text.Json.Serialization;

namespace Projet_Victor_c_
{
    public class DataStore
    {
        public List<Host> Hosts { get; set; } = new();
        public List<NetworkInterface> Interfaces { get; set; } = new();
        public List<FirewallRule> Rules { get; set; } = new();

        private string _path;
        public string Path => _path;

        // Parameterless ctor with no side-effects: used by the JSON serializer
        public DataStore()
        {
        }

        public DataStore(string path) : this()
        {
            // determine a usable path: if provided path's folder is writable use it, otherwise fallback to AppData
            _path = path ?? string.Empty;
            try
            {
                var dir = System.IO.Path.GetDirectoryName(_path) ?? AppContext.BaseDirectory;
                if (!EnsureDirectoryWritable(dir))
                {
                    var appFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Projet_Victor_c");
                    System.IO.Directory.CreateDirectory(appFolder);
                    _path = System.IO.Path.Combine(appFolder, "data.json");
                }
            }
            catch
            {
                var appFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Projet_Victor_c");
                try { System.IO.Directory.CreateDirectory(appFolder); } catch { }
                _path = System.IO.Path.Combine(appFolder, "data.json");
            }

            Load();
        }

        private bool EnsureDirectoryWritable(string? folder)
        {
            try
            {
                if (string.IsNullOrEmpty(folder)) return false;
                System.IO.Directory.CreateDirectory(folder);
                var testFile = System.IO.Path.Combine(folder, "__writetest.tmp");
                System.IO.File.WriteAllText(testFile, "test");
                System.IO.File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Load()
        {
            try
            {
                if (string.IsNullOrEmpty(_path) || !System.IO.File.Exists(_path)) return;
                var txt = System.IO.File.ReadAllText(_path);
                if (string.IsNullOrWhiteSpace(txt)) return;

                var opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                var ds = JsonSerializer.Deserialize<DataStore>(txt, opts);
                if (ds != null)
                {
                    Hosts = ds.Hosts ?? new List<Host>();
                    Interfaces = ds.Interfaces ?? new List<NetworkInterface>();
                    Rules = ds.Rules ?? new List<FirewallRule>();
                }
            }
            catch
            {
                // ignore errors and keep defaults
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(_path)) throw new InvalidOperationException("DataStore path is not set");

            var opts = new JsonSerializerOptions { WriteIndented = true };
            opts.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            var txt = JsonSerializer.Serialize(this, opts);

            // Ensure directory exists
            var dir = System.IO.Path.GetDirectoryName(_path) ?? AppContext.BaseDirectory;
            System.IO.Directory.CreateDirectory(dir);

            // Write atomically: write to temp then replace
            var tmp = _path + ".tmp";
            System.IO.File.WriteAllText(tmp, txt);
            try
            {
                System.IO.File.Copy(tmp, _path, true);
                System.IO.File.Delete(tmp);
            }
            catch
            {
                // try fallback: overwrite directly
                System.IO.File.WriteAllText(_path, txt);
                try { if (System.IO.File.Exists(tmp)) System.IO.File.Delete(tmp); } catch { }
            }
        }
    }
}
