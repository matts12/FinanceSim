using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceSim {
	class Profile {
		//members
		private string firstName;
		private string lastName;
		private string streetAddress;
		private int desiredMonth;
		private int desiredYear;
		//constructors
		internal Profile() {
			firstName = "";
			lastName = "";
			streetAddress = "";
			desiredMonth = DateTime.Today.Month;
			desiredYear = DateTime.Today.Year;
		}
		//properties
		internal string FirstName { get { return firstName; } set { firstName = value; } }
		internal string LastName { get { return lastName; } set { lastName = value; } }
		internal string StreetAddress { get { return streetAddress; } set { streetAddress = value; } }
		internal int DesiredMonth { get { return desiredMonth; } set { desiredMonth = value; } }
		internal int DesiredYear { get { return desiredYear; } set { desiredYear = value; } }
		//methods
	}
}
