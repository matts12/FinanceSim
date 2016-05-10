using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSim {
	class Profile {
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
		private List<CertainPayment> regularBills;
		//car
		private decimal carValue;
		private int mpg;
		private decimal monthlyCarPayment;
		private bool isCarSavings;
		//pets
		private int dogs;
		private int cats;

		private int desiredMonth;
		private int desiredYear;
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
			regularBills = new List<CertainPayment>();
			//car
			carValue = monthlyCarPayment = 0m;
			mpg = 0;
			isCarSavings = false;
			//pets
			dogs = cats = 0;

			desiredMonth = DateTime.Today.Month;
			desiredYear = DateTime.Today.Year;
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
		internal List<CertainPayment> RegularBills { get { return regularBills; } set { regularBills = value; } }
		//car
		internal decimal CarValue { get { return carValue; } set { carValue = value; } }
		internal int MPG { get { return mpg; } set { mpg = value; } }
		internal decimal MonthlyCarPayment { get { return monthlyCarPayment; } set { monthlyCarPayment = value; } }
		internal bool IsCarSavings { get { return isCarSavings; } set { isCarSavings = value; } }
		//pets
		internal int Dogs { get { return dogs; } set { dogs = value; } }
		internal int Cats { get { return cats; } set { cats = value; } }

		internal int DesiredMonth { get { return desiredMonth; } set { desiredMonth = value; } }
		internal int DesiredYear { get { return desiredYear; } set { desiredYear = value; } }
		//methods
	}
}
