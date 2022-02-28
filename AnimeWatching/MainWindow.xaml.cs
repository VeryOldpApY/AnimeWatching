using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimeWatching
{

	public partial class MainWindow : Window
	{
		readonly List<Anime> animeList = new List<Anime>();
		private List<Episode> episodeList = new List<Episode>();
		readonly Scraping scraping = new Scraping();

		public MainWindow()
		{
			InitializeComponent();
			animeList = scraping.SearchAnime();
			foreach (Anime anime in animeList)
			{
				lv_Search.Items.Add(anime.Name);
			}
		}


		private void Bt_Search_Click(object sender, RoutedEventArgs e)
		{
			episodeList.Clear();
			lv_Search.Items.Clear();
			string search = tb_Search.Text;
			foreach(Anime anime in animeList)
			{
				if(anime.Name.ToUpper().Contains(search.ToUpper()))
				{
					lv_Search.Items.Add(anime.Name);
				}
			}
		}

		private void Lv_Search_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(lv_Search.SelectedItem != null)
			{
				string search = lv_Search.SelectedItem.ToString();
				//Episode
				if (episodeList.Count > 0)
				{
					Episode episode = new Episode();
					foreach (Episode episodeItem in episodeList)
					{
						if (search.Contains(episodeItem.Number))
						{
							episode = episodeItem;
						}
					}
					string linkPlayer = scraping.SearchPlayer(episode);
					if(linkPlayer != null)
					{
						PlayerWindow playerWindow = new PlayerWindow(linkPlayer);
						this.Visibility = Visibility.Hidden;
						playerWindow.ShowDialog();
						this.Visibility = Visibility.Visible;
					}
					else
					{
						MessageBox.Show("Error, L'épisode est indisponible !");
					}
				}
				//Anime
				else
				{
					Anime anime = new Anime();
					foreach (Anime animeItem in animeList)
					{
						if (animeItem.Name.ToUpper() == search.ToUpper())
						{
							anime = animeItem;
							break;
						}
					}
					episodeList = scraping.SearchEpisode(anime);
					if(episodeList.Count > 0)
					{
						lv_Search.Items.Clear();
						foreach (Episode episode in episodeList)
						{
							lv_Search.Items.Add(anime.Name + " - " + episode.Number);
						}
					}
					else
					{
						MessageBox.Show("Error, aucun épisodes disponible !");
					}
				}
			}
		}

		private void Tb_Search_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			Bt_Search_Click(sender, e);
		}
	}
}
