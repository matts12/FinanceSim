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
	public partial class DataView : Page {
		//members
		private MainWindow parent;
		private Profile profile;
		private DateTime date;
		//constructors
		internal DataView(MainWindow parent, Profile profile) {
			InitializeComponent();
			this.parent = parent;
			this.profile = profile;
			date = new DateTime(profile.DesiredYear, profile.DesiredMonth, 1);
			for (int i = 0; i < calendarGrid.Rows * calendarGrid.Columns; i++) {
				Label l = new Label();
				l.Content = i;
				calendarGrid.Children.Add(l);
			}
		}
		//methods
	}
}
