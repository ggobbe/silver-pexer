using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace SilverPexer
{
    public class Pexer : IDisposable
    {
        private readonly Random _random;

        private readonly IEnumerable<string> _pathToInn;

        private ChromeDriver _driver;

        public Pexer(IEnumerable<string> pathToInn)
        {
            _random = new Random();
            _driver = new ChromeDriver(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "drivers"));
            _driver.Manage().Window.Maximize();
            _pathToInn = pathToInn;
        }

        public void Login(string username, string password)
        {
            _driver.Url = "https://www.silver-world.net";
            _driver.FindElementByName("login").SendKeys(username);
            _driver.FindElementByName("pass").SendKeys(password);
            _driver.FindElementByName("Submit2").Click();
        }

        public void KillAllMonsters()
        {
            while (IsMonsterPresent())
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
                Environment.Exit(0);
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
            while (!IsMonsterPresent())
            {
                if (_driver.Url.Contains("levelup.php"))
                {
                    LevelUp();
                }

                if (IsNewMessagePresent())
                {
                    GoToSleep();
                    Environment.Exit(0);
                }

                if (IsOtherPlayerPresent())
                {
                    GoToSleep();
                    Environment.Exit(0);
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
            foreach (var cell in _pathToInn)
            {
                ClickOnMap(cell.Trim());
            }
            _driver.Url = "https://www.silver-world.net/auberge.php";
            if (!_driver.Url.Contains("auberge.php"))
            {
                Console.WriteLine("Could not open auberge.php");
                return;
            }

            var duree = _driver.FindElementByName("duree");
            duree.Clear();
            duree.SendKeys("24");
            _driver.FindElementByCssSelector("input[name =\"Submit\"][value=\"m'endormir\"]").Click();
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
            var x = match.Groups["latitude"].Value[0] - (int)'A';
            var y = int.Parse(match.Groups["longitude"].Value) - 1;

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
                _driver.Url = "https://www.silver-world.net/map.php";
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _driver.Close();
                    _driver.Dispose();
                    _driver = null;
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}