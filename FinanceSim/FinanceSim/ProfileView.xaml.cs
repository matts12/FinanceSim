using System;
using System.Collections.Generic;
using System.Globalization;
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
			//TODO
		}
		private bool ValidateProfile() {
			//return ValidateText(firstNameIn) & ValidateText(lastNameIn) & ValidateText(streetAddressIn)
			//	& ValidateCurrency(incomeIn);
			return true; //TODO
		}
		private bool ValidateText(TextBox tb) {
			ColorValid(tb, tb.Text.Length > 0);
			return tb.Text.Length > 0;
		}
		private bool ValidateCurrency(TextBox tb) {
			decimal result;
			bool valid = decimal.TryParse(tb.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out result);
			if (valid) {
				tb.Text = result.ToString("C");
			}
			else {
				tb.Text = "";
			}
			ColorValid(tb, valid);
			return valid;
		}
		private bool ValidateNumber(TextBox tb) {
			short result;
			bool valid = short.TryParse(tb.Text, out result);
			if (!valid) {
				tb.Text = "";
			}
			ColorValid(tb, valid);
			return valid;
		}
		private void ColorValid(TextBox tb, bool valid) {
			tb.Background = valid ? Brushes.White : Brushes.DarkSalmon;
		}
		//wpf
		private void String_LostFocus(object sender, RoutedEventArgs e) {
			ValidateText(sender as TextBox);
		}
		private void Number_LostFocus(object sender, RoutedEventArgs e) {
			ValidateNumber(sender as TextBox);
		}
		private void Currency_LostFocus(object sender, RoutedEventArgs e) {
			ValidateCurrency(sender as TextBox);
		}
	}
}
