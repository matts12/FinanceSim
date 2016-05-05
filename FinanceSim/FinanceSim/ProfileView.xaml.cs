using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FinanceSim {
	public partial class ProfileView : Page {
		//members
		private MainWindow parent;
		private Profile profile;
		//constructors
		internal ProfileView(MainWindow parent, Profile profile) {
			InitializeComponent();
			this.parent = parent;
			this.profile = profile;
			firstNameIn.LostFocus += String_LostFocus;
			lastNameIn.LostFocus += String_LostFocus;
		}
		//methods
		private void p_goButton_Click(object sender, RoutedEventArgs e) {
			if (ValidateProfile()) {
				CompleteProfile();
				parent.ChangeContent(1);
			}
		}
		private void CompleteProfile() {
			profile.FirstName = firstNameIn.Text;
			profile.LastName = lastNameIn.Text;
		}
		private bool ValidateProfile() {
			return ValidateText(firstNameIn.Text) && ValidateText(lastNameIn.Text);
		}
		private bool ValidateText(string s) { return s.Length > 0; }
		//wpf
		private void String_LostFocus(object sender, RoutedEventArgs e) {
			TextBox tb = sender as TextBox;
			tb.Background = ValidateText(tb.Text) ? Brushes.White : Brushes.DarkSalmon;
		}
	}
}
