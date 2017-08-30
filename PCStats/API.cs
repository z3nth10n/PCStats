using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace PCStats
{
    //Esto irá en el windows form api, y luego le metere una carpeta llamada Credentails donde meteré el formulario del frmCredentials
    internal class API
    {
        internal static Action _loadConfigCallback;

        public static LanguageManager lang;
        public static Configuration config;

        public const string _InitConfig = "InitializatedConfig",
                            usernameConfig = "loginUsername",
                            passwordConfig = "loginPassword",
                            sessionTimeConfig = "endSessionConfig";

        public static bool RememberingAuth
        {
            get
            {
                return config != null && !config.AppSettings.Settings.IsEmpty(usernameConfig) || !config.AppSettings.Settings.IsEmpty(passwordConfig);
            }
        }

        private static void InitConfig(Configuration config)
        {
            config.AppSettings.Settings.Add(_InitConfig, "true");
            config.AppSettings.Settings.Add(usernameConfig, "");
            config.AppSettings.Settings.Add(passwordConfig, "");
            config.AppSettings.Settings.Add(sessionTimeConfig, "");
        }

        public static void LoadConfig()
        {
            //Load config
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "App.config");
            config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.IsEmpty(_InitConfig))
                InitConfig(config);

            //Load things that needs config
            //Load for example offline sessions
            string val = !config.AppSettings.Settings.IsEmpty(sessionTimeConfig) ? config.AppSettings.Settings[sessionTimeConfig].Value : "";
            if (!string.IsNullOrEmpty(val))
                OfflineSession.Load(val);

            Console.WriteLine(_loadConfigCallback == null);
            _loadConfigCallback?.Invoke();
        }

        public static void LoadConfigCallback(Action call)
        {
            Console.WriteLine("Setting callback!!");
            _loadConfigCallback = call;
        }
    }
}