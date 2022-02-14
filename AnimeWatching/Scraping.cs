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
			var configJson = StructuredDataConfig.ParseJsonString(@"
			{
				'name': '//ul[contains(@class, \'columns\')]/li/a',
				'link': '//ul[contains(@class, \'columns\')]/li/a/@href'
			}
			");
			string website = webClient.DownloadString("http://www.mavanimes.co/tous-les-animes-en-vostfr-fullhd-2/");
			var openScraping = new StructuredDataExtractor(configJson);
			var scrapingResults = openScraping.Extract(website);

			List<Anime> anime = new List<Anime>();
			int count = 0;
			foreach (var item in scrapingResults["name"])
			{
				anime.Add(new Anime());
				anime[count].Name = scrapingResults["name"][count].ToString();
				anime[count].Link = scrapingResults["link"][count].ToString();
				count++;
			}
			webClient.Dispose();
			return anime;
		}

		public List<Episode> SearchEpisode(Anime anime)
		{
			webClient = new WebClient
			{
				Encoding = Encoding.UTF8
			};
			var configJson = StructuredDataConfig.ParseJsonString(@"
			{
				'name': '//h2[contains(@class, \'raees\')]/a',
				'link': '//h2[contains(@class, \'raees\')]/a/@href'
			}
			");
			string website = webClient.DownloadString(anime.Link);
			var openScraping = new StructuredDataExtractor(configJson);
			var scrapingResults = openScraping.Extract(website);

			List<Episode> episode = new List<Episode>();
			if(scrapingResults["name"] != null)
			{
				int count = 0;
				foreach (var item in scrapingResults["name"])
				{
					episode.Add(new Episode());
					episode[count].Name = scrapingResults["name"][count].ToString();
					episode[count].Link = scrapingResults["link"][count].ToString();
					count++;
				}
			}
			webClient.Dispose();
			return episode;
		}

		public string SearchPlayer(Episode episode)
		{
			webClient = new WebClient
			{
				Encoding = Encoding.UTF8
			};
			var configJson = StructuredDataConfig.ParseJsonString(@"
			{
				'link': '//iframe/@src'
			}
			");
			string website = webClient.DownloadString(episode.Link);
			var openScraping = new StructuredDataExtractor(configJson);
			var scrapingResults = openScraping.Extract(website);
			string linkPlayer = null;
			if (scrapingResults["link"] != null)
			{
				foreach (string item in scrapingResults["link"])
				{
					//Ban : mavplayer
					if (item.ToString().Contains("mavplay.") ||
						item.ToString().Contains("gounlimited") ||
						item.ToString().Contains("streamtape") ||
						//item.ToString().Contains("sendvid") ||
						item.ToString().Contains("ok"))
					{
						linkPlayer = item.ToString();
						break;
					}
				}
			}
			webClient.Dispose();
			return linkPlayer;
		}
	}

	public class Anime
	{
		public string Name { get; set; }
		public string Link { get; set; }
	}

	public class Episode
	{
		public string Name { get; set; }
		public string Link { get; set; }
	}
}
