using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceSim {
	enum Frequency { YEARLY, MONTHLY_DAY, MONTHLY_DOW, WEEKLY, DAILY }
	abstract class Payment {
		//members
		private string name;
		private DateTime refTime;
		private Frequency freq;
		//constructors
		internal Payment(string name) {
			this.name = name;
		}
		//properties
		internal string Name { get { return name; } }
		//methods
		internal abstract decimal GetPayment();
		internal bool IsDue(DateTime day) {
			//TODO
			return false;
		}
	}
	class RandomPayment : Payment {
		//members
		private decimal upper, lower;
		private Random rand;
		//constructors
		internal RandomPayment(string name, decimal upper, decimal lower, Random rand) : base(name) {
			this.upper = upper;
			this.lower = lower;
			this.rand = rand;
		}
		//properties

		//methods
		internal override decimal GetPayment() {
			return (decimal)rand.NextDouble() * upper + lower;
		}
	}

}
