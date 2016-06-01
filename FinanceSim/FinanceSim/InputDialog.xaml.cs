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

namespace FinanceSim {
	public partial class InputDialog : Window {
		//members
		//constructors
		public InputDialog() {
			InitializeComponent();
		}
		//methods
		internal decimal GetValue() {
			ShowDialog();
			return 0m;
		}
		//wpf
		private void okButton_Click(object sender, RoutedEventArgs e) {
			DialogResult = true;
			Close();
		}
		
	}
}
