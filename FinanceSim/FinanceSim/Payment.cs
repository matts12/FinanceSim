using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceSim {
	enum Frequency { YEARLY, MONTHLY_DAY, WEEKLY }
	abstract class Payment {
		//members
		private string name;
		private Frequency freq;
		//constructors
		internal Payment(string name, Frequency freq) {
			this.name = name;
			this.freq = freq;
		}
		//properties
		internal string Name { get { return name; } }
		internal Frequency Freq { get { return freq; } }
		//methods
		internal abstract decimal GetPayment();
		internal abstract bool IsDue(DateTime day);
		
	}
	abstract class CertainPayment : Payment {
		//members
		private DateTime refTime;
		//constructors
		internal CertainPayment(string name, Frequency freq, DateTime refTime) : base(name, freq) {
			this.refTime = refTime;
		}
		//methods
		internal override bool IsDue(DateTime day) {
			//TODO
			switch (Freq) {
				case Frequency.YEARLY: return day.DayOfYear == refTime.DayOfYear;
				case Frequency.MONTHLY_DAY: return day.Day == refTime.Day;
				case Frequency.WEEKLY: return day.DayOfWeek == refTime.DayOfWeek;
			}
			return true;
		}
	}
	class CertainFixedPayment : CertainPayment {
		//members
		private decimal payment;
		//constructors
		internal CertainFixedPayment(string name, decimal payment, Frequency freq, DateTime refTime) : base(name, freq, refTime) {
			this.payment = payment;
		}
		//methods
		internal override decimal GetPayment() { return payment; }
	}
	class CertainRandomPayment : CertainPayment {
		//members
		private decimal upper, lower;
		private Random rand;
		//constructors
		internal CertainRandomPayment(string name, decimal upper, decimal lower, Random rand,
			Frequency freq, DateTime refTime) : base(name, freq, refTime) {
			this.upper = upper;
			this.lower = lower;
			this.rand = rand;
		}
		//methods
		internal override decimal GetPayment() {
			return (decimal)rand.NextDouble() * upper + lower;
		}
	}
	abstract class UncertainPayment : Payment {
		//members
		private int[] randTimes;
		private int currRand;
		private Random rand;
		//constructors
		internal UncertainPayment(string name, Frequency freq, int times, DateTime month, Random rand) : base(name, freq) {
			RandomizeTimes(times, month);
			this.rand = rand;
			currRand = 0;
		}
		//properties
		protected Random Rand { get { return rand; } }
		//methods
		protected void RandomizeTimes(int times, DateTime month) {
			randTimes = new int[times];
			int upperBound = -1, lowerBound = 1;
			switch (Freq) {
				//TODO
				case Frequency.YEARLY:
					upperBound = DateTime.IsLeapYear(month.Year) ? 366 : 365;
					break;
				case Frequency.MONTHLY_DAY:
					upperBound = DateTime.DaysInMonth(month.Year, month.Month);
					break;
				case Frequency.WEEKLY:
					upperBound = 6;
					lowerBound = 0;
					break;
			}
			for (int i = 0; i < times; i++) {
				int r;
				do {
					r = rand.Next(lowerBound, upperBound);
                }
				while (randTimes.Contains(r));
				randTimes[i] = r;
			}
			Array.Sort(randTimes);
		}
		internal override bool IsDue(DateTime day) {
			bool isDue = true;
			switch (Freq) {
				case Frequency.YEARLY: isDue = day.DayOfYear == randTimes[currRand]; break;
				case Frequency.MONTHLY_DAY: isDue = day.Day == randTimes[currRand]; break;
				case Frequency.WEEKLY: isDue = (int)day.DayOfWeek == randTimes[currRand]; break;
			}
			if (isDue && currRand < randTimes.Length) {
				currRand++;
			}
			return isDue;
		}
	}
	class UncertainRandomPayment : UncertainPayment {
		//members
		private decimal upper, lower;
		//constructors
		internal UncertainRandomPayment(string name, Frequency freq, decimal upper, decimal lower, Random rand, int times, DateTime month) : base(name, freq, times, month, rand) {
			this.upper = upper;
			this.lower = lower;
		}
		//methods
		internal override decimal GetPayment() {
			return (decimal)Rand.NextDouble() * upper + lower;
		}
	}
}
