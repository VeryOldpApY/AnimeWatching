using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;

namespace AnimeWatching
{
	public partial class MainWindow : Window
	{
		private List<Anime> animeList = new List<Anime>();
		private List<Episode> episodeList = new List<Episode>();
		readonly Scraping scraping = new Scraping();

		public MainWindow()
		{
			InitializeComponent();
			#if DEBUG
				System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
			#endif
            try
            {
				CoreWebView2Environment.GetAvailableBrowserVersionString();
			}
			catch
			{
				MessageBox.Show("WebView2 Require !");
				Application.Current.Shutdown();
			}
			AnimeShow("VostFr");
		}

		private void AnimeShow(string lang)
		{
            animeList = scraping.SearchAnime(lang);
            List<ListViewItem> animes = new List<ListViewItem>();
            foreach(Anime anime in animeList)
            {
                ListViewItem animeItem = new ListViewItem
                {
                    ContentTemplate = (DataTemplate)FindResource("AnimeListTemplate"),
                    Content = anime
                };
                animes.Add(animeItem);
            }
            lv_Anime.ItemsSource = animes;
        }

		private void Lv_Search_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//Episode
			if (episodeList.Count > 0)
			{
                ListViewItem selectedEpisode = (ListViewItem)lv_Anime.SelectedItem;
                Episode episode = (Episode)selectedEpisode.Content;
				string linkPlayer = scraping.SearchPlayer(episode);
				if(linkPlayer != null)
				{
					PlayerWindow playerWindow = new PlayerWindow(linkPlayer);
					Visibility = Visibility.Hidden;
					playerWindow.ShowDialog();
					Visibility = Visibility.Visible;
				}
				else
				{
					MessageBox.Show("Error, No video available !");
				}
			}
			//Anime
			else
			{
                ListViewItem selectedAnime = (ListViewItem)lv_Anime.SelectedItem;
				Anime anime = (Anime)selectedAnime.Content;
				episodeList = scraping.SearchEpisode(anime);
				if(episodeList.Count > 0)
				{
                    List<ListViewItem> episodes = new List<ListViewItem>();
                    foreach(Episode episode in episodeList)
                    {
                        ListViewItem animeItem = new ListViewItem
                        {
                            ContentTemplate = (DataTemplate)FindResource("EpisodeListTemplate"),
                            Content = episode
                        };
                        episodes.Add(animeItem);
                    }
                    lv_Anime.ItemsSource = episodes;
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
            List<ListViewItem> animesSearch = new List<ListViewItem>();
            foreach(Anime anime in animeList)
			{
                if(anime.OtherName == null)
				{
					anime.OtherName = "";
				}
				if(anime.Name.ToUpper().Contains(search.ToUpper()) || anime.OtherName.ToUpper().Contains(search.ToUpper()))
				{
                    ListViewItem animeItem = new ListViewItem
                    {
                        ContentTemplate = (DataTemplate)FindResource("AnimeListTemplate"),
                        Content = anime
                    };
                    animesSearch.Add(animeItem);
				}
			}
			lv_Anime.ItemsSource = animesSearch;
		}

        private void Tb_Search_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			Bt_Search_Click(sender, e);
		}
		private void Cb_VersAnime_DropDownClosed(object sender, System.EventArgs e)
		{
			episodeList.Clear();
			tb_Search.Clear();
            string lang = cb_VersAnime.Text;
            AnimeShow(lang);
        }

		private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}
        private void Bt_Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

		private void Bt_Close_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
    }
}
