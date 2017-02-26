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
        private IEnumerable<string> _pathToSleep;
        private SleepType? _sleepType;
        private string _timeToSleep;
        private string _username;
        private int? _actionPoints;
        private bool? _goToSleepWhenMessage;
        private Stats _levelUp;
        private string _spell;
        private string _monster;
        private Potion _potion;

        public Configuration(ILogger logger)
        {
            _logger = logger;
            _logger.LogDebug("Building configuration");

            var builder = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
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

        public SleepType? SleepType
        {
            get
            {
                return _sleepType ?? (_sleepType = (SleepType)Enum.Parse(typeof(SleepType), GetConfiguration("sleepType")));
            }
        }

        public IEnumerable<string> PathToSleep
        {
            get
            {
                if (_pathToSleep == null)
                {
                    var path = GetConfiguration("pathToSleep");
                    _pathToSleep = path.Split(',').Select(e => e.Trim());
                }
                return _pathToSleep;
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

        public string Spell
        {
            get { return _spell ?? (_spell = GetConfiguration("spell")); }
        }

        public string Monster
        {
            get { return _monster ?? (_monster = GetConfiguration("monster")); }
        }
        
        public Potion Potion
        {
            get
            {
                if (_potion == null)
                {
                    _potion = new Potion()
                    {
                        Id = GetConfiguration("potion:id"),
                        Amount = int.Parse(GetConfiguration("potion:amount")),
                        Hits = int.Parse(GetConfiguration("potion:hits"))
                    };
                }
                return _potion;
            }
        }

        private string GetConfiguration(string key)
        {
            _logger.LogDebug($"Reading configuration key {key} in {configurationFile}.");
            return _configuration[key];
        }
    }
}
