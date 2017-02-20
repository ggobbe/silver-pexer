using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace SilverPexer
{
    public class Pexer
    {
        public bool Continue { get; private set; } = true;

        private const string BaseUrl = "https://www.silver-world.net";

        private readonly Configuration _configuration;

        private readonly Random _random;

        private readonly ChromeDriver _driver;

        private readonly ILogger _logger;

        private int _actionCount = 0;

        private int _hitsCount = 0;

        public Pexer(Configuration configuration, ChromeDriver driver, ILogger logger)
        {
            _logger = logger;
            _random = new Random();
            _configuration = configuration;
            _driver = driver;
            _driver.Manage().Window.Maximize();
        }

        public void Login()
        {
            _logger.LogInformation($"Logging user {_configuration.Username}");
            _driver.Url = BaseUrl;
            _driver.FindElementByName("login").SendKeys(_configuration.Username);
            _driver.FindElementByName("pass").SendKeys(_configuration.Password);
            _driver.FindElementByName("Submit2").Click();
        }

        public void KillAllMonsters()
        {
            while (Continue && IsMonsterPresent())
            {
                AttackMonster();
                Thread.Sleep(_random.Next(100, 500));
            }
        }

        public bool IsMonsterPresent()
        {
            NavigateTo("map.php", force: false);
            return _driver.FindElementsByCssSelector("a[href^=\"fight.php?type=monster\"]").Any();
        }

        public void AttackMonster()
        {
            NavigateTo("map.php", force: false);

            _driver.FindElementByCssSelector("a[href^=\"fight.php?type=monster\"]").Click();

            var killed = false;
            while (!killed)
            {
                _driver.FindElementByCssSelector($"input[src^=\"systeme/mag{_configuration.Spell}.\"]").Click();
                _actionCount += 2;
                _hitsCount++;

                if (_hitsCount >= _configuration.Potion.Hits)
                {
                    DrinkPotion();
                }

                killed = (!_driver.Url.Contains("fight.php?type=monster") && !_driver.Url.Contains("sort.php"))
                            || _driver.FindElementByClassName("descriptiontitle").Text.Contains("est tué");
            }

            if (_actionCount >= _configuration.ActionPoints)
            {
                GoToSleep();
            }
        }

        private void NavigateTo(string page, bool force = true)
        {
            if (force || !_driver.Url.Contains(page))
            {
                _driver.Url = $"{BaseUrl}/{page}";
            }

            if (_driver.Url.Contains("levelup.php"))
            {
                LevelUp();
                _driver.Url = $"{BaseUrl}/{page}";
            }

            if (_driver.Url.Contains("map.php"))
            {
                if (IsOtherPlayerPresent() || (_configuration.GoToSleepWhenMessage && IsNewMessagePresent()))
                {
                    GoToSleep();
                    return;
                }
            }
        }

        private void DrinkPotion()
        {
            if (string.IsNullOrEmpty(_configuration.Potion.Id))
            {
                return;
            }

            NavigateTo("myperso.php");

            // Level up could reset the hitsCount
            if(_hitsCount == 0) 
            {
                return;
            }

            for (int i = 0; i < _configuration.Potion.Amount; i++)
            {
                var potionImage = _driver.FindElementByCssSelector($"img[src^=\"systeme/obj{_configuration.Potion.Id}.\"");
                potionImage.Click();
            }
            _hitsCount = 0;
        }

        public void WaitForMonsters()
        {
            while (Continue && !IsMonsterPresent())
            {
                if (IsLootPresent())
                {
                    GrabLoot();
                    continue;
                }

                Thread.Sleep(_random.Next(250, 1000));
                NavigateTo("map.php", force: true);
            }
        }

        private void GoToSleep()
        {
            _logger.LogInformation("Going to sleep");
            NavigateTo("map.php", force: true);

            foreach (var cell in _configuration.PathToSleep)
            {
                ClickOnMap(cell.Trim());
                NavigateTo("map.php", force: false);
            }

            switch (_configuration.SleepType)
            {
                default:
                case SleepType.Inn:
                    NavigateTo("auberge.php");
                    if (!_driver.Url.Contains("auberge.php"))
                    {
                        _logger.LogError("Could not open auberge.php");
                        return;
                    }
                    var duree = _driver.FindElementByName("duree");
                    duree.Clear();
                    duree.SendKeys(_configuration.TimeToSleep);
                    _driver.FindElementByCssSelector("input[name =\"Submit\"][value=\"m'endormir\"]").Click();
                    break;
                case SleepType.Camp:
                    NavigateTo("camp.php?map=enter");
                    if (!_driver.Url.Contains("camp.php"))
                    {
                        _logger.LogError("Could not open camp.php");
                        return;
                    }
                    break;
            }

            // Stop looping
            Continue = false;
        }

        private bool IsOtherPlayerPresent()
        {
            var spotted = _driver.FindElements(By.CssSelector("a[href^=\"fight.php?type=user\"]")).Any();
            if (spotted)
            {
                _logger.LogInformation("Another player was spotted");
            }
            return spotted;
        }

        private void ClickOnMap(string coordinates)
        {
            NavigateTo("map.php", force: false);
            _logger.LogInformation($"Clicking on map at coordinates {coordinates}");
            var regex = new Regex(@"^(?<latitude>[A-Z])(?<longitude>[1-3]?[0-9])$");

            if (!regex.IsMatch(coordinates))
            {
                throw new ArgumentException(nameof(coordinates), "Invalid map coordinates.");
            }

            const int cellWidth = 20;

            var match = regex.Match(coordinates);
            var x = int.Parse(match.Groups["longitude"].Value) - 1;
            var y = match.Groups["latitude"].Value[0] - (int)'A';

            Thread.Sleep(TimeSpan.FromMilliseconds(500)); // wait for flash to load the map before to click on it
            Actions builder = new Actions(_driver);
            var map = _driver.FindElementByCssSelector("td[colspan=\"30\"][rowspan=\"12\"]");
            var xPos = (x * cellWidth) + (cellWidth / 2);
            var yPos = (y * cellWidth) + (cellWidth / 2);
            builder.MoveToElement(map, xPos, yPos);
            builder.Click().Perform();
        }

        private void GrabLoot()
        {
            NavigateTo("map.php", force: false);
            _driver.FindElementByCssSelector("input[src^=\"systeme/obj\"]").Click();
        }

        private bool IsNewMessagePresent()
        {
            var received = _driver.FindElements(By.CssSelector("[href=\"/messages.php\"]")).Count > 1;
            if (received)
            {
                _logger.LogInformation("Message received");
            }
            return received;
        }

        private bool IsLootPresent()
        {
            NavigateTo("map.php", force: false);
            return _driver.FindElements(By.CssSelector("input[src^=\"systeme/obj\"]")).Any();
        }

        private void LevelUp()
        {
            _logger.LogInformation("Level up");
            int pointsLeft;
            if (!int.TryParse(_driver.FindElementByName("left").GetAttribute("value"), out pointsLeft) || pointsLeft != _configuration.LevelUp.Total)
            {
                throw new InvalidOperationException("The amount of points left is different than the amount of points to distribute.");
            }

            var distributed = new Stats();
            while (distributed.Constitution < _configuration.LevelUp.Constitution)
            {
                _driver.FindElementByName("Button").Click();
                distributed.Constitution++;
            }

            while (distributed.Strength < _configuration.LevelUp.Strength)
            {
                _driver.FindElementByName("Button2").Click();
                distributed.Strength++;
            }

            while (distributed.Agility < _configuration.LevelUp.Agility)
            {
                _driver.FindElementByName("Button3").Click();
                distributed.Agility++;
            }

            while (distributed.Intelligence < _configuration.LevelUp.Intelligence)
            {
                _driver.FindElementByName("Button4").Click();
                distributed.Intelligence++;
            }

            _driver.FindElementByName("Submit").Click();

            // Reset hits count as level up restore mana and health
            _hitsCount = 0;
        }
    }
}