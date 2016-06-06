using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace FinanceSim {
	public partial class DataView : Page {
		//members
		private MainWindow parent;
		private DateTime date;
		private Profile currPro;
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
			currPro = null;
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
				if (i - ((int)date.DayOfWeek + date.Day - 1) == 0) {
					highlightedIndex = 7 + i;
				}
				calendarGrid.Children.Add(i - (int)date.DayOfWeek >= 0 ? CreateCalendarDate(day++) : CreateEmptyDate());
			}
			foreach (Payment p in currPro.Payments) {
				if(p is CertainFixedPayment || p is CertainMonthDepPayment || p is RelativeRandomPayment || p is CertainRandomPayment) {
					for (int j = 0; j < monthDays; j++) {
						DateTime dt = new DateTime(date.Year, date.Month, j + 1);
						if (p.IsDue(dt)) {
							decimal payment = p.GetPayment(dt);
							Label l = (calendarGrid.Children[(int)date.DayOfWeek + 7 + j] as Label);
							Brush newBrush = payment > 0 ? Brushes.LightGreen : Brushes.LightSalmon;
							if (l.Background.Equals(Brushes.White) || l.Background.Equals(newBrush))
								l.Background = newBrush;
							else
								l.Background = Brushes.LightYellow;
							if(l.ToolTip == null) {
								l.ToolTip = CreateCalTool(p.Name);
							}
							else {
								(l.ToolTip as Label).Content += "\n" + p.Name;
                            }
						}
					}
				}
			}
		}
		private Label CreateCalTool(string content) {
			Label l = new Label();
			l.FontStyle = FontStyles.Italic;
			l.FontSize = 16;
			l.Content = content;
			return l;
		}
		internal void OpenProfile(Profile profile) {
			profile.LastOpened = DateTime.Now;
			currPro = profile;
			date = profile.StopDate.AddDays(1);
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
			dateLabel.Content = date.ToString("dddd, MMMM dd, yyyy");
			expensesPanel.Children.Clear();
			List<ViewablePayment> vPays = GetExpenses();
			foreach (ViewablePayment vp in vPays) {
				vp.FontSize = 14;
				DockPanel.SetDock(vp, Dock.Top);
				expensesPanel.Children.Add(vp);
				currPro.Balance += vp.Bill;
			}
			if(vPays.Count == 0) {
				Label ne = new Label();
				ne.Content = "No expenses.";
				ne.FontWeight = FontWeights.SemiBold;
				ne.FontSize = 24;
				expensesPanel.Children.Add(ne);
            }
			if(!first)
				highlightedIndex++;
			if(highlightedIndex == calendarGrid.Children.Count) {
				AdjustCalendar();
				ColorToday(false);
			}
			else
				ColorToday(!first);
			moneyLabel.Content = currPro.Balance.ToString("C", formatInfo);
			moneyLabel.Foreground = currPro.Balance > 0 ? Brushes.Green : Brushes.Red;
		}
		private List<ViewablePayment> GetExpenses() {
			List<ViewablePayment> vPays = new List<ViewablePayment>();
			bool color = false;
			foreach(Payment p in currPro.Payments) {
				if (p.IsDue(date)) {
					vPays.Add(new ViewablePayment(p, date, color, formatInfo));
					color = !color;
				}
			}
			return vPays;
		}
		private void advanceButton_Click(object sender, RoutedEventArgs e) {
			date = date.Add(TimeSpan.FromDays(1));
			DoExpenses(false);
		}
		private void backButton_Click(object sender, RoutedEventArgs e) {
			currPro.StopDate = date;
			parent.ReturnAndSave();
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
		private decimal bill;
		//constructors
		internal ViewablePayment(Payment payment, DateTime date, bool color, NumberFormatInfo formatInfo) {
			bill = payment.GetPayment(date);
			HorizontalAlignment = HorizontalAlignment.Stretch;
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			Background = color ? Brushes.LightGray : Brushes.White;
			Label cost = new Label();
			cost.Foreground = bill > 0 ? Brushes.Green : Brushes.Red;
			cost.FontSize = 28;
			cost.Content = bill.ToString("C", formatInfo);
			DockPanel.SetDock(cost, Dock.Left);
			Label title = new Label();
			title.FontSize = 24;
			title.FontWeight = FontWeights.ExtraBold;
			title.Content = payment.Name;
			title.HorizontalContentAlignment = HorizontalAlignment.Right;
			DockPanel.SetDock(title, Dock.Right);
			DockPanel dp = new DockPanel();
			dp.Children.Add(cost);
			dp.Children.Add(title);
			DockPanel.SetDock(dp, Dock.Top);
			Label cate = new Label();
			cate.Content = payment.Category;
			cate.Foreground = Brushes.Gray;
			cate.HorizontalContentAlignment = HorizontalAlignment.Right;
			DockPanel.SetDock(cate, Dock.Right);
			TextBlock desc = new TextBlock();
			desc.FontStyle = FontStyles.Italic;
			desc.FontSize = 14;
			desc.Text = payment.FindDescription(-1 * bill);
			desc.TextWrapping = TextWrapping.WrapWithOverflow;
			DockPanel.SetDock(desc, Dock.Left);
			DockPanel dp2 = new DockPanel();
			dp2.Children.Add(cate);
			dp2.Children.Add(desc);
			DockPanel.SetDock(dp2, Dock.Bottom);

			UniformGrid ug = new UniformGrid();
			ug.HorizontalAlignment = HorizontalAlignment.Stretch;
			ug.Rows = 2;
			ug.Columns = 1;
			ug.Children.Add(dp);
			ug.Children.Add(dp2);
			Content = ug;
		}
		//properties
		internal decimal Bill { get { return bill; } }
	}
}
