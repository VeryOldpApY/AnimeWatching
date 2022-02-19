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

			var configJson = StructuredDataConfig.ParseJsonString(@"
			{
				'name': '//div[contains(@class, \'text\')]/a/div',
				'link': '//div[contains(@class, \'text\')]/a/@href'
			}
			");

			string website = webClient.DownloadString("https://neko-sama.fr/" + anime.Url);
			var openScraping = new StructuredDataExtractor(configJson);
			var scrapingResults = openScraping.Extract(website);

			List<Episode> episodeList = new List<Episode>();
			try
			{
				string name = scrapingResults["name"][0].ToString();
				string link = "https://neko-sama.fr" + scrapingResults["link"][0].ToString();

				if(name != null)
				{
					for(int i = 0; i < Convert.ToInt16(anime.Numbers.Split(' ')[0]); i++)
					{
						string[] separatingStrings = { "-01-" };
						string[] linkNE = link.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
						string nb;
						if(i < 9)
						{
							nb = "0" + (i + 1);
						}
						else
						{
							nb = (i + 1).ToString();
						}
					
						Episode episode = new Episode
						{
							Name = name,
							Number = "Ep " + nb,
							Link = linkNE[0] + "-" + nb + "-vostfr"
						};
						episodeList.Add(episode);
					}
				}
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
			var configJson = StructuredDataConfig.ParseJsonString(@"
			{
				'script': '//script[contains(@type, \'text/javascript\')]'
			}
			");
			string website = webClient.DownloadString(episode.Link);
			var openScraping = new StructuredDataExtractor(configJson);
			var scrapingResults = openScraping.Extract(website);
			string urlPlayer = null;

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

		[JsonProperty("nb_eps")]
		public string Numbers { get; set; }
	}

	public class Episode
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("number")]
		public string Number { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }
	}
}
