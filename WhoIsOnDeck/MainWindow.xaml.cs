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

namespace WhoIsOnDeck
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private StudentSorter Sorter;

		private Storyboard storyboard = new Storyboard();

		public MainWindow()
		{
			InitializeComponent();

			// Try to create a new sorter object, if there are any IO exceptions, print out an error message
			try
			{
				Sorter = new StudentSorter(".\\classnames.txt");
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

				TextBlock tb = (TextBlock)sender;

				// Get the name from this button
				string textBlockName = tb.Text;

				// If there's a name here, mark it as used
				if (!textBlockName.Equals("")) {
					Sorter.MarkNameAsUsed(textBlockName);
				}

				// Animate the student's name
				storyboard.Children.Clear();	// clear any previous anims

				var nameAnim = new DoubleAnimation();	// setup the new animation
				nameAnim.From = 12.0;
				nameAnim.To = 18.0;
				nameAnim.Duration = new Duration(TimeSpan.FromSeconds(0.5));
				nameAnim.AutoReverse = true;
				nameAnim.Completed += (s, e) =>
				{
					// Get a new set of students on anim completion
					GetNewStudentsOnDeck();
				};

				// Add to storyboard
				storyboard.Children.Add(nameAnim);
				Storyboard.SetTargetName(nameAnim, tb.Name);
				Storyboard.SetTargetProperty(nameAnim, new PropertyPath(TextBlock.FontSizeProperty));

				// Start storyboard
				storyboard.Begin(this, true);

			}
		}

		// Retrieves a new set of students from the sorter and displays their names on screen
		private void GetNewStudentsOnDeck()
		{
			// Fill all three labels with new random students
			string[] studentsOnDeck = Sorter.GetStudentsOnDeck();
			StudentOne.Text = studentsOnDeck[0];
			StudentTwo.Text = studentsOnDeck[1];
			StudentThree.Text = studentsOnDeck[2];
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
