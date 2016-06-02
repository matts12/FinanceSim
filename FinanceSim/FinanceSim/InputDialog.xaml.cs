using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinanceSim {
	public partial class InputDialog : Window {
		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		//members
		private decimal min, max;
		private decimal input;
		//constructors
		public InputDialog() {
			InitializeComponent();
			min = max = -1;
			input = -1m;
			inputBox.Focus();
		}
		//methods
		internal decimal GetInput(Payment p, bool spending, decimal min, decimal max) {
			descBlock.Text = p.FindDescription(0m);
			this.min = min;
			this.max = max;
			spendingLabel.Content = spending ? "This expense is taken out of your monthly spending money." : "";
			ShowDialog();
			return -1 * input;
		}
		private bool Validate() {
			decimal result;
			bool valid = decimal.TryParse(inputBox.Text, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out result);
			if (valid) {
				if(min < 0 && max < 0) {
					valid = result > 0;
				}
				else {
					valid = result <= max && result >= min;
                }
			}
			inputBox.Text = valid ? result.ToString("C") : "";
			inputBox.Background = valid ? Brushes.White : Brushes.DarkSalmon;
			if (valid) {
				input = result;
			}
			return valid;
		}
		//wpf
		private void okButton_Click(object sender, RoutedEventArgs e) {
			if (Validate()) {
				DialogResult = true;
				Close();
			}
		}
		private void Window_Loaded(object sender, RoutedEventArgs e) {
			var hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}
		private void inputBox_KeyDown(object sender, KeyEventArgs e) {
			if (Keyboard.IsKeyDown(Key.Enter)) {
				okButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
			}
		}
		private void inputBox_LostFocus(object sender, RoutedEventArgs e) {
			Validate();
		}
	}
}
