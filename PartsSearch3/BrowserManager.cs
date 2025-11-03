using PuppeteerSharp;

namespace PartsSearch3;

public sealed class BrowserManager
{
    private static readonly Lazy<BrowserManager> _instance = new Lazy<BrowserManager>(() => new BrowserManager());
    private IBrowser? _browser;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private bool _isInitialized = false;

    private BrowserManager() { }

    public static BrowserManager Instance => _instance.Value;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        await _lock.WaitAsync();
        try
        {
            if (_isInitialized)
                return;

           // Console.WriteLine("Initializing browser...");
            
            var launchOptions = new LaunchOptions
            {
                Headless = true,
                Args = new[] { 
                    "--no-sandbox", 
                    "--disable-setuid-sandbox",
                    "--disable-features=IsolateOrigins,site-per-process"
                },
                Timeout = 60000
            };

            var browserFetcher = new BrowserFetcher();
            var installedBrowsers = browserFetcher.GetInstalledBrowsers();
            
            if (!installedBrowsers.Any())
            {
                // First try system Chrome before downloading
                var systemChromePaths = new[]
                {
                    "/usr/bin/google-chrome",
                    "/usr/bin/chromium",
                    "/usr/bin/chromium-browser",
                    "/snap/bin/chromium",
                    "/usr/bin/google-chrome-stable"
                };
                
                foreach (var path in systemChromePaths)
                {
                    if (File.Exists(path))
                    {
                        Console.WriteLine($"Using system Chrome: {path}");
                        launchOptions.ExecutablePath = path;
                        break;
                    }
                }
                
                // Only attempt download if no system Chrome found
                if (string.IsNullOrEmpty(launchOptions.ExecutablePath))
                {
                    Console.WriteLine("No Chrome installation found.");
                    Console.WriteLine("Attempting to download Chrome...");
                    
                    var downloadTask = Task.Run(async () => await browserFetcher.DownloadAsync());
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
                    
                    var completedTask = await Task.WhenAny(downloadTask, timeoutTask);
                    
                    if (completedTask == timeoutTask)
                    {
                        Console.WriteLine("Download timed out.");
                        throw new Exception(
                            "No Chrome browser found and download failed. Please install:\n" +
                            "  sudo apt install chromium-browser");
                    }
                    else if (downloadTask.IsFaulted)
                    {
                        Console.WriteLine($"Download failed: {downloadTask.Exception?.InnerException?.Message}");
                        throw new Exception(
                            "No Chrome browser found and download failed. Please install:\n" +
                            "  sudo apt install chromium-browser");
                    }
                    else
                    {
                        Console.WriteLine("Download complete.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Using cached Chrome installation.");
            }
            
            Console.WriteLine("Launching browser...");
            _browser = await Puppeteer.LaunchAsync(launchOptions);

            _isInitialized = true;
            Console.WriteLine("Browser initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize browser: {ex.Message}");
            _isInitialized = false;
            throw;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IPage> GetNewPageAsync()
    {
        if (!_isInitialized || _browser == null)
        {
            await InitializeAsync();
        }

        var page = await _browser!.NewPageAsync();
        
        await page.SetUserAgentAsync(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/120.0.0.0 Safari/537.36"
        );
        
        await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
        {
            { "Accept-Language", "en-US,en;q=0.9" }
        });

        return page;
    }

    public async Task CloseAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
            _isInitialized = false;
        }
    }
}
