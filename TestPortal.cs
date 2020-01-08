using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Firefox;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

namespace Lobanov_TestPortal
{
    class TestPortal
    {
        IWebDriver driver;

        [SetUp]
        public void startBrowser()
        {
            driver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Test]
        public void LoginLogout()
        {
            string email = "ultramofo+ask_ev@gmail.com";
            string pass = "sERVERS12345";

            bool loggedIn = false;
            bool loggedOut = false;

            TestContext.WriteLine("Logging in..");

            try
            {
                driver.Url = "https://portal.servers.com/";
                IWebElement element = driver.FindElement(By.XPath("//form[@class='login-form']//input[@type='email']"));
                element.SendKeys(email);
                element = driver.FindElement(By.XPath("//form[@class='login-form']//input[@type='password']"));
                element.SendKeys(pass);
                element = driver.FindElement(By.XPath("//form[@class='login-form']//button[@type='submit']"));

                element.Click();

                element = driver.FindElement(By.XPath("//div[@class='cjihdxx']/button"));

                if (element.Text.ToLower() == email.ToLower())
                {
                    loggedIn = true;
                    TestContext.WriteLine($"Logged in as {element.Text}");
                }


                TestContext.Write("Logging out..");
                driver.Url = "https://portal.servers.com/logout";

                if (driver.FindElement(By.XPath("//form[@class='login-form']")).Displayed) loggedOut = true;
                TestContext.WriteLine(" done");
            }
            catch (Exception e)
            {
                TestContext.WriteLine(e.Message);
            }

            Assert.Multiple(() =>
            {
                Assert.IsTrue(loggedIn);
                Assert.IsTrue(loggedOut);
            });


        }

        [Test]
        public void Profile()
        {
            string email = "ask_ev_" + Steps.RandomString(10) + "@gmail.com";

            bool userCreated = false;
            bool profileFilled = false;

            TestContext.WriteLine($"Creating new user {email}..");

            try
            {
                driver.Url = "https://portal.servers.com/registration";

                IWebElement element = driver.FindElement(By.XPath("//form[@class='login-form']//input[@name='accept_privacy_policy_and_terms_of_use']"));
                if (!element.Selected) element.Click();

                element = driver.FindElement(By.XPath("//form[@class='login-form']//input[@type='email']"));
                element.SendKeys(email + Keys.Enter);


                element = driver.FindElement(By.XPath("//div[@class='cjihdxx']/button"));

                TestContext.WriteLine($"Logged in as {element.Text}");

                if (element.Text.ToLower() == email.ToLower())
                {
                    userCreated = true;                    
                }

                TestContext.Write("Filling profile..");
                driver.Url = "https://portal.servers.com/#/profile/info/edit";

                string billingAddressStreet = Steps.RandomString(150);

                element = driver.FindElement(By.XPath("//div[@class='midd0eu']//input[@name='fname']"));
                element.SendKeys(Steps.RandomString(100));

                element = driver.FindElement(By.XPath("//div[@class='midd0eu']//input[@name='lname']"));
                element.SendKeys(Steps.RandomString(100));

                element = driver.FindElement(By.XPath("//div[@class='midd0eu']//input[@name='phone_number']"));
                element.SendKeys("1" + Steps.RandomNumString(14));

                element = driver.FindElement(By.XPath("//div[@id='billing_address_country']//div[contains(@class, 'select__control')]"));
                Actions move = new Actions(driver);
                move.MoveToElement(element).Click().SendKeys("Andorra" + Keys.Return).Perform();

                element = driver.FindElement(By.XPath("//div[@class='midd0eu']//input[@name='billing_address_city']"));
                element.SendKeys(Steps.RandomString(100));

                element = driver.FindElement(By.XPath("//div[@class='midd0eu']//input[@name='billing_address_postalcode']"));
                element.SendKeys(Steps.RandomNumString(20));

                element = driver.FindElement(By.XPath("//div[@class='midd0eu']//input[@name='billing_address_street']"));
                element.SendKeys(billingAddressStreet);

                element = driver.FindElement(By.XPath("//div[@class='midd0eu']//button[@title='Save']"));
                element.Click();

                

                try
                {
                    element = driver.FindElement(By.XPath($"//*[contains(text(), '{billingAddressStreet}')]"));
                    profileFilled = true;
                    TestContext.WriteLine(" done");
                }
                catch 
                {
                    TestContext.WriteLine("could not find created billing address in profile info");
                }

                TestContext.Write("Logging out..");
                driver.Url = "https://portal.servers.com/logout";
                TestContext.WriteLine(" done");

            }
            catch (Exception e)
            {
                TestContext.WriteLine(e.Message);
            }

            Assert.Multiple(() =>
            {
                Assert.IsTrue(userCreated);
                Assert.IsTrue(profileFilled);
            });


        }

        [TearDown]
        public void closeBrowser()
        {
            driver.Quit();
        }

    }
}
