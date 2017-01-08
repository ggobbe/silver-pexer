using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

        public Pexer(Configuration configuration, ChromeDriver driver)
        {
            _random = new Random();
            _configuration = configuration;
            _driver = driver;
            _driver.Manage().Window.Maximize();
        }

        public void Login()
        {
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
                Thread.Sleep(_random.Next(100, 1000));
            }
        }

        public bool IsMonsterPresent()
        {
            NavigateToMap();
            return _driver.FindElementsByCssSelector("a[href^=\"fight.php?type=monster\"]").Any();
        }

        public void AttackMonster()
        {
            NavigateToMap();

            if (IsOtherPlayerPresent())
            {
                GoToSleep();
                return;
            }

            _driver.FindElementByCssSelector("a[href^=\"fight.php?type=monster\"]").Click();

            var killed = false;
            while (!killed)
            {
                _driver.FindElementByCssSelector("input[src=\"systeme/mag17.gif\"]").Click();
                killed = _driver.FindElementByClassName("descriptiontitle").Text.Contains("est tué") ||
                         (!_driver.Url.Contains("fight.php?type=monster") && !_driver.Url.Contains("sort.php"));
            }
        }

        public void WaitForMonsters()
        {
            while (Continue && !IsMonsterPresent())
            {
                if (_driver.Url.Contains("levelup.php"))
                {
                    LevelUp();
                }

                if (IsNewMessagePresent())
                {
                    GoToSleep();
                    return;
                }

                if (IsOtherPlayerPresent())
                {
                    GoToSleep();
                    return;
                }

                if (IsLootPresent())
                {
                    GrabLoot();
                    continue;
                }

                Thread.Sleep(_random.Next(250, 2500));
                NavigateToMap(force: true);
            }
        }

        private void GoToSleep()
        {
            NavigateToMap();
            foreach (var cell in _configuration.PathToInn)
            {
                ClickOnMap(cell.Trim());
            }
            _driver.Url = $"{BaseUrl}/auberge.php";
            if (!_driver.Url.Contains("auberge.php"))
            {
                Console.WriteLine("Could not open auberge.php");
                return;
            }

            var duree = _driver.FindElementByName("duree");
            duree.Clear();
            duree.SendKeys(_configuration.TimeToSleep);
            _driver.FindElementByCssSelector("input[name =\"Submit\"][value=\"m'endormir\"]").Click();
            
            // Stop looping
            Continue = false;
        }

        private bool IsOtherPlayerPresent()
        {
            return _driver.FindElements(By.CssSelector("a[href^=\"fight.php?type=user\"]")).Any();
        }

        private void ClickOnMap(string coordinates)
        {
            var regex = new Regex(@"^(?<latitude>[A-Z])(?<longitude>[1-3]?[0-9])$");

            if (!regex.IsMatch(coordinates))
            {
                throw new ArgumentException(nameof(coordinates), "Invalid map coordinates.");
            }

            const int cellWidth = 20;

            var match = regex.Match(coordinates);
            var x = int.Parse(match.Groups["longitude"].Value) - 1;
            var y = match.Groups["latitude"].Value[0] - (int)'A';

            Thread.Sleep(TimeSpan.FromSeconds(1)); // wait for flash to load the map before to click on it
            Actions builder = new Actions(_driver);
            var map = _driver.FindElementByCssSelector("td[colspan=\"30\"][rowspan=\"12\"]");
            var xPos = (x * cellWidth) + (cellWidth / 2);
            var yPos = (y * cellWidth) + (cellWidth / 2);
            builder.MoveToElement(map, xPos, yPos);
            builder.Click().Perform();
        }

        private void GrabLoot()
        {
            _driver.FindElementByCssSelector("input[src^=\"systeme/obj\"]").Click();
        }

        private bool IsNewMessagePresent()
        {
            return _driver.FindElements(By.CssSelector("[href=\"/messages.php\"]")).Count > 1;
        }

        private bool IsLootPresent()
        {
            return _driver.FindElements(By.CssSelector("input[src^=\"systeme/obj\"]")).Any();
        }

        private void LevelUp()
        {
            while (_driver.FindElementByName("left").GetAttribute("value") != "0")
            {
                _driver.FindElementByName("Button4").Click();
            }
            _driver.FindElementByName("Submit").Click();
        }

        private void NavigateToMap(bool force = false)
        {
            if (force || !_driver.Url.Contains("map.php"))
            {
                _driver.Url = $"{BaseUrl}/map.php";
            }
        }
    }
}