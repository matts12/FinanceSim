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
			contents = new Page[] { new ProfileView(this, profile), new DataView(this) };
			ChangeContent(0);
		}
		//methods
		internal void ChangeContent(int i) {
			if (i == 1)
				(contents[1] as DataView).OpenProfile(profile);
			Content = contents[i];
		}
	}
}
