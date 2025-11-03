# PartsSearch CLI

PartsSearch is a small **CLI tool written in C#** that retrieves product listings for car parts (mainly German cars) from several popular vendors.  
Currently, this only runs on Linunx, with a Mac implementation to be ironed out in the future, and windows after that. 

I wrote this tool to initially help me find parts for my MK4 Volkswagens very quickly, so the stack of sites I used were mostly tailored to that.
Of course, these sites also carry parts for many other European vehicles, so don't be too discouraged if you are looking for parts for a Volvo or something. 

Supported Vendors
- UroTuning  
- FCP Euro  
- Kerma TDI  
- ECS Tuning  

More vendors may be supported in the future, so keep an eye out. 

Usage
Run with a part number:

```bash
./partssearch <partnumber>
```

After a few seconds, a list of available listings will appear, showing:

- Price

- Manufacturer

- Link to the product

Using the part's regular name may give mixed results. For example:
dotnet run VR6 Ignition Coil may return unrelated parts like thermostat housings or timing kits.
Using the part number (e.g. dotnet run 021905106) yields more accurate results.

unfortunately, puppeteer sharp has been quite difficult to get running with a published version of this program outside of linux, so if you are experienced with Puppeteer and want to help out, I am always open to feedback or help. 

## Important Notes

- Fitment is not verified. This tool relies on each vendor's search function and does not guarantee compatibility with a specific vehicle.

- This program does not crawl multiple pages. Only one page per vendor is accessed, keeping traffic equivalent to a regular user search.

- This program requires downloading a Chromium instance via PuppeteerSharp. This may slow down the initial runtime.

- This program will appear quite slow if your internet connection is poor, as it still needs to load multiple webpages and potentially download chromium as well.
