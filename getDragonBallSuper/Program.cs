// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using System.Net;
using System.Text;

string html;
// obtain some arbitrary html....



for (int volume = 1; volume <= 83; volume++)
{
    string url = $"https://planetadragonball.com/dragon-ball-super-manga-{volume}-espanol/";

    using (var client = new WebClient())
    {
        html = client.DownloadString(url);
    }

    HtmlDocument doc = new HtmlDocument();
    doc.LoadHtml(html);

    HtmlNodeCollection figureOfImgs = doc.DocumentNode.SelectNodes("//figure[@class='wp-block-image']//img");


}