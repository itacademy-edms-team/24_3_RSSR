using System.ServiceModel.Syndication;
using System.Xml;
using HtmlAgilityPack;

public class RssDiscoverer
{
    public static async Task<List<string>> FindRssFeedsAsync(string url)
    {
        List<string> rssFeeds = new List<string>();

        try
        {
            using (HttpClient client = new HttpClient())
            {
                
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); 
                string html = await response.Content.ReadAsStringAsync();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                var linkTags = doc.DocumentNode.SelectNodes("//link[@rel='alternate' and @type='application/rss+xml']");

                if (linkTags != null)
                {
                    foreach (var linkTag in linkTags)
                    {
                        string hrefValue = linkTag.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrEmpty(hrefValue))
                        {
                            rssFeeds.Add(hrefValue);
                            Console.WriteLine(hrefValue);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка нахождения RSS feeds: {ex.Message}");
        }

        return rssFeeds;
    }
}