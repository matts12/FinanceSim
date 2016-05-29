using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FinanceSim {
	public partial class DataView : Page {
		//members
		private MainWindow parent;
		private DateTime date;
		private decimal money;
		private List<Payment> payments;
		private NumberFormatInfo formatInfo;
		private int highlightedIndex;
		private Brush prevColor;
		private bool doneLoading;
		//constructors
		internal DataView(MainWindow parent) {
			InitializeComponent();
			Loaded += DataView_Loaded;
			this.parent = parent;
			formatInfo = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
			formatInfo.CurrencyNegativePattern = 1;
			highlightedIndex = -1;
			prevColor = null;
			doneLoading = false;
		}
		//methods
		private Label CreateCalendarDate(int day) {
			Label l = new Label();
			l.Content = day;
			l.Background = Brushes.White;
			l.BorderBrush = Brushes.Black;
			l.BorderThickness = new Thickness(0.5);
			return l;
		}
		private Label CreateEmptyDate() {
			return new Label();
		}
		private Label CreateTitleLabel(string name) {
			Label l = new Label();
			l.Content = name;
			l.FontSize = 15;
			l.Background = Brushes.LightGray;
			l.BorderBrush = Brushes.Black;
			l.BorderThickness = new Thickness(1.5);
			l.HorizontalContentAlignment = HorizontalAlignment.Center;
			l.VerticalContentAlignment = VerticalAlignment.Center;
			return l;
		}
		private void AdjustCalendar() {
			calendarGrid.Children.Clear();
			int day = 1;
			int monthDays = DateTime.DaysInMonth(date.Year, date.Month);
			string[] dowNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
			foreach(string s in dowNames)
				calendarGrid.Children.Add(CreateTitleLabel(s));
            for (int i = 0; day <= monthDays; i++) {
				if (i - (int)date.DayOfWeek == 0) {
					highlightedIndex = 7 + i;
				}
				calendarGrid.Children.Add(i - (int)date.DayOfWeek >= 0 ? CreateCalendarDate(day++) : CreateEmptyDate());
			}
			foreach (Payment p in payments) {
				if(p is CertainFixedPayment) {
					for (int j = 0; j < monthDays; j++) {
						if(p.IsDue(new DateTime(date.Year, date.Month, j + 1))) {
							decimal payment = p.GetPayment();
							Label l = (calendarGrid.Children[(int)date.DayOfWeek + 7 + j] as Label);
							Brush newBrush = payment > 0 ? Brushes.LightGreen : Brushes.LightSalmon;
							if (l.Background.Equals(Brushes.White))
								l.Background = newBrush;
							else
								l.Background = Brushes.LightYellow;
							l.ToolTip = l.ToolTip == null ? p.Name + "\n" : l.ToolTip + p.Name + "\n";
						}
					}
				}
			}
		}
		internal void OpenProfile(Profile profile) {
			profile.LastOpened = DateTime.Now;
			payments = Payment.GeneratePayments(profile);
			date = new DateTime(profile.DesiredDate.Year, profile.DesiredDate.Month, 1);
			money = 0m;
			AdjustCalendar();
			DoExpenses(true);
		}
		private void ColorToday(bool colorPrev) {
			if (colorPrev) {
				(calendarGrid.Children[highlightedIndex - 1] as Label).Background = prevColor;
			}
			Label curr = (calendarGrid.Children[highlightedIndex] as Label);
			prevColor = curr.Background;
			curr.Background = Brushes.AliceBlue;
        }
		private void DoExpenses(bool first) {
			dateLabel.Content = date.ToString("MM/dd/yyyy");
			expensesPanel.Items.Clear();
			List<ViewablePayment> vPays = GetExpenses();
			foreach (ViewablePayment vp in vPays) {
				vp.FontSize = 14;
				expensesPanel.Items.Add(vp);
				money += vp.Bill;
			}
			if(vPays.Count == 0) {
				Label ne = new Label();
				ne.Content = "No expenses.";
				ne.FontSize = 14;
				expensesPanel.Items.Add(ne);
            }
			moneyLabel.Content = "Balance: " + money.ToString("C", formatInfo);
			if(!first)
				highlightedIndex++;
			if(highlightedIndex == calendarGrid.Children.Count) {
				AdjustCalendar();
				ColorToday(false);
			}
			else
				ColorToday(!first);
		}
		private List<ViewablePayment> GetExpenses() {
			List<ViewablePayment> vPays = new List<ViewablePayment>();
			foreach(Payment p in payments) {
				if (p.IsDue(date)) {
					vPays.Add(new ViewablePayment(p, formatInfo));
				}
			}
			return vPays;
		}
		private void advanceButton_Click(object sender, RoutedEventArgs e) {
			date = date.Add(TimeSpan.FromDays(1));
			DoExpenses(false);
		}
		private void backButton_Click(object sender, RoutedEventArgs e) {
			parent.Return();
		}
		private void calendarGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
			double width = calendarGrid.ActualWidth;
			calendarGrid.Height = width;
		}
		//wpf
		private void DataView_PreviewKeyDown(object sender, KeyEventArgs e) {
			if (Keyboard.IsKeyDown(Key.A) && Keyboard.IsKeyDown(Key.LeftCtrl)) {
				advanceButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
			}
		}
		private void DataView_Loaded(object sender, RoutedEventArgs e) {
			if (!doneLoading) {
				PreviewKeyDown += DataView_PreviewKeyDown;
				doneLoading = true;
			}
			Focusable = true;
			Focus();
		}
	}
	class ViewablePayment : Label {
		//members
		private Payment payment;
		private decimal bill;
		//constructors
		internal ViewablePayment(Payment payment, NumberFormatInfo formatInfo) {
			this.payment = payment;
			bill = payment.GetPayment();
			Content = payment.Name + " " + bill.ToString("C", formatInfo);
		}
		//properties
		internal decimal Bill { get { return bill; } }
	}
}
