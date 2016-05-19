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
			//personal
			firstNameIn.Text = "";
			firstNameIn.Background = Brushes.White;
            lastNameIn.Text = "";
			lastNameIn.Background = Brushes.White;
			streetAddressIn.Text = "";
			streetAddressIn.Background = Brushes.White;
			apartmentNumberIn.Text = "";
			apartmentNumberIn.Background = Brushes.White;
			zipCodeIn.Text = "";
			zipCodeIn.Background = Brushes.White;
			birthdayIn.SelectedDate = null;
			birthdayIn.Background = Brushes.White;
			//income
			incomeIn.Text = "";
			incomeIn.Background = Brushes.White;
			savingsIn.Text = "";
			savingsIn.Background = Brushes.White;
			//apartment
			rentersInsuranceIn.Text = "";
			rentersInsuranceIn.Background = Brushes.White;
			monthlyRentIn.Text = "";
			monthlyRentIn.Background = Brushes.White;
			//utilities
			internetIn.Text = "";
			internetIn.Background = Brushes.White;
			heatIn.Text = "";
			heatIn.Background = Brushes.White;
			electricityIn.Text = "";
			electricityIn.Background = Brushes.White;
			waterIn.Text = "";
			waterIn.Background = Brushes.White;
			countInternet.IsChecked = countHeat.IsChecked = 
				countElectricity.IsChecked = countWater.IsChecked = false;
			//regular bills
			regularBills.Items.Clear();
			//car
			carValueIn.Text = "";
			carValueIn.Background = Brushes.White;
			mpgIn.Text = "";
			mpgIn.Background = Brushes.White;
			monthlyCarPaymentIn.Text = "";
			monthlyCarPaymentIn.Background = Brushes.White;
			isCarSavingsIn.IsChecked = false;
			//pets
			dogsIn.Text = "";
			dogsIn.Background = Brushes.White;
			catsIn.Text = "";
			catsIn.Background = Brushes.White;

			desiredDateIn.SelectedDate = null;
			desiredDateIn.Background = Brushes.White;
		}
		internal void NewProfile() {
			edit = false;
			Clear();
			this.profile = new Profile();
		}
		internal void LoadProfile(Profile newProfile) {
			edit = true;
			Clear();
			//personal
			this.profile = newProfile;
			firstNameIn.Text = newProfile.FirstName;
			lastNameIn.Text = newProfile.LastName;
			streetAddressIn.Text = newProfile.StreetAddress;
			apartmentNumberIn.Text = newProfile.ApartmentNumber.ToString();
			zipCodeIn.Text = newProfile.ZipCode.ToString();
			birthdayIn.SelectedDate = newProfile.Birthday;
			//income
			incomeIn.Text = newProfile.Income.ToString("C");
			savingsIn.Text = newProfile.Savings.ToString("C");
			//apartment
			rentersInsuranceIn.Text = newProfile.RentersInsurance.ToString("C");
			monthlyRentIn.Text = newProfile.MonthlyRent.ToString("C");
			//utilities
			internetIn.Text = newProfile.Internet.ToString("C");
			heatIn.Text = newProfile.Heat.ToString("C");
			electricityIn.Text = newProfile.Electricity.ToString("C");
			waterIn.Text = newProfile.Water.ToString("C");
			countInternet.IsChecked = newProfile.UtilIncluded[0];
			countHeat.IsChecked = newProfile.UtilIncluded[1];
			countElectricity.IsChecked = newProfile.UtilIncluded[2];
			countWater.IsChecked = newProfile.UtilIncluded[3];
			//regular bills
			foreach(CertainFixedPayment cfp in newProfile.RegularBills)
				regularBills.Items.Add(CreateRegularBillUI(cfp.Name, cfp.GetPayment(), cfp.RefTime));
			//car
			carValueIn.Text = newProfile.CarValue.ToString("C");
			mpgIn.Text = newProfile.MPG.ToString();
			monthlyCarPaymentIn.Text = newProfile.MonthlyCarPayment.ToString("C");
			isCarSavingsIn.IsChecked = newProfile.IsCarSavings;
			//pets
			dogsIn.Text = newProfile.Dogs.ToString();
			catsIn.Text = newProfile.Cats.ToString();

			desiredDateIn.SelectedDate = newProfile.DesiredDate;
		}
		private void CompleteProfile() {
			//personal
			profile.FirstName = firstNameIn.Text;
			profile.LastName = lastNameIn.Text;
			profile.StreetAddress = streetAddressIn.Text;
			profile.ApartmentNumber = int.Parse(apartmentNumberIn.Text);
			profile.ZipCode = int.Parse(zipCodeIn.Text);
			profile.Birthday = birthdayIn.SelectedDate.Value;
			//income
			profile.Income = decimal.Parse(incomeIn.Text, NumberStyles.Currency);
			profile.Savings = decimal.Parse(savingsIn.Text, NumberStyles.Currency);
			//apartment
			profile.RentersInsurance = decimal.Parse(rentersInsuranceIn.Text, NumberStyles.Currency);
			profile.MonthlyRent = decimal.Parse(monthlyRentIn.Text, NumberStyles.Currency);
			//utilities
			profile.Internet = decimal.Parse(internetIn.Text, NumberStyles.Currency);
			profile.Heat = decimal.Parse(heatIn.Text, NumberStyles.Currency);
			profile.Electricity = decimal.Parse(electricityIn.Text, NumberStyles.Currency);
			profile.Water = decimal.Parse(waterIn.Text, NumberStyles.Currency);
			profile.UtilIncluded = new bool[] { countInternet.IsChecked.Value, countHeat.IsChecked.Value,
				countElectricity.IsChecked.Value, countWater.IsChecked.Value };
			//regular bills
			profile.RegularBills = GenerateRegularBills();
			//car
			profile.CarValue = decimal.Parse(carValueIn.Text, NumberStyles.Currency);
			profile.MPG = int.Parse(mpgIn.Text);
			profile.MonthlyCarPayment = decimal.Parse(monthlyCarPaymentIn.Text, NumberStyles.Currency);
			profile.IsCarSavings = isCarSavingsIn.IsChecked.Value;
			//pets
			profile.Dogs = int.Parse(dogsIn.Text);
			profile.Cats = int.Parse(catsIn.Text);

			profile.DesiredDate = desiredDateIn.SelectedDate.Value;
		}
		private List<CertainFixedPayment> GenerateRegularBills() {
			List<CertainFixedPayment> bills = new List<CertainFixedPayment>();
			for (int i = 0; i < regularBills.Items.Count; i++) {
				UIElementCollection uiec = (regularBills.Items[i] as UniformGrid).Children;
				bills.Add(new CertainFixedPayment((uiec[1] as TextBox).Text, 
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
			return ValidateText(firstNameIn) & ValidateText(lastNameIn) & ValidateText(streetAddressIn) //personal
				& ValidateNumber(apartmentNumberIn) & ValidateNumber(zipCodeIn) & ValidateDateTime(birthdayIn)
				& ValidateCurrency(incomeIn) & ValidateCurrency(savingsIn) //income
				& ValidateCurrency(rentersInsuranceIn) & ValidateCurrency(monthlyRentIn) //apartment
				& ValidateCurrency(internetIn) & ValidateCurrency(heatIn) & ValidateCurrency(electricityIn) //utilities
				& ValidateCurrency(waterIn)
				& ValidateCustoms() //regular bills
				& ValidateCurrency(carValueIn) & ValidateNumber(mpgIn) & ValidateCurrency(monthlyCarPaymentIn)
				& ValidateNumber(dogsIn) & ValidateNumber(catsIn)
				& ValidateDateTime(desiredDateIn); //pets
		}
		private bool ValidateCustoms() {
			bool valid = true;
			for(int i = 0; i < regularBills.Items.Count; i++) {
				UIElementCollection uiec = (regularBills.Items[i] as UniformGrid).Children;
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
			regularBills.Items.Add(CreateRegularBillUI(null, null, null));
		}
		private void removeBillButton_Click(object sender, RoutedEventArgs e) {
			if(regularBills.SelectedIndex != -1) {
				regularBills.Items.RemoveAt(regularBills.SelectedIndex);
			}
		}
		private void goButton_Click(object sender, RoutedEventArgs e) {
			if (ValidateProfile()) {
				CompleteProfile();
				if (edit) {
					parent.ReturnAndSave();
				}
				else {
					parent.AddProfile(profile);
					parent.OpenProfile(profile);
				}
			}
		}
		private void cancelButton_Click(object sender, RoutedEventArgs e) {
			parent.Return();
		}
	}
}
