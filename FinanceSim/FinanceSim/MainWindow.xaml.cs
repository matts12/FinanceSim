using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FinanceSim {
	public partial class MainWindow : Window {
		//members
		private Page[] contents;
		private Profile profile;
		//constructors
		public MainWindow() {
			InitializeComponent();
			profile = new Profile();
			contents = new Page[] { new ProfileView(this, profile), new DataView(this, profile) };
			ChangeContent(0);
		}
		//methods
		internal void ChangeContent(int i) {
			Content = contents[i];
		}
	}
}
