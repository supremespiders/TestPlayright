using Microsoft.Playwright;
using TestPlayright;

var scraper = new VestiaireCollectiveScraper();
try
{
    await scraper.Work();
}
catch (Exception e)
{
    Console.WriteLine(e);
}
finally
{
    await scraper.Dispose();
}

public static class Extensions
{
    public static async Task<bool> Exist(this IPage page,string selector,int timeout=5000)
    {
        try
        {
            await page.Locator(selector).WaitForAsync(new LocatorWaitForOptions(){Timeout = timeout,State = WaitForSelectorState.Attached});
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}