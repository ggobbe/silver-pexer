using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SilverPexer
{
    public class Configuration
    {
        private const string configurationFile = "appsettings.json";

        private readonly IConfigurationRoot _configuration;

        private readonly ILogger _logger;

        private string _password;
        private IEnumerable<string> _pathToInn;
        private string _timeToSleep;
        private string _username;
        private int? _actionPoints;
        private bool? _goToSleepWhenMessage;
        private Stats _levelUp;

        public Configuration(ILogger logger)
        {
            _logger = logger;
            _logger.LogDebug("Building configuration");

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(configurationFile, false, false);

            _configuration = builder.Build();
        }

        public string Username
        {
            get { return _username ?? (_username = GetConfiguration("username")); }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password ?? (_password = GetConfiguration("password")); }
            set { _password = value; }
        }

        public IEnumerable<string> PathToInn
        {
            get
            {
                if (_pathToInn == null)
                {
                    var path = GetConfiguration("pathToInn");
                    _pathToInn = path.Split(',').Select(e => e.Trim());
                }
                return _pathToInn;
            }
        }

        public string TimeToSleep
        {
            get { return _timeToSleep ?? (_timeToSleep = GetConfiguration("timeToSleep")); }
        }

        public int? ActionPoints
        {
            get { return _actionPoints ?? (_actionPoints = int.Parse(GetConfiguration("actionPoints"))); }
        }

        public bool GoToSleepWhenMessage
        {
            get
            {
                if (!_goToSleepWhenMessage.HasValue)
                {
                    _goToSleepWhenMessage = bool.Parse(GetConfiguration("goToSleepWhenMessage"));
                }
                return _goToSleepWhenMessage.Value;
            }
        }

        public Stats LevelUp
        {
            get
            {
                if (_levelUp == null)
                {
                    _levelUp = new Stats()
                    {
                        Constitution = int.Parse(GetConfiguration("levelUp:constitution")),
                        Strength = int.Parse(GetConfiguration("levelUp:strength")),
                        Agility = int.Parse(GetConfiguration("levelUp:agility")),
                        Intelligence = int.Parse(GetConfiguration("levelUp:intelligence")),
                    };
                }
                return _levelUp;
            }
        }

        private string GetConfiguration(string key)
        {
            _logger.LogDebug($"Reading configuration key {key} in {configurationFile}.");
            return _configuration[key];
        }
    }
}