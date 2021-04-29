using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using WhoIsOnDeck.Models;

namespace WhoIsOnDeck
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private StudentSorter Sorter;

		private Storyboard storyboard = new Storyboard();

		private const string namesFilePath = ".\\classnames.txt";

		public MainWindow()
		{
			InitializeComponent();

			// Try to create a new sorter object, if there are any IO exceptions, print out an error message
			try
			{
				Sorter = new StudentSorter(namesFilePath);
			}
			catch(IOException e)
			{
				ErrorBlock.Text =	"There was an error, and the students couldn't be loaded.\n" +
									"Make sure there is a classnames.txt file in the same directory as the .exe, with names separated by commas.\n\n" +
									"ERROR: " + e.Message;
			}

			// Begin the storyboard with no children
			storyboard.Begin(this, true);
		}

		// If a student's name is click, mark them off as having answered, and get a new set of students
		private void StudentNameMouseDown(object sender, MouseButtonEventArgs e)
		{

			// Don't do this if there's no sorter OR if there's an animation playing
			if (CanInteractWithScreen()) {

				// Sort through the stack panel's children and find all the objects
				StackPanel sp = (StackPanel)sender;

				TextBlock name = null;
				Image image = null;
				TextBlock level = null;
				TextBlock xp = null;

				foreach(Object o in sp.Children)
				{
					if(o is TextBlock)
					{
						TextBlock tempTextBlock = (TextBlock)o;
						if(tempTextBlock.Name.Contains("Name"))
						{
							name = tempTextBlock;
						}
						else if(tempTextBlock.Name.Contains("Level"))
						{
							level = tempTextBlock;
						}
						else if (tempTextBlock.Name.Contains("XP"))
						{
							xp = tempTextBlock;
						}
					}
					else if(o is Image)
					{
						image = (Image)o;
					}
				}

				// Only do this if we found a student
				if (name != null)
				{
					// Get the name from this button
					string textBlockName = name.Text;

					// Get the student object
					Student student = Sorter.FindOnDeckStudentByName(textBlockName);

					// Store current level
					int oldLevel = student.level;

					// Add xp
					student.AddOneXP();

					// Update text displays
					SetLevelDisplay(student, level);
					SetXPDisplay(student, xp);

					// If there's a name here, mark it as used
					if (!textBlockName.Equals(""))
					{
						Sorter.MarkStudentAsUsed(student);
					}

					// Start saving all the student data
					Sorter.SaveAllStudentData(namesFilePath);

					// Setup variables for animation
					string tbToAnimate = xp.Name;
					double startSize = 14.0;

					// If the level has changed animate that instead
					if (oldLevel != student.level)
					{
						tbToAnimate = level.Name;
						startSize = 16.0;
					}

					// Animate the correct panel
					storyboard.Children.Clear();    // clear any previous anims

					var panelAnim = new DoubleAnimation();   // setup the new animation
					panelAnim.From = startSize;
					panelAnim.To = startSize + 6.0;
					panelAnim.Duration = new Duration(TimeSpan.FromSeconds(0.75));
					panelAnim.AutoReverse = true;
					panelAnim.Completed += (s, e) =>
					{
						// TODO - Don't reset all students after xp gain
						// Get a new set of students on anim completion
						// 4.29 Commented out so that multiple students can gain xp
						//GetNewStudentsOnDeck();
					};


					// Add to storyboard
					storyboard.Children.Add(panelAnim);
					Storyboard.SetTargetName(panelAnim, tbToAnimate);
					Storyboard.SetTargetProperty(panelAnim, new PropertyPath(TextBlock.FontSizeProperty));

					// Start storyboard
					storyboard.Begin(this, true);
				}

			}
		}

		// Retrieves a new set of students from the sorter and displays their names on screen
		private void GetNewStudentsOnDeck()
		{
			// Fill all three labels with new random students
			Student[] studentsOnDeck = Sorter.GetStudentsOnDeck();

			// Set names
			StudentNameOne.Text = studentsOnDeck[0].name;
			StudentNameTwo.Text = studentsOnDeck[1].name;
			StudentNameThree.Text = studentsOnDeck[2].name;

			// Set images
			SetImageFromJob(StudentImageOne, studentsOnDeck[0].job);
			SetImageFromJob(StudentImageTwo, studentsOnDeck[1].job);
			SetImageFromJob(StudentImageThree, studentsOnDeck[2].job);

			// Set job and level
			SetLevelDisplay(studentsOnDeck[0], StudentLevelOne);
			SetLevelDisplay(studentsOnDeck[1], StudentLevelTwo);
			SetLevelDisplay(studentsOnDeck[2], StudentLevelThree);

			// Set xp
			SetXPDisplay(studentsOnDeck[0], StudentXPOne);
			SetXPDisplay(studentsOnDeck[1], StudentXPTwo);
			SetXPDisplay(studentsOnDeck[2], StudentXPThree);
		}

		private void SetLevelDisplay(Student s, TextBlock tb)
		{
			tb.Text = "Lv " + s.level + " " + CapitalizeFirstLetter(s.job);
		}

		private void SetXPDisplay(Student s, TextBlock tb)
		{
			tb.Text = "XP " + s.xp + " / " + s.XpToNextLevel();
		}
		private string CapitalizeFirstLetter(string s)
		{
			return char.ToUpper(s[0]) + s.Substring(1);
		}
		private void SetImageFromJob(Image img, string job)
		{
			if (Student.IsJobApproved(job)) {
				img.Source = new BitmapImage(new Uri("Images/" + job + ".png", UriKind.Relative));
			}
			else
			{
				img.Source = new BitmapImage(new Uri("Images/jester.png", UriKind.Relative));
			}
		}

		private void NewSetButtonMouseDown(object sender, RoutedEventArgs e)
		{
			// Don't do this if there's no sorter
			if (CanInteractWithScreen())
			{
				GetNewStudentsOnDeck();
			}
		}

		private bool CanInteractWithScreen()
		{
			// If there is no valid sorter object, don't allow interaction
			if(Sorter == null)
			{
				return false;
			}

			// If we're animating, don't allow interactions
			ClockState check = storyboard.GetCurrentState(this);
			bool p = storyboard.GetIsPaused(this);
			if (storyboard.GetCurrentState(this) == ClockState.Active)
			{
				return false;
			}

			return true;
		}
	}
}
