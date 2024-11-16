using Newtonsoft.Json;
using Tavstal.TExample.Models;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TExample
{
    public class ExampleConfig : ConfigurationBase
    {
        [JsonProperty(Order = 3)]
        public DatabaseData Database { get; set; }

        public override void LoadDefaults()
        {
            DebugMode = false;
            Locale = "en";
            DownloadLocalePacks = true;
            Database = new DatabaseData("example_players");
        }

        // Required because of the library
        public ExampleConfig() { }
        public ExampleConfig(string fileName, string path) : base(fileName, path) { }
    }
}
