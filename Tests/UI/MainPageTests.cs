using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Tests.UI;

public class MainPageTests
{
    private const string baseUrl = "https://localhost:5001/";

    [Fact]
    public void MainPage_Loaded_WithSucces()
    {
        var driver = new ChromeDriver();
        driver.Navigate().GoToUrl(baseUrl);
        Assert.Equal("Home", driver.Title);

        var heading = driver.FindElement(By.TagName("h2"));
        Assert.Equal("Hello, football fans!", heading.Text);

        var header3 = driver.FindElement(By.TagName("h3"));
        Assert.Equal("Welcome to your simple UEFA ranking app for 2025", header3.Text);
    }
}
