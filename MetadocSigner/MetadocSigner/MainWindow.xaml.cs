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

namespace MetadocSigner {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private MainViewModel viewModel;

		public MainWindow() {
			InitializeComponent();
			this.viewModel = (MainViewModel)this.DataContext;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			viewModel.Initialize(this);
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e) {
			viewModel.BrowseForFile();
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e) {
			viewModel.RefreshCertificates();
		}

		private void SignButton_Click(object sender, RoutedEventArgs e) {
			viewModel.Sign();
		}

		private void ApplyPreset1Button_Click(object sender, RoutedEventArgs e) {
			viewModel.ApplyPreset(1);
		}

		private void ApplyPreset2Button_Click(object sender, RoutedEventArgs e) {
			viewModel.ApplyPreset(2);
		}
	}
}
