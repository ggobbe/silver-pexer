using System;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace SilverPexer
{
    public class Pexer
    {
        private readonly Random _random;
        private readonly ChromeDriver _driver;

        public Pexer()
        {
            _random = new Random();
            _driver = new ChromeDriver(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "drivers"));
            _driver.Manage().Window.Maximize();
        }

        public void Login(string username, string password)
        {
            _driver.Url = "http://www.silver-world.net";
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
            // ClickOnMap(190, 180);
            // ClickOnMap(210, 200);
            // _driver.Url = "http://www.silver-world.net/auberge.php";
            // var duree = _driver.FindElementByName("duree");
            // duree.Clear();
            // duree.SendKeys("24");
            // _driver.FindElementByCssSelector("input[name =\"Submit\"][value=\"m'endormir\"]").Click();
        }

        private bool IsOtherPlayerPresent()
        {
            return _driver.FindElements(By.CssSelector("a[href^=\"fight.php?type=user\"]")).Any();
        }

        private void ClickOnMap(int x, int y)
        {
            Actions builder = new Actions(_driver);
            var map = _driver.FindElementByCssSelector("table[width=\"620\"][border=\"0\"]");
            builder.MoveToElement(map, x, y);
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
                _driver.Url = "http://www.silver-world.net/map.php";
            }
        }
    }
}