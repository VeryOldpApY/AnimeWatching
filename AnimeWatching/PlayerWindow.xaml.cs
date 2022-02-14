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
	}
}
