using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceSim {
	enum Frequency { YEARLY, MONTHLY_DAY, WEEKLY }
	abstract class Payment {
		private static Random rand = new Random();
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
		//statics
		internal static List<Payment> GeneratePayments(Profile profile) {
			//TODO
			List<Payment> payments = new List<Payment>();
			payments.Add(new CertainFixedPayment("Health care", 500, Frequency.MONTHLY_DAY, new DateTime(2016, 5, 7)));
			payments.Add(new UncertainRandomPayment("Broken stuff", Frequency.WEEKLY, 5.00m, 10.00m, rand, 4, DateTime.Today));
			return payments;
		}
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
		private bool needNewValues;
		//constructors
		internal UncertainPayment(string name, Frequency freq, int times, DateTime month, Random rand) : base(name, freq) {
			this.rand = rand;
			currRand = 0;
			needNewValues = false;
			RandomizeTimes(times, month);
		}
		//properties
		protected Random Rand { get { return rand; } }
		//methods
		protected void RandomizeTimes(int times, DateTime refTime) {
			randTimes = new int[times];
			int upperBound = -1, lowerBound = 1;
			switch (Freq) {
				case Frequency.YEARLY:
					upperBound = DateTime.IsLeapYear(refTime.Year) ? 366 : 365;
					break;
				case Frequency.MONTHLY_DAY:
					upperBound = DateTime.DaysInMonth(refTime.Year, refTime.Month);
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
			if (needNewValues) {
				bool renewable = false;
				switch (Freq) {
					case Frequency.YEARLY: renewable = day.DayOfYear == (DateTime.IsLeapYear(day.Year) ? 366 : 365); break;
					case Frequency.MONTHLY_DAY: renewable = day.Day == DateTime.DaysInMonth(day.Year, day.Month); break;
					case Frequency.WEEKLY: renewable = day.DayOfWeek == DayOfWeek.Saturday; break;
	            }
				if (renewable) {
					RandomizeTimes(randTimes.Length, day.Add(TimeSpan.FromDays(1)));
					currRand = 0;
					needNewValues = false;
				}
				return false;
			}
			else {
				bool isDue = false;
				switch (Freq) {
					case Frequency.YEARLY: isDue = day.DayOfYear == randTimes[currRand]; break;
					case Frequency.MONTHLY_DAY: isDue = day.Day == randTimes[currRand]; break;
					case Frequency.WEEKLY: isDue = (int)day.DayOfWeek == randTimes[currRand]; break;
				}
				if (isDue) {
					if (currRand + 1 < randTimes.Length) {
						currRand++;
					}
					else {
						needNewValues = true;
					}
				}
				return isDue;
			}
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
