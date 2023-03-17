using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.IO.Compression;


int startingVolume = 1;

string comicImgsDirBase = @"C:\Users\manue\source\repos\getDragonBallSuper\getDragonBallSuper\download\";
string zipDir = @"C:\Users\manue\source\repos\getDragonBallSuper\getDragonBallSuper\cbr\";

// creating download directory 
try
{
    if (Directory.Exists(comicImgsDirBase))
    {
        Directory.Delete(comicImgsDirBase, true);
    }
    Directory.CreateDirectory(comicImgsDirBase);
}
catch (Exception e)
{
    Console.WriteLine("The process failed: {0}", e.ToString());
    System.Environment.Exit(1);
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


try
{
    int volume = startingVolume < 1 ? 1 : startingVolume;

    while(true)
    {
        string html;
        string url;
        string baseName = "dbs " + volume.ToString().PadLeft(3, '0');
        string comicImgsDir = comicImgsDirBase + baseName;

        if (volume > 0 && volume <= 58)
        {
            url = $"https://planetadragonball.com/dragon-ball-super-manga-{volume}-espanol/";
        }
        else
        {
            url = $"https://radardeldragon.com/dragon-ball-super-manga-{volume}-espanol/";
        }

        // getting HTML 
        using (var client = new WebClient())
        {
            html = client.DownloadString(url);
        }

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        HtmlNodeCollection imgs;

        imgs = doc.DocumentNode.SelectNodes("//figure[@class='wp-block-image']//img");

        if (imgs == null || imgs.Count == 0)
        {
            imgs = doc.DocumentNode.SelectNodes("//div[@class='entry-content clear']//img");
        }

        // create imgs folders
        try
        {
            Directory.CreateDirectory(comicImgsDir);
        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
            System.Environment.Exit(1);
        }

        // download imgs
        for (int i = 0; i < imgs.Count; i++)
        {
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

        //compress dir
        string cbrPath = zipDir + baseName + ".cbr";
        Console.WriteLine("creating ".PadRight(15) + cbrPath);
        ZipFile.CreateFromDirectory(comicImgsDir, cbrPath);

        volume++;

    }
}
catch (System.Net.WebException ex)
{
    Console.WriteLine(ex);
}
finally
{
    Console.WriteLine("This is all");
}