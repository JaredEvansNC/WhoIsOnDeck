using System;
using System.Collections.Generic;
using System.Text;

namespace WhoIsOnDeck.Models
{
	public class Student
	{
		public string name { get; }
		public string job { get; }
		public int level { get; set;  }
		public int xp { get; set; }

		private static string[] approvedJobs = new string[] { "cleric", "fighter", "ranger", "rogue", "wizard", "jester" };

		public Student(string name, string job, int level, int xp)
		{
			this.name = name;
			this.job = job;
			this.level = level;
			this.xp = xp;
		}

		public static bool IsJobApproved(string job)
		{
			bool jobApproved = false;


			foreach(string j in approvedJobs)
			{
				if(job.Equals(j))
				{
					jobApproved = true;
				}
			}

			return jobApproved;
		}

		public void AddOneXP()
		{
			xp++;
			CheckForLevelUp();
		}

		private void CheckForLevelUp()
		{
			// We level up when the xp is equal to the level * 1.5 rounded up
			if(xp >= XpToNextLevel())
			{
				level++;
				xp = 0;
			}
		}

		public int XpToNextLevel()
		{
			return (int) Math.Ceiling(level * 1.5);
		}

		public override string ToString()
		{
			return name + ":" + job + ":" + level + ":" + xp;
		}
	}
}
