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

namespace FinanceSim {
	public partial class TitleView : Page {
		//members
		private MainWindow parent;
		//constructors
		public TitleView(MainWindow parent) {
            InitializeComponent();
			this.parent = parent;
		}
		//methods
		internal void OpenProfiles(List<Profile> profiles) {
			profilesPanel.Items.Clear();
			for(int i = 0; i < profiles.Count; i++)
				profilesPanel.Items.Add(CreateProfilePanel(profiles[i], i));
		}
		private Grid CreateProfilePanel(Profile p, int i) {
			Grid g = new Grid();
			g.ColumnDefinitions.Add(new ColumnDefinition());
			g.ColumnDefinitions.Add(new ColumnDefinition());
			g.ColumnDefinitions.Add(new ColumnDefinition());
			g.RowDefinitions.Add(new RowDefinition());
			g.RowDefinitions.Add(new RowDefinition());

			Label l = new Label();
			l.Content = p.LastName + ", " + p.FirstName;
			Grid.SetRow(l, 0);
			Grid.SetColumn(l, 0);
			Grid.SetColumnSpan(l, 2);
			g.Children.Add(l);

			Button b = new Button();
			b.Content = "Open";
			b.Name = "openButton_" + i;
			b.Click += OpenButton_Click;
			Grid.SetRow(b, 1);
			Grid.SetColumn(b, 0);
			g.Children.Add(b);

			b = new Button();
			b.Content = "Edit";
			b.Name = "editButton_" + i;
			b.Click += EditButton_Click;
			Grid.SetRow(b, 1);
			Grid.SetColumn(b, 1);
			g.Children.Add(b);
			b = new Button();

			b = new Button();
			b.Content = "Delete";
			b.Name = "deleteButton_" + i;
			b.Click += DeleteButton_Click;
			Grid.SetRow(b, 1);
			Grid.SetColumn(b, 2);
			g.Children.Add(b);
			return g;
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
