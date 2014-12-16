using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Web.Http;

namespace 周公解梦
{
    public static class DreamHelper
    {
        private const string BaseUri = @"http://api.uihoo.com/dream/dream.http.php?key={0}&format=xml";

        public static async Task<IReadOnlyCollection<string>> GetResult(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new ReadOnlyCollection<string>(new List<string>());
            }

            try
            {
                string url = string.Format(BaseUri, WebUtility.UrlEncode(key));
                Uri uri = new Uri(url);
                HttpClient client = new HttpClient();
                var xml = await client.GetStringAsync(uri);

                if (string.IsNullOrEmpty(xml))
                {
                    return new ReadOnlyCollection<string>(new List<string>());
                }
                client.Dispose();

                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                var items = document.SelectNodes("//item");
                var list = new List<string>();
                foreach (var item in items)
                {
                    var temp = item.InnerText.Replace("\r", "").Replace("\n", "");
                    if (string.IsNullOrEmpty(temp))
                    {
                        continue;
                    }
                    list.Add(temp);
                }
                return new ReadOnlyCollection<string>(list);
            }
            catch
            {
                return new ReadOnlyCollection<string>(new List<string>());
            }
        }
    }
}
