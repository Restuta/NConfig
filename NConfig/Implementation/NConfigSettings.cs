﻿using System;
using System.Configuration;
using System.Linq;
using System.IO;

namespace NConfig
{
    internal sealed class NConfigSettings : INConfigSettings
    {
        private const string HostMapFile = "HostMap.config";

        private readonly IConfigurationRepository repository;
        private readonly string hostAlias;
        private readonly bool isWeb;


        public NConfigSettings(IConfigurationRepository repository)
        {
            this.repository = repository;
            hostAlias = DetectHostAlias();
            isWeb = DetectIsWeb();
        }


        /// <summary>
        /// Gets the alias assigned for current Host.
        /// This alias used to find out Host specific configurations.
        /// </summary>
        /// <value>The host's alias.</value>
        public string HostAlias
        {
            get
            {
                return hostAlias;
            }
        }

        /// <summary>
        /// Gets a value indicating whether current application is web based.
        /// </summary>
        /// <value><c>true</c> if current application is web bases; otherwise, <c>false</c>.</value>
        public bool IsWeb
        {
            get
            {
                return isWeb;
            }
        }


        /// <summary>
        /// Detects the alias for the current host.
        /// First it reads HostMap.Config file then searches inside App.Config, if not sucessful
        /// returns current host name.
        /// </summary>
        private string DetectHostAlias()
        {
            // Try to read from HostMap.config file, then try to read from AppConfig/WebConfig
            string hostName = Environment.MachineName;
            HostMapSection hostMapSection = null;

            Configuration hostConfig = repository.GetFileConfiguration(HostMapFile);
            if (hostConfig != null)
                hostMapSection = hostConfig.GetSection<HostMapSection>("hostMap");

            if (hostMapSection == null)
                hostMapSection = ConfigurationManager.GetSection("hostMap") as HostMapSection;

            if (hostMapSection != null)
            {
                if (hostMapSection.Mappings.ContainsHost(hostName))
                    return hostMapSection.Mappings[hostName].Alias;
            }
            return hostName;
        }

        /// <summary>
        /// Detects if the current application is web based.
        /// Detection method is not natural (HostingEnvironment.IsHosted) but allows no to upload System.Web assembly.
        /// </summary>
        public static bool DetectIsWeb()
        {
            string configFile = Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            return configFile.Equals("web.config", StringComparison.InvariantCultureIgnoreCase);
        }
    
    }
}
