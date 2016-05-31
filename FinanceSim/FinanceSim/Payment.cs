using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;

namespace FinanceSim {
	enum Frequency { YEARLY, MONTHLY_DAY, WEEKLY, BI_MONTHLY }
	[Serializable]
	abstract class Payment : ISerializable {
		private static Random rand = new Random();
		//members
		private string name;
		private string desc;
		private string category;
		private Frequency freq;
		private bool lastDue;
		private decimal lastPayment;
		//constructors
		internal Payment(string name, string desc, string category, Frequency freq) {
			this.name = name;
			this.desc = desc;
			this.category = category;
			this.freq = freq;
			lastPayment = 0m;
			lastDue = false;
		}
		internal Payment(SerializationInfo info, StreamingContext context) {
			name = info.GetString("name");
			desc = info.GetString("desc");
			category = info.GetString("category");
			freq = (Frequency)info.GetInt32("freq");
		}
		//properties
		internal string Name { get { return name; } }
		internal string Desc { get { return desc; } }
		internal string Category { get { return category; } }
		internal Frequency Freq { get { return freq; } }
		internal decimal LastPayment { get { return lastPayment; } set { lastPayment = value; } }
		internal bool LastDue { get { return lastDue; } set { lastDue = value; } }
		//methods
		internal abstract decimal CalculatePayment(DateTime? day);
		internal abstract bool CalculateIsDue(DateTime day);
		internal decimal RandRange(decimal lower, decimal upper, Random rand) {
			return (decimal)rand.NextDouble() * (upper - lower) + lower;
		}
		//statics
		private static DateTime RandomDay() {
			return new DateTime(1, 1, rand.Next(1, 28));
        }
		internal static List<Payment> GeneratePayments(Profile profile) {
			//TODO add payments
			//TODO apply challenge level
			List<Payment> payments = new List<Payment>();
			//pay
			DateTime tuesday = new DateTime(profile.DesiredDate.Year, profile.DesiredDate.Month, 1);
			while (!tuesday.DayOfWeek.Equals(DayOfWeek.Tuesday))
				tuesday = tuesday.AddDays(1);
            payments.Add(new CertainFixedPayment("Paycheck", "Your bi-weekly paycheck is here.", "Income", -1 * profile.BiPay, Frequency.BI_MONTHLY, tuesday));
			//other monthlies
			foreach (CertainFixedPayment cfp in profile.OtherMonthly)
				payments.Add(cfp);
			//utilities
			payments.Add(new CertainMonthDepPayment("Heating Bill", "Your heating bill is due.", "Utility", rand, new decimal[] {
				97, 92, 75, 60, 30, 0, 0, 0, 25, 40, 75, 90
			}, new decimal[] {
				15, 15, 10, 10, 3, 0, 0, 0, 5, 10, 10, 15
			}, Frequency.MONTHLY_DAY, RandomDay()));
			payments.Add(new CertainMonthDepPayment("Electricity Bill", "Your electricity bill is due.", "Utility", rand, new decimal[] {
				70, 60, 50, 50, 50, 55, 80, 80, 50, 50, 50, 60, 70, 60, 50
			}, new decimal[] {
				10, 5, 5, 5, 5, 10, 15, 15, 10, 5, 5, 10, 10, 5, 5
			}, Frequency.MONTHLY_DAY, RandomDay()));
			payments.Add(new CertainMonthDepPayment("Water Bill", "Your water bill is due.", "Utility", rand, new decimal[] {
				20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20
			}, new decimal[] {
				7, 7,7,7,7,7,7,7,7,7,7,7
			}, Frequency.MONTHLY_DAY, RandomDay()));
			//car
			//food
			payments.Add(new UncertainRandomPayment("Buy Food", "You need this to survive.", "Food", Frequency.WEEKLY, 75m, 95m, rand, 1, 1, profile.DesiredDate, 2));
			payments.Add(new UncertainRandomPayment("Meal Out", "You enjoy a lunch out with friends.", "Food", Frequency.MONTHLY_DAY, 6.5m, 15.5m, rand, 2, 3, profile.DesiredDate));
			payments.Add(new UncertainRandomPayment("Meal Out", "You enjoy a dinner out with friends.", "Food", Frequency.MONTHLY_DAY, 9.5m, 29.5m, rand, 1, 2, profile.DesiredDate));
			payments.Add(new UncertainRandomPayment("Snack", "Get a snack to make your hunger go away.", "Food", Frequency.WEEKLY, 1m, 3.95m, rand, profile.SnackFreq/2, profile.SnackFreq, profile.DesiredDate));
			payments.Add(new UncertainRandomPayment("Coffee", "Start the day with some coffee.", "Food", Frequency.WEEKLY, 1.5m, 3.95m, rand, profile.CoffeeFreq / 2, profile.CoffeeFreq, profile.DesiredDate));
			//TODO different descriptions depending on payment, or randomized
			//spending $
			//medical
			payments.Add(new CertainRandomPayment("Dentist", "Get your teeth cleaned with a visit to the dentist.", "Medical", 6950m, 13950m, rand, Frequency.YEARLY, RandomDay()));
			//TODO
			//bad stuff
			//good stuff
			//misc

			return payments;
		}
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("name", name);
			info.AddValue("desc", desc);
			info.AddValue("category", category);
			info.AddValue("freq", (int)freq);
		}
	}
	[Serializable]
	abstract class CertainPayment : Payment, ISerializable {
		//members
		private DateTime refTime;
		//constructors
		internal CertainPayment(string name, string desc, string category, Frequency freq, DateTime refTime) : base(name, desc, category, freq) {
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
		internal CertainFixedPayment(string name, string desc, string category, decimal payment, Frequency freq, DateTime refTime) 
			: base(name, desc, category, freq, refTime) {
			this.payment = payment;
		}
		internal CertainFixedPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			payment = info.GetDecimal("payment");
		}
		//methods
		internal override decimal GetPayment(DateTime? day) { return -payment; }
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
		internal CertainRandomPayment(string name, string desc, string category, decimal lower, decimal upper, Random rand,
			Frequency freq, DateTime refTime) : base(name, desc, category, freq, refTime) {
			this.upper = upper;
			this.lower = lower;
			this.rand = rand;
		}
		//methods
		internal override decimal GetPayment(DateTime? day) {
			return -1 * RandRange(lower, upper, rand);
		}
	}
	class CertainMonthDepPayment : CertainPayment {
		//members
		private Random rand;
		private decimal[] monthPays;
		private decimal[] variations;
		//constructors
		internal CertainMonthDepPayment(string name, string desc, string category, Random rand, decimal[] monthPays, decimal[] variations,
			Frequency freq, DateTime refTime) : base(name, desc, category, freq, refTime) {
			this.rand = rand;
			this.monthPays = monthPays;
			this.variations = variations;
		}
		//methods
		internal override decimal GetPayment(DateTime? day) {
			decimal pBase = monthPays[day.Value.Month - 1];
			decimal variation = variations[day.Value.Month - 1];
			return -1 * RandRange(pBase - variation, pBase + variation, rand);
		}
	}
	abstract class UncertainPayment : Payment {
		//members
		private int[] randTimes;
		private int currRand;
		private Random rand;
		private bool needNewValues;
		private int highAdj;
		private int minTimes, maxTimes;
		//constructors
		internal UncertainPayment(string name, string desc, string category, Frequency freq, int minTimes, int maxTimes, DateTime month, Random rand, int highAdj) 
			: base(name, desc, category, freq) {
			this.rand = rand;
			currRand = 0;
			needNewValues = false;
			this.minTimes = minTimes;
			this.maxTimes = maxTimes;
			this.highAdj = highAdj;
			RandomizeTimes(month);
		}
		//properties
		protected Random Rand { get { return rand; } }
		//methods
		protected void RandomizeTimes(DateTime refTime) {
			randTimes = new int[rand.Next(minTimes, maxTimes)];
			int upperBound = -1, lowerBound = 1;
			switch (Freq) {
				case Frequency.YEARLY:
					upperBound = DateTime.IsLeapYear(refTime.Year) ? 366 - highAdj : 365 - highAdj;
					break;
				case Frequency.MONTHLY_DAY:
					upperBound = DateTime.DaysInMonth(refTime.Year, refTime.Month) - highAdj;
					break;
				case Frequency.WEEKLY:
					upperBound = 6 - highAdj;
					lowerBound = 0;
					break;
				case Frequency.BI_MONTHLY:
					throw new NotImplementedException();
			}
			for (int i = 0; i < randTimes.Length; i++) {
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
					RandomizeTimes(day.Add(TimeSpan.FromDays(1)));
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
		internal UncertainRandomPayment(string name, string desc, string category, Frequency freq, decimal lower, decimal upper,
			Random rand, int minTimes, int maxTimes, DateTime month, int highAdj = 0) : base(name, desc, category, freq, minTimes, maxTimes, month, rand, highAdj) {
			this.upper = upper;
			this.lower = lower;
		}
		//methods
		internal override decimal GetPayment(DateTime? day) {
			return -1 * RandRange(lower, upper, Rand);
		}
	}
	class UncertainInputPayment : UncertainPayment {
		//members
		//constructors
		internal UncertainInputPayment(string name, string desc, string category, Frequency freq, int minTimes, int maxTimes, 
			DateTime month, Random rand, int highAdj = 0) : base(name, desc, category, freq, minTimes, maxTimes, month, rand, highAdj) {

		}
		//methods
		internal override decimal GetPayment(DateTime? day) {
			string result = Microsoft.VisualBasic.Interaction.InputBox("Enter a Value", "Enter a Value"); //TODO
			Console.WriteLine(result);
			return 0m;
		}
	}
	class RelativeRandomPayment : CertainRandomPayment {
		//members
		private DateTime nextDue;
		private int inBetween;
		private bool requestNew;
		//constructors
		internal RelativeRandomPayment(string name, string desc, string category, Frequency freq, decimal lower, decimal upper, Random rand, DateTime refTime, int inBetween) : base(name, desc, category, lower, upper, rand, freq, refTime) {
			nextDue = refTime;
			this.inBetween = inBetween;
			requestNew = false;
		}
		//methods
		internal override bool IsDue(DateTime day) {
			switch (Freq) {
				case Frequency.YEARLY: return day.DayOfYear == refTime.DayOfYear;
				case Frequency.MONTHLY_DAY: return day.Day == refTime.Day;
				case Frequency.WEEKLY: Console.WriteLine(day.DayOfWeek + " " + refTime.DayOfWeek); return day.DayOfWeek == refTime.DayOfWeek;
				case Frequency.BI_MONTHLY: return day.DayOfWeek == refTime.DayOfWeek && day.Subtract(refTime).Days % 14 == 0;
			}
		}
		internal override decimal GetPayment(DateTime? day) {
			return base.GetPayment(day);
		}
	}
}
