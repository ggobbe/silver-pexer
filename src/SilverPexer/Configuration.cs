using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SilverPexer
{
    public class Configuration
    {
        private readonly IConfigurationRoot _configuration;

        private string _password;
        private string _pathToInn;
        private string _timeToSleep;
        private string _username;
        private Stats _levelUp;
        private int? _actionPoints;

        public Configuration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false, false);

            _configuration = builder.Build();
        }

        public string Username
        {
            get { return _username ?? _configuration["username"]; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password ?? _configuration["password"]; }
            set { _password = value; }
        }

        public IEnumerable<string> PathToInn
        {
            get
            {
                var path = _pathToInn ?? _configuration["pathToInn"];
                return path.Split(',').Select(e => e.Trim());
            }
            set { _pathToInn = string.Join(",", value); }
        }

        public string TimeToSleep
        {
            get { return _timeToSleep ?? _configuration["timeToSleep"]; }
            set { _timeToSleep = value; }
        }

        public int? ActionPoints
        {
            get { return _actionPoints ?? int.Parse(_configuration["actionPoints"]); }
            set { _actionPoints = value; }
        }

        public Stats LevelUp
        {
            get
            {
                var levelUp = new Stats()
                {
                    Constitution = int.Parse(_configuration["levelUp:constitution"]),
                    Strength = int.Parse(_configuration["levelUp:strength"]),
                    Agility = int.Parse(_configuration["levelUp:agility"]),
                    Intelligence = int.Parse(_configuration["levelUp:intelligence"]),
                };
                return levelUp;
            }
        }
    }
}