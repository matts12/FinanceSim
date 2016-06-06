using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSim {
	[Serializable]
	class Profile : ISerializable, IComparable<Profile> {
		//members
		//personal
		private string firstName;
		private string lastName;
		private DateTime birthday;
		private decimal biPay;
		private bool? male;
		//utilities
		private decimal cableInternet;
		private decimal cellPhone;
		private bool incWater;
		private bool incElectricity;
		private bool incHeat;
		//regular bills
		private List<CertainFixedPayment> otherMonthly;
		//car
		private decimal carValue;
		private int carMiles;
		private decimal gasRate;
		private int mpg;
		private decimal monthlyCarPayment;
		private int carYears;
		//misc
		private int pets;
		private decimal rent;
		private decimal collegeLoan;
		//habits
		private int snackFreq;
		private int coffeeFreq;
		private int digitals;
		//other
		private DateTime desiredDate;
		private DateTime lastOpened;
		private int challengeLevel;

		private decimal balance;
		private List<Payment> payments;
		private DateTime stopDate;
		//constructors
		internal Profile() {
			//personal
			firstName = "";
			lastName = "";
			birthday = DateTime.Today;
			biPay = 0m;
			male = true;
			//utilities
			cableInternet = cellPhone = 0m;
			incWater = incElectricity = incHeat = false;
			//regular bills
			otherMonthly = new List<CertainFixedPayment>();
			//car
			carValue = gasRate = monthlyCarPayment = 0m;
			carMiles = carYears = mpg = 0;
			//misc
			pets = 0;
			rent = collegeLoan = 0m;
			//habits
			snackFreq = coffeeFreq = digitals = 0;
			//other
			desiredDate = DateTime.Today;
			lastOpened = DateTime.Now;
			challengeLevel = 1;
			balance = 0m;
			payments = null;
			stopDate = DateTime.Today;
		}
		internal Profile(SerializationInfo info, StreamingContext context) {
			//personal
			firstName = info.GetString("firstName");
			lastName = info.GetString("lastName");
			birthday = info.GetDateTime("birthday");
			biPay = info.GetDecimal("biPay");
			male = info.GetValue("male", typeof(bool?)) as bool?;
			//utilities
			cableInternet = info.GetDecimal("cableInternet");
			cellPhone = info.GetDecimal("cellPhone");
			incWater = info.GetBoolean("incWater");
			incElectricity = info.GetBoolean("incElectricity");
			incHeat = info.GetBoolean("incHeat");
			//regular bills
			otherMonthly = info.GetValue("otherMonthly", typeof(List<CertainFixedPayment>)) as List<CertainFixedPayment>;
			//car
			carValue = info.GetDecimal("carValue");
			gasRate = info.GetDecimal("gasRate");
			monthlyCarPayment = info.GetDecimal("monthlyCarPayment");
			carMiles = info.GetInt32("carMiles");
			carYears = info.GetInt32("carYears");
			mpg = info.GetInt32("mpg");
			//misc
			pets = info.GetInt32("pets");
			rent = info.GetDecimal("rent");
			collegeLoan = info.GetDecimal("collegeLoan");
			//habits
			snackFreq = info.GetInt32("snackFreq");
			coffeeFreq = info.GetInt32("coffeeFreq");
			digitals = info.GetInt32("digitals");
			//other
			desiredDate = info.GetDateTime("desiredDate");
			lastOpened = info.GetDateTime("lastOpened");
			challengeLevel = info.GetInt32("challengeLevel");
			balance = info.GetDecimal("balance");
			payments = info.GetValue("payments", typeof(List<Payment>)) as List<Payment>;
			stopDate = info.GetDateTime("stopDate");
		}
		//properties
		//personal
		internal string FirstName { get { return firstName; } set { firstName = value; } }
		internal string LastName { get { return lastName; } set { lastName = value; } }
		internal DateTime Birthday { get { return birthday; } set { birthday = value; } }
		internal decimal BiPay { get { return biPay; } set { biPay = value; } }
		internal bool? Male { get { return male; } set { male = value; } }
		//utilities
		internal decimal CableInternet { get { return cableInternet; } set { cableInternet = value; } }
		internal decimal CellPhone { get { return cellPhone; } set { cellPhone = value; } }
		internal bool IncWater { get { return incWater; } set { incWater = value; } }
		internal bool IncElectricity { get { return incElectricity; } set { incElectricity = value; } }
		internal bool IncHeat { get { return incHeat; } set { incHeat = value; } }
		//regular bills
		internal List<CertainFixedPayment> OtherMonthly { get { return otherMonthly; } set { otherMonthly = value; } }
		//car
		internal decimal CarValue { get { return carValue; } set { carValue = value; } }
		internal decimal GasRate { get { return gasRate; } set { gasRate = value; } }
		internal decimal MonthlyCarPayment { get { return monthlyCarPayment; } set { monthlyCarPayment = value; } }
		internal int CarMiles { get { return carMiles; } set { carMiles = value; } }
		internal int CarYears { get { return carYears; } set { carYears = value; } }
		internal int MPG { get { return mpg; } set { mpg = value; } }
		//misc
		internal int Pets { get { return pets; } set { pets = value; } }
		internal decimal Rent { get { return rent; } set { rent = value; } }
		internal decimal CollegeLoan { get { return collegeLoan; } set { collegeLoan = value; } }
		//habits
		internal int SnackFreq { get { return snackFreq; } set { snackFreq = value; } }
		internal int CoffeeFreq { get { return coffeeFreq; } set { coffeeFreq = value; } }
		internal int Digitals { get { return digitals; } set { digitals = value; } }
		//other
		internal DateTime DesiredDate { get { return desiredDate; } set {
			desiredDate = new DateTime(value.Year, value.Month, 1);
		} }
		internal DateTime LastOpened { get { return lastOpened; } set { lastOpened = value; } }
		internal int ChallengeLevel { get { return challengeLevel; } set { challengeLevel = value; } }
		internal decimal Balance { get { return balance; } set { balance = value; } }
		internal List<Payment> Payments { get { return payments; } }
		internal DateTime StopDate { get { return stopDate; } set { stopDate = value; } }
		//methods
		internal void CreatePayments() {
			payments = PaymentManager.GeneratePayments(this);
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			//personal
			info.AddValue("firstName", firstName);
			info.AddValue("lastName", lastName);
			info.AddValue("birthday", birthday);
			info.AddValue("biPay", biPay);
			info.AddValue("male", male);
			//utilities
			info.AddValue("cableInternet", cableInternet);
			info.AddValue("cellPhone", cellPhone);
			info.AddValue("incWater", incWater);
			info.AddValue("incElectricity", incElectricity);
			info.AddValue("incHeat", incHeat);
			//regular bills
			info.AddValue("otherMonthly", otherMonthly);
			//car
			info.AddValue("carValue", carValue);
			info.AddValue("gasRate", gasRate);
			info.AddValue("monthlyCarPayment", monthlyCarPayment);
			info.AddValue("carMiles", carMiles);
			info.AddValue("carYears", carYears);
			info.AddValue("mpg", mpg);
			//misc
			info.AddValue("pets", pets);
			info.AddValue("rent", rent);
			info.AddValue("collegeLoan", collegeLoan);
			//habits
			info.AddValue("snackFreq", snackFreq);
			info.AddValue("coffeeFreq", coffeeFreq);
			info.AddValue("digitals", digitals);
			//other
			info.AddValue("desiredDate", desiredDate);
			info.AddValue("lastOpened", lastOpened);
			info.AddValue("challengeLevel", challengeLevel);
			info.AddValue("balance", balance);
			info.AddValue("payments", payments);
			info.AddValue("stopDate", stopDate);
		}
		public int CompareTo(Profile other) {
			return lastOpened.CompareTo(other.lastOpened);
		}
	}
}
