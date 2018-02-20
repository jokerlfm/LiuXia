using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    class ConfigHandler
    {
        private static Dictionary<string, string> configs;

        public static void LoadConfig()
        {
            configs = new Dictionary<string, string>();
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "config.ini";
            if (!System.IO.File.Exists(configPath))
            {
                return;
            }
            string[] configArray = System.IO.File.ReadAllText(configPath, Encoding.UTF8).Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string eachConfig in configArray)
            {
                string[] valuePair = eachConfig.Split(new string[] { "=" }, StringSplitOptions.None);
                if (valuePair.Length > 1)
                {
                    configs.Add(valuePair[0].ToLower(), valuePair[1].ToLower());
                }
            }
        }

        public static string GetConfigValue_string(string pmKey)
        {
            string lowerKey = pmKey.ToLower();
            if (configs.ContainsKey(lowerKey))
            {
                return configs[lowerKey];
            }
            else
            {
                return "";
            }
        }

        public static int GetConfigValue_int(string pmKey)
        {
            string lowerKey = pmKey.ToLower();
            if (configs.ContainsKey(lowerKey))
            {
                int checkValue = 0;
                int.TryParse(configs[lowerKey], out checkValue);
                return checkValue;
            }
            else
            {
                return 0;
            }
        }

        public static float GetConfigValue_float(string pmKey)
        {
            string lowerKey = pmKey.ToLower();
            if (configs.ContainsKey(lowerKey))
            {
                float checkValue = 0;
                float.TryParse(configs[lowerKey], out checkValue);
                return checkValue;
            }
            else
            {
                return 0;
            }
        }
    }
}