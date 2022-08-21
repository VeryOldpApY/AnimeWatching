using System;
using System.Windows;
using System.Windows.Input;

namespace AnimeWatching
{
	/// <summary>
	/// Logique d'interaction pour PlayerWindow.xaml
	/// </summary>
	public partial class PlayerWindow : Window
	{
		public PlayerWindow(string Link)
		{
			InitializeComponent();
			PlayerAnime.Source = new Uri(Link);
		}

		private void PlayerAnime_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
		{
			PlayerAnime.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
		}

		private void CoreWebView2_NewWindowRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
		{
			e.Handled = true;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			PlayerAnime.Dispose();
		}

		private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void Bt_Close_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
        private void Bt_Max_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else if(WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Bt_Min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
	}
}
