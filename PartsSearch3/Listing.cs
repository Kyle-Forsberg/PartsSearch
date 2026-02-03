namespace PartsSearch3;

public class Listing
{
    
    public string? Partnumber { get; set; }
    public string? Link { get; set; }
    public string? Brand { get; set; }
    public double Price { get; set; } 

    //moved these around to work better with the JS
    public Listing() { }

    public Listing(string partnumber, string link, string? brand, double price)
    {
        this.Partnumber = partnumber;
        this.Link = link;
        this.Brand = brand;
        this.Price = price;
    }

    public Listing(string partnumber, string link, double price)
    {
        this.Partnumber = partnumber;
        this.Link = link;
        this.Price = price;
    }

    public Listing(string link, double price)
    {
        this.Link = link;
        this.Price = price;
    }

    public override string ToString()
    {
        if (this.Brand!=null)
        {
            return $"{this.Price.ToString("F2").PadRight(8)}: {this.Brand.PadRight(20)} | {this.Link}";
        }
        return $"{this.Price} | {this.Link}";
    }
}
