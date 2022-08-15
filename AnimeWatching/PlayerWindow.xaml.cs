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
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Wpf;

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
			this.DragMove();
		}

		private void Bt_Close_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
        private void Bt_Max_Click(object sender, RoutedEventArgs e)
        {
			Label_PreviewMouseDoubleClick(sender, null);
        }

        private void Bt_Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void Label_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (this.WindowState == WindowState.Maximized)
			{
				this.WindowState = WindowState.Normal;
			}
			else if (this.WindowState == WindowState.Normal)
			{
				this.WindowState = WindowState.Maximized;
			}
		}
	}
}
