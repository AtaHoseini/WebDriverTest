using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace WebDriverTest
{
    class Program
    {
        enum MessageType
        {
            INFO,
            ERROR,
            WARNING
        }
       
        //thats because of slow internet connection in IRAN
        static readonly int TIMEOUT = 900;
        static void Main(string[] args)
        {
            string department = string.Empty;
            string language = string.Empty;
            if (args.Length == 2)
            {
                department = args[0];
                language = args[1];
            }
            else
            {
                PrintMessage("Please Enter the department and language to count vacancies.", MessageType.ERROR);
                PrintMessage("USAGE: WebDriverTest.exe department_name language\n", MessageType.INFO);
                Console.ReadKey();
                Environment.Exit(0);
            }
            SearchForVacanciesAndWait(department, language);
        }

        /// <summary>
        /// this function is made in order to count the vacancies acording to desired criteria
        /// </summary>
        /// <param name="department"></param>
        /// <param name="language"></param>
        static void SearchForVacanciesAndWait(string department, string language)
        {
            try
            {
                string app_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string gecko_driver_path = System.IO.Path.Combine(app_dir, "geckodriver.exe");

                if (!System.IO.File.Exists(gecko_driver_path))
                {
                    PrintMessage("Oops, Geckodriver could not be found beside the app...\n please download it from: https://github.com/mozilla/geckodriver/releases", MessageType.WARNING);
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                IWebDriver driver;
                driver = new FirefoxDriver(app_dir, new FirefoxOptions(), TimeSpan.FromSeconds(TIMEOUT));
                driver.Url = "https://cz.careers.veeam.com/vacancies";
                driver.Manage().Window.Maximize();

                IWebElement all_departments_button = driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div/div[1]/div/div[2]/div/div/button"));
                all_departments_button.Click();

                var btns = driver.FindElements(By.ClassName("dropdown-item"));

                // click on desired department
                foreach (var btn in btns)
                {
                    if (btn.Text == department)
                    {
                        btn.Click();
                        break;
                    }
                }

                IWebElement all_languages_button = driver.FindElement(By.XPath("/html/body/div[1]/div/div[1]/div/div/div[1]/div/div[3]/div/div/button"));
                all_languages_button.Click();

                var languages_btns = driver.FindElements(By.ClassName("custom-checkbox"));

                // click on desired language
                foreach (var btn in languages_btns)
                {
                    if (btn.Text == language)
                    {
                        btn.Click();
                        break;
                    }
                }

                int cardsCount = driver.FindElements(By.ClassName("card")).Count;
                PrintMessage(string.Format(">>>>>> The Count of Vacancies is : {0}", cardsCount), MessageType.INFO);
                PrintMessage("To check the count you can go to openned FireFox else press any key to exit", MessageType.WARNING);
                Console.ReadKey();

                driver.Close();
                driver.Quit();
            }
            catch (Exception ex)
            {
                PrintMessage(ex.Message, MessageType.ERROR);
            }

        }

        static void PrintMessage(string message, MessageType messageType)
        {
            ConsoleColor fore_color;
            switch (messageType)
            {
                case MessageType.INFO:
                    fore_color = ConsoleColor.Green;
                    break;
                case MessageType.ERROR:
                    fore_color = ConsoleColor.Red;
                    break;
                case MessageType.WARNING:
                    fore_color = ConsoleColor.DarkYellow;
                    break;
                default:
                    fore_color = ConsoleColor.Green;
                    break;
            }

            Console.ForegroundColor = fore_color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
