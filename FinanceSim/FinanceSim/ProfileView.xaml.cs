using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace FinanceSim {
	public partial class ProfileView : Page {
		//members
		private MainWindow parent;
		private Profile profile;
		private bool edit;
		//constructors
		internal ProfileView(MainWindow parent) {
			InitializeComponent();
			this.parent = parent;
			this.profile = null;
			edit = false;
		}
		//methods
		private void Clear() {
			viewer.ScrollToTop();
			//personal
			firstNameIn.Text = "";
			firstNameIn.Background = Brushes.White;
			lastNameIn.Text = "";
			lastNameIn.Background = Brushes.White;
			birthdayIn.Text = "";
			birthdayIn.Background = Brushes.White;
			biPayIn.Text = "";
			biPayIn.Background = Brushes.White;
			maleIn.SelectedIndex = 0;
			//utilities
			cableInternetIn.Text = "";
			cableInternetIn.Background = Brushes.White;
			cellPhoneIn.Text = "";
			cellPhoneIn.Background = Brushes.White;
			incWaterIn.IsChecked = incElectricityIn.IsChecked = incHeatIn.IsChecked = false;
			//regular bills
			otherMonthlyIn.Items.Clear();
			//car
			carValueIn.Text = "";
			carValueIn.Background = Brushes.White;
			gasRateIn.Text = "";
			gasRateIn.Background = Brushes.White;
			monthlyCarPaymentIn.Text = "";
			monthlyCarPaymentIn.Background = Brushes.White;
			carMilesIn.Text = "";
			carMilesIn.Background = Brushes.White;
			carYearsIn.Text = "";
			carYearsIn.Background = Brushes.White;
			mpgIn.Text = "";
			mpgIn.Background = Brushes.White;
			//misc
			petsIn.Text = "";
			petsIn.Background = Brushes.White;
			rentIn.Text = "";
			rentIn.Background = Brushes.White;
			collegeLoanIn.Text = "";
			collegeLoanIn.Background = Brushes.White;
			//habits
			snackFreqIn.Text = "";
			snackFreqIn.Background = Brushes.White;
			coffeeFreqIn.Text = "";
			coffeeFreqIn.Background = Brushes.White;
			digitalsIn.Text = "";
			digitalsIn.Background = Brushes.White;
			//other
			desiredDateIn.Text = "";
			cellPhoneIn.Background = Brushes.White;
			desiredDateIn.SelectedDate = null;
			desiredDateIn.Background = Brushes.White;
			challengeLevelIn.Value = 1;
		}
		internal void NewProfile() {
			edit = false;
			Clear();
			this.profile = new Profile();
			otherMonthlyIn.Items.Add(CreateRegularBillUI("Netflix", null, null));
			otherMonthlyIn.Items.Add(CreateRegularBillUI("Gym", null, null));
		}
		internal void LoadProfile(Profile newProfile) {
			edit = true;
			Clear();
			profile = newProfile;
			//personal
			firstNameIn.Text = profile.FirstName;
			lastNameIn.Text = profile.LastName;
			birthdayIn.SelectedDate = profile.Birthday;
			biPayIn.Text = profile.BiPay.ToString("C");
			maleIn.SelectedIndex = profile.Male == null ? 2 : (profile.Male.Value ? 0 : 1);
			//utilities
			cableInternetIn.Text = profile.CableInternet.ToString("C");
			cellPhoneIn.Text = profile.CellPhone.ToString("C");
			incWaterIn.IsChecked = profile.IncWater;
			incElectricityIn.IsChecked = profile.IncElectricity;
			incHeatIn.IsChecked = profile.IncHeat;
			//regular bills
			foreach (CertainFixedPayment cfp in newProfile.OtherMonthly)
				otherMonthlyIn.Items.Add(CreateRegularBillUI(cfp.Name, -1 * cfp.GetPayment(null), cfp.RefTime));
			//car
			carValueIn.Text = profile.CarValue.ToString("C");
			carMilesIn.Text = profile.CarMiles.ToString();
			gasRateIn.Text = profile.GasRate.ToString("C");
			mpgIn.Text = profile.MPG.ToString();
			monthlyCarPaymentIn.Text = profile.MonthlyCarPayment.ToString("C");
			carYearsIn.Text = profile.CarYears.ToString();
			//misc
			petsIn.Text = profile.Pets.ToString();
			rentIn.Text = profile.Rent.ToString("C");
			collegeLoanIn.Text = profile.CollegeLoan.ToString("C");
			//habits
			snackFreqIn.Text = profile.SnackFreq.ToString();
			coffeeFreqIn.Text = profile.CoffeeFreq.ToString();
			digitalsIn.Text = profile.Digitals.ToString();
			//other
			desiredDateIn.SelectedDate = profile.DesiredDate;
			challengeLevelIn.Value = profile.ChallengeLevel;
		}
		private void CompleteProfile() {
			//personal
			profile.FirstName = firstNameIn.Text;
			profile.LastName = lastNameIn.Text;
			profile.Birthday = birthdayIn.SelectedDate.Value;
			profile.BiPay = decimal.Parse(biPayIn.Text, NumberStyles.Currency);
			if(maleIn.SelectedIndex == 2) {
				profile.Male = null;
			}
			else {
				profile.Male = maleIn.SelectedIndex == 0;
            }
			//utilities
			profile.CableInternet = decimal.Parse(cableInternetIn.Text, NumberStyles.Currency);
			profile.CellPhone = decimal.Parse(cellPhoneIn.Text, NumberStyles.Currency);
			profile.IncWater = incWaterIn.IsChecked.Value;
			profile.IncElectricity = incElectricityIn.IsChecked.Value;
			profile.IncHeat = incHeatIn.IsChecked.Value;
			//regular bills
			profile.OtherMonthly = GenerateOtherBills();
			//car
			profile.CarValue = decimal.Parse(carValueIn.Text, NumberStyles.Currency);
			profile.GasRate = decimal.Parse(gasRateIn.Text, NumberStyles.Currency);
			profile.MonthlyCarPayment = decimal.Parse(monthlyCarPaymentIn.Text, NumberStyles.Currency);
			profile.CarMiles = int.Parse(carMilesIn.Text, NumberStyles.AllowThousands);
			profile.MPG = int.Parse(mpgIn.Text, NumberStyles.AllowThousands);
			profile.CarYears = int.Parse(carYearsIn.Text, NumberStyles.AllowThousands);
			//misc
			profile.Pets = int.Parse(petsIn.Text, NumberStyles.AllowThousands);
			profile.Rent = decimal.Parse(rentIn.Text, NumberStyles.Currency);
			profile.CollegeLoan = decimal.Parse(collegeLoanIn.Text, NumberStyles.Currency);
			//habits
			profile.SnackFreq = int.Parse(snackFreqIn.Text, NumberStyles.AllowThousands);
			profile.CoffeeFreq = int.Parse(coffeeFreqIn.Text, NumberStyles.AllowThousands);
			profile.Digitals = int.Parse(digitalsIn.Text, NumberStyles.AllowThousands);
			//other
			profile.DesiredDate = desiredDateIn.SelectedDate.Value;
			profile.ChallengeLevel = (int)challengeLevelIn.Value;

			profile.Balance = 0m;
			profile.StopDate = profile.DesiredDate;
			profile.CreatePayments();
		}
		private List<CertainFixedPayment> GenerateOtherBills() {
			List<CertainFixedPayment> bills = new List<CertainFixedPayment>();
			for (int i = 0; i < otherMonthlyIn.Items.Count; i++) {
				UIElementCollection uiec = (otherMonthlyIn.Items[i] as UniformGrid).Children;
				bills.Add(new CertainFixedPayment((uiec[1] as TextBox).Text, new Description("A monthly expense."), "Monthly Expense",
					decimal.Parse((uiec[3] as TextBox).Text, NumberStyles.Currency), 
					Frequency.MONTHLY_DAY, (uiec[5] as DatePicker).SelectedDate.Value));
			}
			return bills;
		}
		private UniformGrid CreateRegularBillUI(string name, decimal? payment, DateTime? date) {
			UniformGrid ug = new UniformGrid();
			ug.Columns = 6;
			Label l = new Label();
			l.Content = "Name:";
			ug.Children.Add(l);
			TextBox tb = new TextBox();
			tb.Text = name;
			tb.LostFocus += String_LostFocus;
			ug.Children.Add(tb);

			l = new Label();
			l.Content = "Payment:";
			ug.Children.Add(l);
			tb = new TextBox();
			tb.Text = payment != null ? payment.Value.ToString("C") : "";
			tb.LostFocus += Currency_LostFocus;
			ug.Children.Add(tb);

			l = new Label();
			l.Content = "Payment Day:";
			ug.Children.Add(l);
			DatePicker dp = new DatePicker();
			dp.SelectedDate = date;
			dp.LostFocus += DateTime_LostFocus;
			ug.Children.Add(dp);
			return ug;
		}
		private bool ValidateProfile() {
			return ValidateText(firstNameIn) & ValidateText(lastNameIn) & ValidateDateTime(birthdayIn) //personal
				& ValidateCurrency(biPayIn)
				& ValidateCurrency(cableInternetIn) & ValidateCurrency(cellPhoneIn) //utitlties
				& ValidateCustoms() //other bills
				& ValidateCurrency(carValueIn) & ValidateNumber(carMilesIn) & ValidateCurrency(gasRateIn) //car
				& ValidateNumber(mpgIn) & ValidateCurrency(monthlyCarPaymentIn) & ValidateNumber(carYearsIn)
				& ValidateNumber(petsIn) & ValidateCurrency(rentIn) & ValidateCurrency(collegeLoanIn) //misc
				& ValidateNumber(snackFreqIn) & ValidateNumber(coffeeFreqIn) & ValidateNumber(digitalsIn) //habits
				& ValidateDateTime(desiredDateIn); //other
		}
		private bool ValidateCustoms() {
			bool valid = true;
			for(int i = 0; i < otherMonthlyIn.Items.Count; i++) {
				UIElementCollection uiec = (otherMonthlyIn.Items[i] as UniformGrid).Children;
				valid &= ValidateText(uiec[1] as TextBox);
				valid &= ValidateCurrency(uiec[3] as TextBox);
				valid &= ValidateDateTime(uiec[5] as DatePicker);
			}
			return valid;
		}
		private bool ValidateText(TextBox tb) {
			ColorValid(tb, tb.Text.Length > 0);
			return tb.Text.Length > 0;
		}
		private bool ValidateCurrency(TextBox tb) {
			decimal result;
			bool valid = decimal.TryParse(tb.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out result);
			tb.Text = valid ? result.ToString("C") : "";
			ColorValid(tb, valid);
			return valid;
		}
		private bool ValidateNumber(TextBox tb) {
			short result;
			bool valid = short.TryParse(tb.Text, NumberStyles.AllowThousands, CultureInfo.CurrentCulture.NumberFormat, out result);
			if (!valid) {
				tb.Text = "";
			}
			ColorValid(tb, valid);
			return valid;
		}
		private bool ValidateDateTime(DatePicker dp) {
			bool valid = dp.SelectedDate.HasValue;
			dp.Background = valid ? Brushes.White : Brushes.DarkSalmon;
			return dp.SelectedDate.HasValue;
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
		private void DateTime_LostFocus(object sender, RoutedEventArgs e) {
			ValidateDateTime(sender as DatePicker);
		}
		private void addBillButton_Click(object sender, RoutedEventArgs e) {
			otherMonthlyIn.Items.Add(CreateRegularBillUI(null, null, null));
		}
		private void removeBillButton_Click(object sender, RoutedEventArgs e) {
			if(otherMonthlyIn.SelectedIndex != -1) {
				otherMonthlyIn.Items.RemoveAt(otherMonthlyIn.SelectedIndex);
			}
		}
		private void goButton_Click(object sender, RoutedEventArgs e) {
			if (ValidateProfile()) {
				if (edit) {
					MessageBoxResult mbr = MessageBox.Show("Are you sure you want to start over with these new settings?", "Restart?", MessageBoxButton.YesNo);
					if (mbr.Equals(MessageBoxResult.No)) {
						return;
					}
				}
				CompleteProfile();
				if (!edit) {
					parent.AddProfile(profile);
				}
				parent.ReturnAndSave();
			}
		}
		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			parent.Return();
		}
	}
}
