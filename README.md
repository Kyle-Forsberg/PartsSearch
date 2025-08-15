# PartsSearch CLI

PartsSearch is a small **CLI tool written in C#** that retrieves product listings for car parts (mainly German cars) from several popular vendors.  

Supported Vendors
- UroTuning  
- FCP Euro  
- Kerma TDI  
- ECS Tuning  

More vendors may be supported in the future. Amazon is intentionally excluded to guide users towards certified parts from smaller, trusted vendors.

Usage
Run the tool with a part number:

```bash
./PartsSearch <PartNumber>
```

After a few seconds, a list of available listings will appear, showing:

- Price

- Manufacturer

- Link to the product

Using the part's regular name may give mixed results. For example:
./PartsSearch VR6 Ignition Coil may return unrelated parts like thermostat housings or timing kits.
Using the part number (e.g., ./PartsSearch 021905106) yields more accurate results.

## Important Notes

- Fitment is not verified: This tool relies on each vendor's search function and does not guarantee compatibility with a specific vehicle.

- This program does not crawl multiple pages. Only one page per vendor is accessed, keeping traffic equivalent to a regular user search.

- This program requires downloading a Chromium instance via PuppeteerSharp. This may slow down the initial runtime.
