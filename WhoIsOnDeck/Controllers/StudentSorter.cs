using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WhoIsOnDeck.Models;

namespace WhoIsOnDeck
{
	public class StudentSorter
	{
		// Students available to be shown
		private List<Student> UnusedStudents;
		// During the sorting process, is used to store the students who were just on deck
		// If needed, will be used to ensure there are enough students to display
		private List<Student> PrevOnDeckStudents = new List<Student>();
		// Students who have already answered a question, shuffled back into unused when unused is too low
		private List<Student> UsedStudents = new List<Student>();
		// The current set of on0deck students
		private List<Student> StudentsOnDeck = new List<Student>();

		public StudentSorter(string nameFilePath) 
		{
			// Read names from file in same directory
			// Put in unused students list
			string allNames = File.ReadAllText(nameFilePath);
			string[] rawStrings = allNames.Split(',');

			UnusedStudents = new List<Student>();

			// Generate new students based on strings
			// Add them to the Unused Students list
			foreach (string s in rawStrings)
			{
				string[] data = s.Split(':');
				UnusedStudents.Add(new Student(data[0], data[1], Int32.Parse(data[2]), Int32.Parse(data[3])));
			}
			

		}

		// Returns an array containing a new set of on-deck students, chosen randomly
		public Student[] GetStudentsOnDeck()
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
		private Student GetRandomUnusedStudent()
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
			Student student = UnusedStudents[randIndex];
			// Remove from the Unused list
			UnusedStudents.Remove(student);
			// Return the name at that index
			return student;
			
		}

		// Called to mark a student's name as used when the answer a question
		public void MarkStudentAsUsed(Student student)
		{
			StudentsOnDeck.Remove(student);
			UsedStudents.Add(student);
		}
	
		public Student FindOnDeckStudentByName(string name)
		{
			foreach(Student s in StudentsOnDeck) {
				if(s.name.Equals(name))
				{
					return s;
				}
			}

			return null;
		}
	}
}
