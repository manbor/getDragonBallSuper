using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.IO.Compression;


string comicImgsDirBase = @"C:\Users\manue\source\repos\getDragonBallSuper\getDragonBallSuper\download\";
string zipDir = @"C:\Users\manue\source\repos\getDragonBallSuper\getDragonBallSuper\cbr\";


for (int volume = 1; volume <= 83; volume++)
{
    string html;
    string url = $"https://planetadragonball.com/dragon-ball-super-manga-{volume}-espanol/";
    string baseName = "dbs " + volume.ToString().PadLeft(3, '0');
    string comicImgsDir = comicImgsDirBase + baseName;


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
        if (Directory.Exists(comicImgsDir))
        {
            Directory.Delete(comicImgsDir, true);
        }
        Directory.CreateDirectory(comicImgsDir);
    }
    catch (Exception e)
    {
        Console.WriteLine("The process failed: {0}", e.ToString());
        System.Environment.Exit(1);
    }


    // download imgs
    for (int i = 0; i < imgs.Count; i++) {
        string imgNameWithPah = comicImgsDir + "\\img_" + i.ToString().PadLeft(3, '0') + ".jpeg";

        var src = 
            (
                from att in imgs.ElementAt(i).GetAttributes()
                where att.Name.Equals("src")
                select att.Value
            ).First()
                ;

        Console.WriteLine("getting ".PadRight(15) + imgNameWithPah);

        using (WebClient client = new WebClient())
        {
            client.DownloadFile(new Uri(src), imgNameWithPah);
        }
    }
    

    // creating cbr directory 
    try
    {
        if (Directory.Exists(zipDir))
        {
            Directory.Delete(zipDir, true);
        }
        Directory.CreateDirectory(zipDir);
    }
    catch (Exception e)
    {
        Console.WriteLine("The process failed: {0}", e.ToString());
        System.Environment.Exit(1);
    }


    //compress dir
    string cbrPath = zipDir + baseName + ".cbr";
    Console.WriteLine("creating ".PadRight(15) + cbrPath)
    ZipFile.CreateFromDirectory(comicImgsDir, cbrPath);

} 