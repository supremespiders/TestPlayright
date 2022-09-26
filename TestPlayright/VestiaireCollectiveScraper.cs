using System.Diagnostics;
using Microsoft.Playwright;

namespace TestPlayright;

public class VestiaireCollectiveScraper
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;

    public async Task StartBrowser(bool headless)
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
         {
             Headless = headless
         });
        _browser = await _playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222",new BrowserTypeConnectOverCDPOptions());
        IBrowserContext context;
        if (File.Exists("session"))
        {
            context = await _browser.NewContextAsync(new()
            {
                StorageStatePath = "session"
            });
        }
        else
        {
            context = await _browser.NewContextAsync();
        }
        _page = await context.NewPageAsync();
    }

    public async Task Attach()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222",new BrowserTypeConnectOverCDPOptions());
        IBrowserContext context;
        context =  _browser.Contexts.First();
        var y = context.Pages.Count;
        _page = context.Pages.First();
    }

    async Task OpenOsBrowser()
    {
        Process proc = new Process();
        proc.StartInfo.FileName = @"C:\Users\Riadh\AppData\Local\ms-playwright\chromium-1024\chrome-win\chrome.exe";
        proc.StartInfo.Arguments = "https://www.intellitect.com/blog/ --new-window --remote-debugging-port=9222";
        proc.Start();
    }
    
    async Task OpenBrowser()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
        {
            Args = new []{"--remote-debugging-port=9222"},
            Headless = false,
        });
        IBrowserContext context;
        if (File.Exists("session"))
        {
            context = await _browser.NewContextAsync(new()
            {
                StorageStatePath = "session"
            });
        }
        else
        {
            context = await _browser.NewContextAsync();
        }
        _page = await context.NewPageAsync();
    }

    async Task SellItem()
    {
        await _page.GotoAsync("https://www.vestiairecollective.com/sell-clothes-online/");
        
    }

    public async Task Work()
    {
        try
        {
           // await OpenOsBrowser();
           // await OpenBrowser();
           // await _page.GotoAsync("https://www.vestiairecollective.com/");
            await Attach();
            await SellItem();
            //await _page.GotoAsync("https://www.vestiairecollective.com/women-bags/handbags/gucci/black-leather-bamboo-bullet-gucci-handbag-25775290.shtml");
            return;
            await StartBrowser(false);
            await _page.GotoAsync("https://www.vestiairecollective.com/");
            if (await _page.Exist("text=Sign in", 1000))
                await Login();
            Console.WriteLine("All good");
        }
        catch (Exception)
        {
            await _page.ScreenshotAsync(new()
            {
                Path = "error.png"
            });
            throw;
        }
    }

    public async Task Dispose()
    {
        await _browser.DisposeAsync();
         _playwright.Dispose();
    }
    
    public async Task Login()
    {
       Console.WriteLine("Start login");
        await _page.Locator("#popin_tc_privacy_button_2").ClickAsync();
        await _page.Locator("text=Sign in").ClickAsync();
        await _page.Locator("#user_email").FillAsync("sales@mydesignerly.com");
        await _page.Locator("#user_password").FillAsync("Upworktemp1!");
        await _page.Locator("[for='remember_me']").ClickAsync();
        await _page.Locator("text=Connect").ClickAsync();
        if(!await _page.Exist("text=Log out"))
            Console.Write("Not connected");
        await _page.Context.StorageStateAsync(new BrowserContextStorageStateOptions() { Path = "session" });
    }
}