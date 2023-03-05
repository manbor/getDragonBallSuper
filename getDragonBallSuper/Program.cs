﻿// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using System.Net;
using System.Text;

string html;
string pathBase = @"C:\Users\manue\source\repos\getDragonBallSuper\getDragonBallSuper\download\";
string zipDir = @"C:\Users\manue\source\repos\getDragonBallSuper\getDragonBallSuper\cbr\";
string path;


for (int volume = 1; volume <= 83; volume++)
{
    string url = $"https://planetadragonball.com/dragon-ball-super-manga-{volume}-espanol/";
    path = pathBase + "dbs " + volume.ToString().PadLeft(3, '0');


    // getting HTML 
    using (var client = new WebClient())
    {
        html = client.DownloadString(url);
    }
    HtmlDocument doc = new HtmlDocument();
    doc.LoadHtml(html);
    HtmlNodeCollection imgs = doc.DocumentNode.SelectNodes("//figure[@class='wp-block-image']//img");


    // creating directory 
    try
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
    }
    catch (Exception e)
    {
        Console.WriteLine("The process failed: {0}", e.ToString());
        System.Environment.Exit(1);
    }


    // download img
    for (int i = 0; i < imgs.Count; i++) {

        string imgPath = path + "\\img_" + i.ToString().PadLeft(3, '0') + ".jpeg";

        var src = 
            (
                from att in imgs.ElementAt(i).GetAttributes()
                where att.Name.Equals("src")
                select att.Value
            ).First()
                ;

        Console.WriteLine(imgPath);

        using (WebClient client = new WebClient())
        {
            client.DownloadFile(new Uri(src), imgPath);
        }

        

    }
}