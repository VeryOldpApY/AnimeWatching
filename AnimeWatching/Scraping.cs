using Newtonsoft.Json;
using OpenScraping;
using OpenScraping.Config;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AnimeWatching
{
	internal class Scraping
	{
		private WebClient webClient;

		public List<Anime> SearchAnime(string lang)
		{
			webClient = new WebClient
			{
				Encoding = Encoding.UTF8
			};

			string website = null;
			if(lang == "VostFr")
			{
				website = webClient.DownloadString("https://neko-sama.fr/animes-search-vostfr.json");
			}
			else if(lang == "VF")
			{
				website = webClient.DownloadString("https://neko-sama.fr/animes-search-vf.json");
			}
			List<Anime> json = JsonConvert.DeserializeObject<List<Anime>>(website);

			webClient.Dispose();
			return json;
		} 

		public List<Episode> SearchEpisode(Anime anime)
		{
			webClient = new WebClient
			{
				Encoding = Encoding.UTF8
			};

			var configJson = StructuredDataConfig.ParseJsonString(/*lang=json*/ @"
			{
				'script': '//script[contains(@type, \'text/javascript\')]'
			}
			");
			string website = webClient.DownloadString("https://neko-sama.fr/" + anime.Url);
			var openScraping = new StructuredDataExtractor(configJson);
			var scrapingResults = openScraping.Extract(website);

			List<Episode> episodeList = new List<Episode>();
			try
			{
				string result = null;
				foreach(var scrapingResult in scrapingResults["script"])
				{
					string[] script = scrapingResult.ToString().Split('\n');
					foreach(var line in script)
					{
						if(line.Contains("var episodes ="))
						{
							result = line;
						}
					}
				}
				char[] charsToTrim = { ' ', ';' };
				string json = result.Split('=')[1].Trim(charsToTrim);

				episodeList = JsonConvert.DeserializeObject<List<Episode>>(json);
			}
			catch
			{
			}
			webClient.Dispose();
			return episodeList;
		}

		public string SearchPlayer(Episode episode)
		{
			webClient = new WebClient
			{
				Encoding = Encoding.UTF8
			};
			var configJson = StructuredDataConfig.ParseJsonString(/*lang=json*/ @"
			{
				'script': '//script[contains(@type, \'text/javascript\')]'
			}
			");
			string website = webClient.DownloadString("https://neko-sama.fr/" + episode.Url);
			var openScraping = new StructuredDataExtractor(configJson);
			var scrapingResults = openScraping.Extract(website);
			string urlPlayer;

			List<string> result = new List<string>();

			foreach(var scrapingResult in scrapingResults["script"])
			{
				if(scrapingResult.ToString() != "")
				{
					foreach(var line in scrapingResult.ToString().Split('\n'))
					{
						if(line.Contains("video[0]"))
						{
							result.Add(line);
						}
					}
				}
			}
			char[] charsToTrim = { '\'', ' ', ';' };
			urlPlayer = result[1].Split('=')[1].Trim(charsToTrim);

			webClient.Dispose();
			return urlPlayer;
		}
	}

	public class Anime
	{
		[JsonProperty("title")]
		public string Name { get; set; }

		[JsonProperty("title_romanji")]
		public string OtherName { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("url_image")]
		public string UrlImg { get; set; }
	}

	public class Episode
	{
		[JsonProperty("episode")]
		public string Number { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

        [JsonProperty("title")]
        public string Name { get; set; }
    }
}
