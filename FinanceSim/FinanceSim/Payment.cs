using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FinanceSim {
	enum Frequency { YEARLY, MONTHLY_DAY, WEEKLY, BI_MONTHLY }
	[Serializable]
	abstract class Payment : ISerializable {
		private static Random rand = new Random();
		//members
		private string name;
		private Frequency freq;
		//constructors
		internal Payment(string name, Frequency freq) {
			this.name = name;
			this.freq = freq;
		}
		internal Payment(SerializationInfo info, StreamingContext context) {
			name = info.GetString("name");
			freq = (Frequency)info.GetInt32("freq");
		}
		//properties
		internal string Name { get { return name; } }
		internal Frequency Freq { get { return freq; } }
		//methods
		internal abstract decimal GetPayment();
		internal abstract bool IsDue(DateTime day);
		//statics
		private static DateTime RandomDay() {
			return new DateTime(1, 1, rand.Next(1, 28));
        }
		internal static List<Payment> GeneratePayments(Profile profile) {
			//TODO add payments
			List<Payment> payments = new List<Payment>();
			//pay
			DateTime tuesday = new DateTime(profile.DesiredDate.Year, profile.DesiredDate.Month, 1);
			while (!tuesday.DayOfWeek.Equals(DayOfWeek.Tuesday))
				tuesday = tuesday.AddDays(1);
            payments.Add(new CertainFixedPayment("Paycheck", -1 * profile.BiPay, Frequency.BI_MONTHLY, tuesday));
			//other monthlies
			foreach (CertainFixedPayment cfp in profile.OtherMonthly)
				payments.Add(cfp);
			return payments;
		}
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("name", name);
			info.AddValue("freq", (int)freq);
		}
	}
	[Serializable]
	abstract class CertainPayment : Payment, ISerializable {
		//members
		private DateTime refTime;
		//constructors
		internal CertainPayment(string name, Frequency freq, DateTime refTime) : base(name, freq) {
			this.refTime = refTime;
		}
		internal CertainPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			refTime = info.GetDateTime("refTime");
		}
		//properties
		internal DateTime RefTime { get { return refTime; } }
		//methods
		internal override bool IsDue(DateTime day) {
			switch (Freq) {
				case Frequency.YEARLY: return day.DayOfYear == refTime.DayOfYear;
				case Frequency.MONTHLY_DAY: return day.Day == refTime.Day;
				case Frequency.WEEKLY: Console.WriteLine(day.DayOfWeek + " " + refTime.DayOfWeek); return day.DayOfWeek == refTime.DayOfWeek;
				case Frequency.BI_MONTHLY: return day.DayOfWeek == refTime.DayOfWeek && day.Subtract(refTime).Days % 14 == 0;
			}
			return true;
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("refTime", refTime);
		}
	}
	[Serializable]
	class CertainFixedPayment : CertainPayment, ISerializable {
		//members
		private decimal payment;
		//constructors
		internal CertainFixedPayment(string name, decimal payment, Frequency freq, DateTime refTime) : base(name, freq, refTime) {
			this.payment = payment;
		}
		internal CertainFixedPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			payment = info.GetDecimal("payment");
		}
		//methods
		internal override decimal GetPayment() { return -payment; }
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("payment", payment);
		}
	}
	class CertainRandomPayment : CertainPayment {
		//members
		private decimal upper, lower;
		private Random rand;
		//constructors
		internal CertainRandomPayment(string name, decimal lower, decimal upper, Random rand,
			Frequency freq, DateTime refTime) : base(name, freq, refTime) {
			this.upper = upper;
			this.lower = lower;
			this.rand = rand;
		}
		//methods
		internal override decimal GetPayment() {
			return -1 * ((decimal)rand.NextDouble() * upper + lower);
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
				case Frequency.BI_MONTHLY:
					throw new NotImplementedException();
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
					case Frequency.BI_MONTHLY: throw new NotImplementedException();
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
					case Frequency.BI_MONTHLY: throw new NotImplementedException();
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
		internal UncertainRandomPayment(string name, Frequency freq, decimal lower, decimal upper, Random rand, int times, DateTime month) : base(name, freq, times, month, rand) {
			this.upper = upper;
			this.lower = lower;
		}
		//methods
		internal override decimal GetPayment() {
			return -1 * ((decimal)Rand.NextDouble() * upper + lower);
		}
	}
}
