using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;

namespace AnimeWatching
{
	public partial class MainWindow : Window
	{
		private List<Anime> animeList = new List<Anime>();
		private List<Episode> episodeList = new List<Episode>();
		readonly Scraping scraping = new Scraping();
		private string vers;
		public Visibility ViImg { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			try
			{
				CoreWebView2Environment.GetAvailableBrowserVersionString();
			}
			catch
			{
				MessageBox.Show("WebView2 Require !");
				Application.Current.Shutdown();
			}
			
			animeList = scraping.SearchAnime("VostFr");
			lv_Anime.ItemsSource = animeList;
			//DataTemplate dataTemplate = lv_Anime.DataContext as DataTemplate;
		}

		private void Lv_Search_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//Episode
			if (episodeList.Count > 0)
			{
				Episode selectedEpisode = (Episode)lv_Anime.SelectedItem;
				string linkPlayer = scraping.SearchPlayer(selectedEpisode);
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
				Anime selectedAnime = (Anime)lv_Anime.SelectedItem;
				episodeList = scraping.SearchEpisode(selectedAnime);
				if(episodeList.Count > 0)
				{
					lv_Anime.ItemsSource = episodeList;
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
			lv_Anime.ItemsSource = null;
			string search = tb_Search.Text;
			if(search == null)
			{
				search = "";
			}
			List<Anime> animeSearch = new List<Anime>();
			foreach(Anime anime in animeList)
			{
				if(anime.OtherName == null)
				{
					anime.OtherName = "";
				}
				if(anime.Name.ToUpper().Contains(search.ToUpper()) || anime.OtherName.ToUpper().Contains(search.ToUpper()))
				{
					animeSearch.Add(anime);
				}
			}
			lv_Anime.ItemsSource = animeSearch;
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
			animeList = scraping.SearchAnime(vers);
			lv_Anime.ItemsSource = animeList;
		}

		private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void Bt_Close_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

        private void Bt_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
