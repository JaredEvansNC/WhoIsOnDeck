using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WhoIsOnDeck
{
	public class StudentSorter
	{
		// Students available to be shown
		private List<string> UnusedStudents;
		// During the sorting process, is used to store the students who were just on deck
		// If needed, will be used to ensure there are enough students to display
		private List<string> PrevOnDeckStudents = new List<string>();
		// Students who have already answered a question, shuffled back into unused when unused is too low
		private List<string> UsedStudents = new List<string>();
		// The current set of on0deck students
		private List<string> StudentsOnDeck = new List<string>();

		public StudentSorter(string nameFilePath) 
		{
			// Read names from file in same directory
			// Put in unused students list
			string allNames = File.ReadAllText(nameFilePath);
			UnusedStudents = new List<string>(allNames.Split(','));

		}

		// Returns an array containing a new set of on-deck students, chosen randomly
		public string[] GetStudentsOnDeck()
		{
			// Store who was on deck, then clear it out
			PrevOnDeckStudents.Clear();
			PrevOnDeckStudents.AddRange(StudentsOnDeck);
			StudentsOnDeck.Clear();

			// Get three unused students to be on deck
			for(int i = 0; i < 3; i++)
			{
				StudentsOnDeck.Add(GetRandomUnusedStudent());
			}

			// Now that we have new students, put the previously used ones back

			UnusedStudents.AddRange(PrevOnDeckStudents);
			
			// Return the array
			return StudentsOnDeck.ToArray();
		}

		// Get's a random student from the unused students list
		private string GetRandomUnusedStudent()
		{
			// Refill the unused students list if we run out
			if (UnusedStudents.Count <= 0)
			{
				UnusedStudents.AddRange(UsedStudents);
				UsedStudents.Clear();

				// If there are still no unused students, grab them from the students who were just on screen
				if (UnusedStudents.Count <= 0)
				{
					UnusedStudents.AddRange(PrevOnDeckStudents);
					PrevOnDeckStudents.Clear();
				}
			}


			// Get a random unused student
			int randIndex = new Random().Next(0, UnusedStudents.Count);
			// Store student name
			string studentName = UnusedStudents[randIndex];
			// Remove from the Unused list
			UnusedStudents.Remove(studentName);
			// Return the name at that index
			return studentName;
			
		}

		// Called to mark a student's name as used when the answer a question
		public void MarkNameAsUsed(string name)
		{
			StudentsOnDeck.Remove(name);
			UsedStudents.Add(name);
		}
	}
}
