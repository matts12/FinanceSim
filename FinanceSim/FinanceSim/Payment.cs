using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;

namespace FinanceSim {
	enum Frequency { YEARLY, MONTHLY_DAY, WEEKLY, BI_MONTHLY }
	[Serializable]
	abstract class Payment : ISerializable {
		//members
		private string name;
		private Description desc;
		private string category;
		private Frequency freq;
		//constructors
		internal Payment(string name, Description desc, string category, Frequency freq) {
			this.name = name;
			this.desc = desc;
			this.category = category;
			this.freq = freq;
		}
		internal Payment(SerializationInfo info, StreamingContext context) {
			name = info.GetString("name");
			desc = info.GetValue("desc", typeof(Description)) as Description;
			category = info.GetString("category");
			freq = (Frequency)info.GetInt32("freq");
		}
		//properties
		internal string Name { get { return name; } }
		internal string Category { get { return category; } }
		internal Frequency Freq { get { return freq; } }
		//methods
		internal virtual string FindDescription(decimal payment) { return desc.GetValue(payment); }
		internal abstract decimal GetPayment(DateTime? day);
		internal abstract bool IsDue(DateTime day);
		internal decimal RandRange(decimal lower, decimal upper, Random rand) {
			return (decimal)rand.NextDouble() * (upper - lower) + lower;
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
		internal CertainPayment(string name, Description desc, string category, Frequency freq, DateTime refTime) : base(name, desc, category, freq) {
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
				case Frequency.YEARLY: return (DateTime.IsLeapYear(day.Year) ? day.DayOfYear - 1 : day.DayOfYear) == (DateTime.IsLeapYear(refTime.Year) ? refTime.DayOfYear - 1 : refTime.DayOfYear);
				case Frequency.MONTHLY_DAY: return day.Day == refTime.Day;
				case Frequency.WEEKLY: return day.DayOfWeek == refTime.DayOfWeek;
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
		internal CertainFixedPayment(string name, Description desc, string category, decimal payment, Frequency freq, DateTime refTime) 
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
	[Serializable]
	class CertainRandomPayment : CertainPayment, ISerializable {
		//members
		private decimal upper, lower;
		private Random rand;
		//constructors
		internal CertainRandomPayment(string name, Description desc, string category, decimal lower, decimal upper, Random rand,
			Frequency freq, DateTime refTime) : base(name, desc, category, freq, refTime) {
			this.upper = upper;
			this.lower = lower;
			this.rand = rand;
		}
		internal CertainRandomPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			upper = info.GetDecimal("upper");
			lower = info.GetDecimal("lower");
			rand = info.GetValue("rand", typeof(Random)) as Random;
		}
		//properties
		internal Random Rand { get { return rand; } }
		//methods
		internal override decimal GetPayment(DateTime? day) {
			return -1 * RandRange(lower, upper, rand);
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("upper", upper);
			info.AddValue("lower", lower);
			info.AddValue("rand", rand);
		}
	}
	[Serializable]
	class CertainMonthDepPayment : CertainPayment, ISerializable {
		//members
		private Random rand;
		private decimal[] monthPays;
		private decimal[] variations;
		//constructors
		internal CertainMonthDepPayment(string name, Description desc, string category, Random rand, decimal[] monthPays, decimal[] variations,
			Frequency freq, DateTime refTime) : base(name, desc, category, freq, refTime) {
			this.rand = rand;
			this.monthPays = monthPays;
			this.variations = variations;
		}
		internal CertainMonthDepPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			monthPays = info.GetValue("monthPays", typeof(decimal[])) as decimal[];
			variations = info.GetValue("variations", typeof(decimal[])) as decimal[];
			rand = info.GetValue("rand", typeof(Random)) as Random;
		}
		//methods
		internal override decimal GetPayment(DateTime? day) {
			decimal pBase = monthPays[day.Value.Month - 1];
			decimal variation = variations[day.Value.Month - 1];
			return -1 * RandRange(pBase - variation, pBase + variation, rand);
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("rand", rand);
			info.AddValue("monthPays", monthPays);
			info.AddValue("variations", variations);
		}
	}
	[Serializable]
	abstract class UncertainPayment : Payment, ISerializable {
		//members
		private int[] randTimes;
		private int currRand;
		private Random rand;
		private bool needNewValues;
		private int highAdj;
		private int minTimes, maxTimes;
		//constructors
		internal UncertainPayment(string name, Description desc, string category, Frequency freq, int minTimes, int maxTimes, DateTime month, Random rand, int highAdj) 
			: base(name, desc, category, freq) {
			this.rand = rand;
			currRand = 0;
			needNewValues = false;
			this.minTimes = minTimes;
			this.maxTimes = maxTimes;
			this.highAdj = highAdj;
			RandomizeTimes(month);
		}
		internal UncertainPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			randTimes = info.GetValue("randTimes", typeof(int[])) as int[];
			currRand = info.GetInt32("currRand");
			rand = info.GetValue("rand", typeof(Random)) as Random;
			needNewValues = info.GetBoolean("needNewValues");
			highAdj = info.GetInt32("highAdj");
			minTimes = info.GetInt32("minTimes");
			maxTimes = info.GetInt32("maxTimes");
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
				int r, k = 0;
				do {
					r = rand.Next(lowerBound, upperBound);
					k++;
                }
				while (randTimes.Contains(r) && k < 9999);
				randTimes[i] = r;
			}
			if (randTimes.Length > 1) {
				int level = -1;
				switch (Freq) {
					case Frequency.YEARLY: level = refTime.DayOfYear; break;
					case Frequency.MONTHLY_DAY: level = refTime.Day; break;
					case Frequency.WEEKLY: level = (int)refTime.DayOfWeek; break;
				}
				Array.Sort(randTimes, new DateComparer(level));
			}
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
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("randTimes", randTimes);
			info.AddValue("currRand", currRand);
			info.AddValue("rand", rand);
			info.AddValue("needNewValues", needNewValues);
			info.AddValue("minTimes", minTimes);
			info.AddValue("maxTimes", maxTimes);
			info.AddValue("highAdj", highAdj);
		}
	}
	[Serializable]
	class UncertainFixedPayment : UncertainPayment, ISerializable {
		//members
		private decimal payment;
		//constructors
		internal UncertainFixedPayment(string name, Description desc, string category, decimal payment, Frequency freq, int minTimes, 
			int maxTimes, DateTime refTime, Random rand) : base(name, desc, category, freq, minTimes, maxTimes, refTime, rand, 0) {
			this.payment = payment;
		}
		internal UncertainFixedPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			payment = info.GetDecimal("payment");
		}
		//methods
		internal override decimal GetPayment(DateTime? day) { return -payment; }
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("payment", payment);
		}
	}
	[Serializable]
	class UncertainRandomPayment : UncertainPayment, ISerializable {
		//members
		private decimal upper, lower;
		//constructors
		internal UncertainRandomPayment(string name, Description desc, string category, Frequency freq, decimal lower, decimal upper,
			Random rand, int minTimes, int maxTimes, DateTime month, int highAdj = 0) : base(name, desc, category, freq, minTimes, maxTimes, month, rand, highAdj) {
			this.upper = upper;
			this.lower = lower;
		}
		internal UncertainRandomPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			upper = info.GetDecimal("upper");
			lower = info.GetDecimal("lower");
		}
		//methods
		internal override decimal GetPayment(DateTime? day) {
			return -1 * RandRange(lower, upper, Rand);
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("upper", upper);
			info.AddValue("lower", lower);
		}
	}
	[Serializable]
	class UncertainInputPayment : UncertainPayment, ISerializable {
		//members
		private decimal min, max;
		//constructors
		internal UncertainInputPayment(string name, Description desc, string category, Frequency freq, int minTimes, int maxTimes, 
			DateTime month, Random rand, decimal min = -1, decimal max = -1) : base(name, desc, category, freq, minTimes, maxTimes, month, rand, 0) {
			this.min = min;
			this.max = max;
		}
		internal UncertainInputPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			min = info.GetDecimal("min");
			max = info.GetDecimal("max");
		}
		//methods
		internal override decimal GetPayment(DateTime? day) {
			return new InputDialog().GetInput(this, Category.Equals("Spending Money"), min, max);
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("min", min);
			info.AddValue("max", max);
		}
	}
	[Serializable]
	class CertainInputPayment : CertainPayment, ISerializable {
		//members
		private decimal min, max;
		//constructors
		internal CertainInputPayment(string name, Description desc, string category, Frequency freq,
			DateTime refTime, decimal min = -1, decimal max = -1) : base(name, desc, category, freq, refTime) {
			this.min = min;
			this.max = max;
		}
		internal CertainInputPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			min = info.GetDecimal("min");
			max = info.GetDecimal("max");
		}
		//methods
		internal override decimal GetPayment(DateTime? day) {
			return new InputDialog().GetInput(this, Category.Equals("Spending Money"), min, max);
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("min", min);
			info.AddValue("max", max);
		}
	}
	[Serializable]
	class RelativeRandomPayment : CertainRandomPayment, ISerializable {
		//members
		private int inBetweenMin, inBetweenMax;
		private int inBe;
		private DateTime lastTime;
		//constructors
		internal RelativeRandomPayment(string name, Description desc, string category, decimal lower, decimal upper, Random rand, DateTime refTime, int inBetweenMin, int inBetweenMax) 
			: base(name, desc, category, lower, upper, rand, Frequency.YEARLY, refTime) {
			this.inBetweenMin = inBetweenMin;
			this.inBetweenMax = inBetweenMax;
			inBe = Rand.Next(inBetweenMin, inBetweenMax);
			lastTime = refTime;
        }
		internal RelativeRandomPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			inBetweenMin = info.GetInt32("inBetweenMin");
			inBetweenMax = info.GetInt32("inBetweenMax");
			lastTime = info.GetDateTime("lastTime");
			inBe = info.GetInt32("inBe");
		}
		//methods
		internal override bool IsDue(DateTime day) {
			bool due = day.Subtract(lastTime).Days % inBe == 0;
			if (due) {
				inBe = Rand.Next(inBetweenMin, inBetweenMax);
				lastTime = day;
			}
			return due;
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("inBetweenMin", inBetweenMin);
			info.AddValue("inBetweenMax", inBetweenMax);
			info.AddValue("lastTime", lastTime);
			info.AddValue("inBe", inBe);
		}
	}
	[Serializable]
	class UncertainAlternatingPayment : UncertainPayment, ISerializable {
		//members
		private decimal[] oriPays;
		private string[] oriDescs;
		private int[] chosens;
		private int curr;
		//constructors
		internal UncertainAlternatingPayment(string name, string category, Frequency freq, int times, decimal[] pays, string[] descs,
			DateTime refTime, Random rand) : base(name, null, category, freq, times, times, refTime, rand, 0) {
			oriPays = pays;
			oriDescs = descs;
			GenerateRandIndexes(times);
		}
		internal UncertainAlternatingPayment(SerializationInfo info, StreamingContext context) : base(info, context) {
			oriPays = info.GetValue("oriPays", typeof(decimal[])) as decimal[];
			oriDescs = info.GetValue("oriDescs", typeof(string[])) as string[];
			chosens = info.GetValue("chosens", typeof(int[])) as int[];
			curr = info.GetInt32("curr");
		}
		//methods
		private void GenerateRandIndexes(int size) {
			curr = 0;
			List<int> indexes = new List<int>();
			for (int i = 0; i < size; i++) {
				int j = -1;
				do {
					j = Rand.Next(0, oriPays.Length);
				} while (indexes.Contains(j));
				indexes.Add(j);
			}
			chosens = indexes.ToArray();
		}
		internal override string FindDescription(decimal payment) {
			string desc = oriDescs[chosens[curr]];
			if (curr + 1 == chosens.Length) {
				GenerateRandIndexes(chosens.Length);
			}
			else {
				curr++;
			}
			return desc;
		}
		internal override decimal GetPayment(DateTime? day) {
			return -1 * oriPays[chosens[curr]];
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("oriPays", oriPays);
			info.AddValue("oriDescs", oriDescs);
			info.AddValue("chosens", chosens);
			info.AddValue("curr", curr);
		}
	}
	class DateComparer : IComparer<int> {
		//members
		private int level;
		//consturctors
		internal DateComparer(int level) {
			this.level = level;
		}
		//methods
		public int Compare(int x, int y) {
			if (x > level && y > level || x < level && y < level) {
				return x.CompareTo(y);
			}
			else if (x > level && y < level) {
				return -1;
			}
			return x < level && y > level ? 1 : 0;
		}
	}

	public delegate int DescriptionSelector(decimal payment, int max);
	[Serializable]
	class Description : ISerializable {
		internal enum SelectorType { NONE, RANDOM, COFFEE }
		private static Random rand = new Random();
		private static DescriptionSelector defSelect = (payment, max) => rand.Next(0, max);
		private static DescriptionSelector coffeeSelect = (payment, max) => payment > 2.1m ? 0 : 1;
		//members
		private string[] descs;
		private SelectorType type;
		//constructors
		internal Description(string s){
			descs = new string[] { s };
			type = SelectorType.NONE;
		}
		internal Description(params string[] descs) {
			this.descs = descs;
			type = SelectorType.RANDOM;
		}
		internal Description(SelectorType st, params string[] descs) {
			this.descs = descs;
			type = st;
		}
		public Description(SerializationInfo info, StreamingContext context) {
			descs = info.GetValue("descs", typeof(string[])) as string[];
			type = (SelectorType) info.GetInt32("type");
		}
		//methods
		internal string GetValue(decimal payment) {
			return descs[type.Equals(SelectorType.NONE) ? 0 : (type.Equals(SelectorType.COFFEE) ? coffeeSelect(payment, descs.Length - 1) : defSelect(payment, descs.Length - 1))];
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("descs", descs);
			info.AddValue("type", (int)type);
		}
	}
}
