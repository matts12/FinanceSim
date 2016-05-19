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
		private string streetAddress;
		private int apartmentNumber;
		private int zipCode;
		private DateTime birthday;
		//income
		private decimal income;
		private decimal savings; //TODO he wants?
		//apartment
		private decimal rentersInsurance;
		private decimal monthlyRent;
		//utilities
		private decimal internet;
		private decimal heat;
		private decimal electricity;
		private decimal water;
		private bool[] utilIncluded;
		//regular bills
		private List<CertainFixedPayment> regularBills;
		//car
		private decimal carValue;
		private int mpg;
		private decimal monthlyCarPayment;
		private bool isCarSavings;
		//pets
		private int dogs;
		private int cats;
		//other
		private DateTime desiredDate;
		private DateTime lastOpened;
		//constructors
		internal Profile() {
			//personal
			firstName = "";
			lastName = "";
			streetAddress = "";
			apartmentNumber = 1;
			zipCode = 03049;
			birthday = DateTime.Today;
			//income
			income = savings = 0m;
			//apartment
			rentersInsurance = monthlyRent = 0m;
			//utilities
			internet = heat = electricity = water = 0m;
			utilIncluded = new bool[4];
			//regular bills
			regularBills = new List<CertainFixedPayment>();
			//car
			carValue = monthlyCarPayment = 0m;
			mpg = 0;
			isCarSavings = false;
			//pets
			dogs = cats = 0;
			//others
			desiredDate = DateTime.Today;
			lastOpened = DateTime.Now;
        }
		internal Profile(SerializationInfo info, StreamingContext context) {
			//personal
			firstName = info.GetString("firstName");
			lastName = info.GetString("lastName");
			streetAddress = info.GetString("streetAddress");
			apartmentNumber = info.GetInt32("apartmentNumber");
			zipCode = info.GetInt32("zipCode");
			birthday = info.GetDateTime("birthday");
			//income
			income = info.GetDecimal("income");
			savings = info.GetDecimal("savings");
			//apartment
			rentersInsurance = info.GetDecimal("rentersInsurance");
			monthlyRent = info.GetDecimal("monthlyRent");
			//utilities
			internet = info.GetDecimal("internet");
			heat = info.GetDecimal("heat");
			electricity = info.GetDecimal("electricity");
			water = info.GetDecimal("water");
			utilIncluded = info.GetValue("utilIncluded", typeof(bool[])) as bool[];
			//regular bills
			regularBills = info.GetValue("regularBills", typeof(List<CertainFixedPayment>)) as List<CertainFixedPayment>;
			//car
			carValue = info.GetDecimal("carValue");
			monthlyCarPayment = info.GetDecimal("monthlyCarPayment");
			mpg = info.GetInt32("mpg");
			isCarSavings = info.GetBoolean("isCarSavings");
			//pets
			dogs = info.GetInt32("dogs");
			cats = info.GetInt32("cats");
			//others
			desiredDate = info.GetDateTime("desiredDate");
			lastOpened = info.GetDateTime("lastOpened");
		}
		//properties
		//personal
		internal string FirstName { get { return firstName; } set { firstName = value; } }
		internal string LastName { get { return lastName; } set { lastName = value; } }
		internal string StreetAddress { get { return streetAddress; } set { streetAddress = value; } }
		internal int ApartmentNumber { get { return apartmentNumber; } set { apartmentNumber = value; } }
		internal int ZipCode { get { return zipCode; } set { zipCode = value; } }
		internal DateTime Birthday { get { return birthday; } set { birthday = value; } }
		//income
		internal decimal Income { get { return income; } set { income = value; } }
		internal decimal Savings { get { return savings; } set { savings = value; } }
		//apartment
		internal decimal RentersInsurance { get { return rentersInsurance; } set { rentersInsurance = value; } }
		internal decimal MonthlyRent { get { return monthlyRent; } set { monthlyRent = value; } }
		//utilities
		internal decimal Internet { get { return internet; } set { internet = value; } }
		internal decimal Heat { get { return heat; } set { heat = value; } }
		internal decimal Electricity { get { return electricity; } set { electricity = value; } }
		internal decimal Water { get { return water; } set { water = value; } }
		internal bool[] UtilIncluded { get { return utilIncluded; } set { utilIncluded = value; } }
		//regular bills
		internal List<CertainFixedPayment> RegularBills { get { return regularBills; } set { regularBills = value; } }
		//car
		internal decimal CarValue { get { return carValue; } set { carValue = value; } }
		internal int MPG { get { return mpg; } set { mpg = value; } }
		internal decimal MonthlyCarPayment { get { return monthlyCarPayment; } set { monthlyCarPayment = value; } }
		internal bool IsCarSavings { get { return isCarSavings; } set { isCarSavings = value; } }
		//pets
		internal int Dogs { get { return dogs; } set { dogs = value; } }
		internal int Cats { get { return cats; } set { cats = value; } }
		//others
		internal DateTime DesiredDate { get { return desiredDate; } set { desiredDate = value; } }
		internal DateTime LastOpened { get { return lastOpened; } set { lastOpened = value; } }
		//methods
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			//personal
			info.AddValue("firstName", firstName);
			info.AddValue("lastName", lastName);
			info.AddValue("streetAddress", streetAddress);
			info.AddValue("apartmentNumber", apartmentNumber);
			info.AddValue("zipCode", zipCode);
			info.AddValue("birthday", birthday);
			//income
			info.AddValue("income", income);
			info.AddValue("savings", savings);
			//apartment
			info.AddValue("rentersInsurance", rentersInsurance);
			info.AddValue("monthlyRent", monthlyRent);
			//utilities
			info.AddValue("internet", internet);
			info.AddValue("heat", heat);
			info.AddValue("electricity", electricity);
			info.AddValue("water", water);
			info.AddValue("utilIncluded", utilIncluded);
			//regular bills
			info.AddValue("regularBills", regularBills);
			//car
			info.AddValue("carValue", carValue);
			info.AddValue("monthlyCarPayment", monthlyCarPayment);
			info.AddValue("mpg", mpg);
			info.AddValue("isCarSavings", isCarSavings);
			//pets
			info.AddValue("dogs", dogs);
			info.AddValue("cats", cats);
			//others
			info.AddValue("desiredDate", desiredDate);
			info.AddValue("lastOpened", lastOpened);
		}
		public int CompareTo(Profile other) {
			return lastOpened.CompareTo(other.lastOpened);
		}
	}
}
