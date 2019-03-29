using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Capgemini.Pipefy.Test
{
    public class TestConfiguration
    {
        private const string DefaultConfigurationFile = "testconfig.json";

        private JObject _configuration;
        public JObject Configuration
        {
            get
            {
                EnsureLoaded();
                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }

        private string configurationFile;

        private static TestConfiguration _instance;
        public static TestConfiguration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TestConfiguration();
                return _instance;
            }
        }

        private Dictionary<string, object> customConfig;

        public TestConfiguration(string configFile)
        {
            customConfig = new Dictionary<string, object>();
            configurationFile = Path.GetFullPath(configFile);
        }

        public TestConfiguration() : this (DefaultConfigurationFile) { }

        protected void EnsureLoaded()
        {
            if (_configuration != null)
                return;

            LoadConfig();
        }

        protected void LoadConfig()
        {
            using (var fileReader = File.OpenText(configurationFile))
            {
                using (var jsonReader = new JsonTextReader(fileReader))
                {
                    _configuration = (JObject)JToken.ReadFrom(jsonReader);
                }
            }
        }

        public string GetBearer()
        {
            if (Configuration == null)
                return string.Empty;

            return _configuration.Value<string>("bearer");
        }

        public void SetCustomConfig(string key, object value)
        {
            customConfig.Add(key, value);
        }

        public object GetCustomConfig(string key)
        {
            if (customConfig.ContainsKey(key))
                return customConfig[key];
            return null;
        }

        public Dictionary<string, object> GetDefaultActivityArguments()
        {
            var dict = new Dictionary<string, object>();
            dict["Bearer"] = Configuration.Value<string>("bearer");
            dict["Timeout"] = Configuration.Value<int>("timeout");
            return dict;
        }
    }
}