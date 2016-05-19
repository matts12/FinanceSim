using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FinanceSim {
	public partial class DataView : Page {
		//members
		private MainWindow parent;
		private DateTime date;
		private decimal money;
		private List<Payment> payments;
		//constructors
		internal DataView(MainWindow parent) {
			InitializeComponent();
			this.parent = parent;
			for (int i = 0; i < calendarGrid.Rows * calendarGrid.Columns; i++) {
				Label l = new Label();
				l.Content = i;
				calendarGrid.Children.Add(l);
			}
			calendar.IsTodayHighlighted = false;
			calendar.SelectionMode = CalendarSelectionMode.SingleDate;

			//calendar.SelectedDates.Add(new DateTime(2016, 5, 20));
		}
		//methods
		internal void OpenProfile(Profile profile) {
			profile.LastOpened = DateTime.Now;
			payments = Payment.GeneratePayments(profile);
			date = new DateTime(profile.DesiredDate.Year, profile.DesiredDate.Month, 1);
			money = profile.Savings;
			DoExpenses();
		}
		private void DoExpenses() {
			dateLabel.Content = date.ToString("MM/dd/yyyy");
			expensesPanel.Children.Clear();
			List<ViewablePayment> vPays = GetExpenses();
			foreach (ViewablePayment vp in vPays) {
				vp.FontSize = 14;
				expensesPanel.Children.Add(vp);
				money += vp.Bill;
			}
			if(vPays.Count == 0) {
				Label ne = new Label();
				ne.Content = "No expenses.";
				ne.FontSize = 14;
				expensesPanel.Children.Add(ne);
            }
			moneyLabel.Content = "Balance: " + money.ToString("C");
		}
		private List<ViewablePayment> GetExpenses() {
			List<ViewablePayment> vPays = new List<ViewablePayment>();
			foreach(Payment p in payments) {
				if (p.IsDue(date)) {
					vPays.Add(new ViewablePayment(p));
				}
			}
			return vPays;
		}
		private void advanceButton_Click(object sender, RoutedEventArgs e) {
			date = date.Add(TimeSpan.FromDays(1));
			DoExpenses();
		}
	}
	class ViewablePayment : Label {
		//members
		private Payment payment;
		private decimal bill;
		//constructors
		internal ViewablePayment(Payment payment) {
			this.payment = payment;
			bill = payment.GetPayment();
			Content = payment.Name + " " + bill.ToString("C");
		}
		//properties
		internal decimal Bill { get { return bill; } }
	}
}
