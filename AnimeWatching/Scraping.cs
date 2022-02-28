using Newtonsoft.Json;
using OpenScraping;
using OpenScraping.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnimeWatching
{
	internal class Scraping
	{
		private WebClient webClient;

		public List<Anime> SearchAnime()
		{
			webClient = new WebClient
			{
				Encoding = Encoding.UTF8
			};

			string website = webClient.DownloadString("https://neko-sama.fr/animes-search-vostfr.json");
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
					string[] sr = scrapingResult.ToString().Split('\n');
					foreach(var sr2 in sr)
					{
						if(sr2.Contains("video[0]"))
						{
							result.Add(sr2);
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

		[JsonProperty("url")]
		public string Url { get; set; }
	}

	public class Episode
	{
		[JsonProperty("episode")]
		public string Number { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
