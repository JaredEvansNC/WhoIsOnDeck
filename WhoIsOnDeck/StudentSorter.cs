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
		// Used temporarily during the selection process and cleared afterwards
		private List<string> StudentsSelectedToBeOnDeck = new List<string>();
		// Students who have already answered a question, shuffled back into unused when unused is too low
		private List<string> UsedStudents = new List<string>();
		// The current set of on0deck students
		private string[] StudentsOnDeck = new string[3];

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
			// Get three unused students to be on deck
			for(int i = 0; i < 3; i++)
			{
				StudentsOnDeck[i] = GetRandomUnusedStudent();
			}

			// Now that the students have been selected, move the selected students back into the unused list
			UnusedStudents.AddRange(StudentsSelectedToBeOnDeck);
			StudentsSelectedToBeOnDeck.Clear();

			// Return the array
			return StudentsOnDeck;
		}

		// Get's a random student from the unused students list
		private string GetRandomUnusedStudent()
		{
			// Refill the unused students list if we run out
			if(UnusedStudents.Count <= 0)
			{
				UnusedStudents.AddRange(UsedStudents);
				UsedStudents.Clear();
			}

			// Get a random unused student
			int randIndex = new Random().Next(0, UnusedStudents.Count);
			// Store student name
			string studentName = UnusedStudents[randIndex];
			// Add this name to the selected list
			StudentsSelectedToBeOnDeck.Add(studentName);
			// Remove from the Unused list
			UnusedStudents.Remove(studentName);
			// Return the name at that index
			return studentName;
		}

		// Called to mark a student's name as used when the answer a question
		public void MarkNameAsUsed(string name)
		{
			UsedStudents.Add(name);
			UnusedStudents.Remove(name);
		}
	}
}
