using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceSim {
	public partial class TitleView : Page {
		//members
		private MainWindow parent;
		private NumberFormatInfo formatInfo;
		//constructors
		public TitleView(MainWindow parent) {
            InitializeComponent();
			this.parent = parent;
			titleImg.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
			   Properties.Resources.img.GetHbitmap(),
			   IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(200, 200));
			formatInfo = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
			formatInfo.CurrencyNegativePattern = 1;
		}
		//methods
		internal void OpenProfiles(List<Profile> profiles) {
			profilesPanel.Children.Clear();
			for(int i = 0; i < profiles.Count; i++) {
				profilesPanel.Children.Add(CreateProfilePanel(profiles[i], i));
			}
		}
		private StackPanel CreateProfilePanel(Profile p, int i) {
			StackPanel sp = new StackPanel();
			sp.Background = i % 2 == 0 ? Brushes.White : Brushes.LightGray;
			DockPanel dp = new DockPanel();

			Label l = new Label();
			l.Content = p.LastName + ", " + p.FirstName;
			l.FontSize = 32;
			l.FontWeight = FontWeights.Heavy;
			DockPanel.SetDock(l, Dock.Left);
			dp.Children.Add(l);
			l = new Label();
			l.Content = "Last Opened: " + p.LastOpened.ToString("dddd, MMMM dd") + " at " + p.LastOpened.ToString("h:mm");
			l.Foreground = Brushes.Gray;
			l.FontSize = 20;
			l.HorizontalContentAlignment = HorizontalAlignment.Right;
			l.VerticalContentAlignment = VerticalAlignment.Center;
			l.FontStyle = FontStyles.Italic;
			DockPanel.SetDock(l, Dock.Right);
			dp.Children.Add(l);

			UniformGrid ug = new UniformGrid();
			ug.Columns = 5;
			ug.Rows = 1;

			l = new Label();
			l.FontSize = 14;
			l.VerticalContentAlignment = VerticalAlignment.Center;
			l.HorizontalContentAlignment = HorizontalAlignment.Center;
			l.Content = "Start Date: " + p.DesiredDate.ToString("M/dd/yy") + "\nCurrent Date: " + p.StopDate.ToString("M/dd/yy");
			ug.Children.Add(l);

			l = new Label();
			l.Foreground = p.Balance > 0 ? Brushes.Green : Brushes.Red;
			l.FontSize = 14;
			l.VerticalContentAlignment = VerticalAlignment.Center;
			l.HorizontalContentAlignment = HorizontalAlignment.Center;
			l.Content = "Balance: " + p.Balance.ToString("C", formatInfo);
			ug.Children.Add(l);

			Button b = new Button();
			b.Margin = new Thickness(15);
			b.Content = "Open";
			b.Name = "openButton_" + i;
			b.Click += OpenButton_Click;
			ug.Children.Add(b);

			b = new Button();
			b.Margin = new Thickness(15);
			b.Content = "Edit & Restart";
			b.Name = "editButton_" + i;
			b.Click += EditButton_Click;
			ug.Children.Add(b);

			b = new Button();
			b.Margin = new Thickness(15);
			b.Content = "Delete";
			b.Name = "deleteButton_" + i;
			b.Click += DeleteButton_Click;
			ug.Children.Add(b);

			sp.Children.Add(dp);
			sp.Children.Add(ug);

			return sp;
		}
		//wpf
		private void OpenButton_Click(object sender, RoutedEventArgs e) {
			int i = int.Parse((sender as Button).Name.Last().ToString());
			parent.OpenProfile(i);
		}
		private void EditButton_Click(object sender, RoutedEventArgs e) {
			int i = int.Parse((sender as Button).Name.Last().ToString());
			parent.EditProfile(i);
		}
		private void NewButton_Click(object sender, RoutedEventArgs e) {
			parent.NewProfile();
		}
		private void DeleteButton_Click(object sender, RoutedEventArgs e) {
			MessageBoxResult r = MessageBox.Show(parent, "Are you sure you want to delete this profile?", "Delete?", MessageBoxButton.YesNo);
			if (r.Equals(MessageBoxResult.Yes)) {
				int i = int.Parse((sender as Button).Name.Last().ToString());
				parent.DeleteProfile(i);
			}
		}
	}
}
