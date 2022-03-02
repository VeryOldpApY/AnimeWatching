using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AnimeWatching
{
	public partial class MainWindow : Window
	{
		private List<Anime> animeList = new List<Anime>();
		private List<Episode> episodeList = new List<Episode>();
		readonly Scraping scraping = new Scraping();
		private string vers;

		public MainWindow()
		{
			InitializeComponent();
			animeList = scraping.SearchAnime("VostFr");
			foreach(Anime anime in animeList)
			{
				lv_Anime.Items.Add(anime.Name);
			}
		}



		private void Lv_Search_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			string selectedAnime = lv_Anime.SelectedItem.ToString();
			//Episode
			if (episodeList.Count > 0)
			{
				Episode episode = new Episode();
				foreach (Episode episodeItem in episodeList)
				{
					if (selectedAnime.Contains(episodeItem.Number))
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
					MessageBox.Show("Error, No video available !");
				}
			}
			//Anime
			else
			{
				Anime anime = new Anime();
				foreach (Anime animeItem in animeList)
				{
					if (animeItem.Name.ToUpper() == selectedAnime.ToUpper())
					{
						anime = animeItem;
						break;
					}
				}
				episodeList = scraping.SearchEpisode(anime);
				if(episodeList.Count > 0)
				{
					lv_Anime.Items.Clear();
					foreach(Episode episode in episodeList)
					{
						lv_Anime.Items.Add(anime.Name + " - " + episode.Number);
					}
				}
				else
				{
					MessageBox.Show("Error, No episodes available !");
				}
			}
		}
		private void Bt_Search_Click(object sender, RoutedEventArgs e)
		{
			episodeList.Clear();
			lv_Anime.Items.Clear();
			string search = tb_Search.Text;
			foreach(Anime anime in animeList)
			{
				if(anime.OtherName == null)
				{
					anime.OtherName = "";
				}
				if(anime.Name.ToUpper().Contains(search.ToUpper()) || anime.OtherName.ToUpper().Contains(search.ToUpper()))
				{
					lv_Anime.Items.Add(anime.Name);
				}
			}
		}

		private void Tb_Search_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			Bt_Search_Click(sender, e);
		}
		private void Cb_VersAnime_DropDownClosed(object sender, System.EventArgs e)
		{
			episodeList.Clear();
			tb_Search.Clear();
			vers = cb_VersAnime.Text;
			lv_Anime.Items.Clear();
			animeList = scraping.SearchAnime(vers);
			foreach(Anime anime in animeList)
			{
				lv_Anime.Items.Add(anime.Name);
			}
		}

		private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void Bt_Close_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
