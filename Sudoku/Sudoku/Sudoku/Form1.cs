/************************************************************
 *                                                          *
 *  Program Name: Sudoku                                    *
 *                                                          *
 *  Date: 05/06/2021                                        *
 *                                                          *
 *  Programmers:  Ahmed Khan                                *
 *                                                          *
 *  Purpose:  The premise of this project is simple: a      *
 *      Sudoku puzzle game.                                 *
 *                                                          *
 ************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sudoku
{
    /************************************************************
     *                                                          *
     *  Class Name:  Form1                                      *
     *                                                          *
     *  Purpose:  This is the class that implements all the     *
     *            functionality of the sudoku form controls.    *
     *            Form implements a basic sudoku game.          *
     *                                                          *
     ************************************************************/
    public partial class Form1 : Form
    {
        // Label used to hold the last label selected to enter keydown value into
        public static Label lastLabelSelected;

        // Puzzle object that holds the current puzzle being done
        static Puzzle puzzle;

        // 2d int array that is used to hold the current values of the sudoku grid
        static int[,] puzzleInProgress = new int[9, 9];

        // int variables to use for drawing of grid on Sudoku_Canvas
        public static int W;
        public static int L;

        // _ticks holds attempt time in seconds
        private int _ticks = 0;

        // formats time to HH:MM:SS
        private TimeSpan timeFormatter;

        // used to check if the game is paused or not
        private bool isPaused;


        /***********************************************************
        *                                                          *
        *  Method Name:  Form1()                                   *
        *                                                          *
        *  Parameters: none                                        *
        *                                                          *
        *  Purpose:  This is the constructor of the Form1 class    *
        *            that initializes the form.                    *
        *                                                          *
        *  Return:  constructors have no return.                   *
        *                                                          *
        ************************************************************/
        public Form1()
        {
            InitializeComponent();

            W = Sudoku_Canvas.Width;
            L = Sudoku_Canvas.Height;


            //load_Puzzle(3);     // load random puzzle into grid, will look for a saved or unsolved puzzle first.  DIDNT NEED THIS AFTER TALKING TO MR. ROGNESS!
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  Sudoku_Canvas_Paint()                     *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an PaintEventArgs object.*
        *                                                          *
        *  Purpose:  This method paints/draws using graphics and   *
        *            a grid to encompass the 9 3x3 blocks of       *
        *            sudoku.                                       *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void Sudoku_Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;        // get graphics object from e


            using(Pen pen = new Pen(Color.Black, 5f))       // allocate pen t draw grid
            {
                for(int i = 0; i <= 9; i++)
                {
                    if(i % 3 == 0)
                    {
                        // vertical block lines
                        g.DrawLine(pen, (i * W / 9), 0, (i * W / 9), L);

                        // horizontal block lines
                        g.DrawLine(pen, 0, (i * L / 9), W, (i * L / 9));
                    }
                }

            }



        }


        /***********************************************************
        *                                                          *
        *  Method Name:  label_Click()                             *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method is called when any one of the     *
        *            81 labels/cells ar clicked on.                *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void label_Click(object sender, EventArgs e)
        {
            // Labels technically don't have input focus.  Need to use click event in conjunction with two other methods to SIMULATE entering and leaving focus.
            // change the background color of lastLabelSelected, if not null, in "focus" to white before changing "focus" to the newly clicked label.
            if (lastLabelSelected != null)
            {
                label_LeaveFocus(lastLabelSelected);
            }

            // cast sender as a Label
            Label label = sender as Label;

            // make clicked Label enter "focus"
            label_EnterFocus(label);

            // setting lastLabelSelected to the clicked label so this click method knows what the last selected label was to "remove its focus"
            lastLabelSelected = label;
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  label_EnterFocus()                        *
        *                                                          *
        *  Parameters: a Label object                              *
        *                                                          *
        *  Purpose:  This method simulates entering focus on a     *
        *            label that has just been seleceted.           *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void label_EnterFocus(Label label)
        {
            label.BackColor = Color.LightGray;
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  label_LeaveFocus()                        *
        *                                                          *
        *  Parameters: a Label object                              *
        *                                                          *
        *  Purpose:  This method simulates leaving focus on a      *
        *            label that was previously selected.           *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void label_LeaveFocus(Label label)
        {
            if (label.BackColor == Color.Red || label.BackColor == Color.Red)   // if user had checked progress and cell in focus turned red, leave it red instead of turning it white
                return;

            label.BackColor = Color.White;
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  Form1_KeyDown()                           *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              sender object and an KeyEventArgs object.   *
        *                                                          *
        *  Purpose:  This method runs anytime any key is pressed   *
        *            when the program is running, checks if the    *
        *            input is valid and records it if it is.       *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (lastLabelSelected == null || !((e.KeyValue > 48 && e.KeyValue <= 57) || e.KeyCode == Keys.Back))    // check to see if user typed a number or hit backspace
                return;

            //string labelID = Regex.Replace(lastLabelSelected.Name, "[^0-9]", "");      // removing all alphabetic characters and leaving only numeric values by using regex

            if (e.KeyCode == Keys.Back)
                lastLabelSelected.Text = "";        // delete existing text if backspace was pressed down
            else
                lastLabelSelected.Text = ((char)e.KeyValue).ToString();
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  easyToolStripMenuItem_Click()             *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method is called when a user clicks      *
        *            new game easy difficulty.                     *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {

            load_Puzzle(0);     // load an easy puzzle, looks for first saved/unsolved puzzle first.

        }


        /***********************************************************
        *                                                          *
        *  Method Name:  mediumToolStripMenuItem_Click()           *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method is called when a user clicks      *
        *            new game medium difficulty.                   *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {

            load_Puzzle(1);     // load a medium puzzle, looks for first saved/unsolved puzzle first.

        }


        /***********************************************************
        *                                                          *
        *  Method Name:  hardToolStripMenuItem_Click()             *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method is called when a user clicks      *
        *            new game hard difficulty.                     *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            load_Puzzle(2);     // load a hard puzzle, looks for first saved/unsolved puzzle first.

        }


        /***********************************************************
        *                                                          *
        *  Method Name:  timer1_Tick()                             *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method is called by timer1 control       *
        *            everry second.                                *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void timer1_Tick(object sender, EventArgs e)
        {
            resetTimer();
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  resetTimer()                              *
        *                                                          *
        *  Parameters: none                                        *
        *                                                          *
        *  Purpose:  This method increments _ticks by 1 and        *
        *            and displays it in timeElapsedLabel.          *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void resetTimer()
        {   
            _ticks++;

            timeFormatter = TimeSpan.FromSeconds(_ticks);

            timeElapsedLabel.Text = timeFormatter.ToString();       // use TimeSpan object to display _ticks in HH:MM:SS

        }


        /***********************************************************
        *                                                          *
        *  Method Name:  Pause_Click()                             *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method runs when the pause/resume button *
        *            is clicked and pauses or resumes the game.    *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void Pause_Click(object sender, EventArgs e)
        {
            if (puzzle == null)     //if no puzzle is loaded, igonre reset button.
                return;


            if (isPaused)       // check if game was paused when the button (resume) was clicked.
            {

                checkProgressButton.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                cheatButton.Enabled = true;

                int i = 0;

                // fill the cells with the values held in puzzleInProgress that were saved when the game was paused.
                while (i < 81)
                {
                    Control control = this.Controls["label" + (i + 1)];

                    if(puzzleInProgress[i / 9, i % 9] != 0)
                        control.Text = puzzleInProgress[i / 9, i % 9].ToString();

                    i++;
                }

                timer1.Start();             // start timer
                pauseButton.Text = "Pause";     // change button text to pause after resuming game
                messageLabel.Text = "";     // clear any message previously displayed
                isPaused = false;
            } else
            {
                checkProgressButton.Enabled = false;
                saveToolStripMenuItem.Enabled = false;
                cheatButton.Enabled = false;

                int i = 0;

                // update puzzleInProgress with current values in cells to prepare for hiding values while the game is paused.
                while (i < 81)
                {
                    Control control = this.Controls["label" + (i + 1)];
                    puzzleInProgress[i / 9, i % 9] = (control.Text.Length != 0 ? Convert.ToInt32(control.Text) : 0);
                    i++;
                }

                i = 0;

                // update puzzleInProgress with blank values to hide them while the game is paused.
                while (i < 81)
                {
                    Control control = this.Controls["label" + (i + 1)];
                    control.Text = "";
                    i++;
                }

                timer1.Stop();                          // start timer
                pauseButton.Text = "Resume";            // change button text to resume after pausing game
                messageLabel.ForeColor = Color.Black;
                messageLabel.Text = "Game is paused.";  // display message that the game is paused.
                isPaused = true;
            }
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  Reset_Click()                             *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method runs when the reset button        *
        *            is clicked and resets the puzzle to initial.  *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void Reset_Click(object sender, EventArgs e)
        {

            if (puzzle == null)     //if no puzzle is loaded, igonre reset button.
                return;

            messageLabel.ForeColor = Color.Black;
            pauseButton.Text = "Pause";     // change pause button back to "Pause" if it was "Resume"
            isPaused = false;
            cheatedLabel.Text = "";     // reset cheated label and puzzle flag
            puzzle.Cheated = 0;         // set cheated to 0 since this is a reset

            // clear last label that was selected
            if(lastLabelSelected != null)
                label_LeaveFocus(lastLabelSelected);
            lastLabelSelected = null;


            // reset the grid with the values from the intial state of the puzzle
            for (int i = 0; i < 81; i++)
            {
                Control control = this.Controls["label" + (i + 1)];     // getting control/ label by name

                if (puzzle.Initial[i / 9, i % 9] != 0)                  // fill initial values and disable the label from being clicked or edited if not equal to 0
                {
                    control.Text = puzzle.Initial[i / 9, i % 9].ToString();
                    control.BackColor = Color.White;
                    control.Font = new Font(control.Font, FontStyle.Bold);
                } else
                {
                    control.Text = null;
                    control.BackColor = Color.White;        // clear text, change backColor to white, forecolor to blue, and enable the label for input.
                    control.ForeColor = Color.DodgerBlue;
                    control.Enabled = true;
                }
            }


            pauseButton.Enabled = true;              // reenabling pause and check button if user was at the end of the game and hit reset
            checkProgressButton.Enabled = true;
            messageLabel.Text = "Resetted the puzzle and attempt time.";

            timeElapsedLabel.Text = "00:00:00";     // reset timeElapsedLabel
            _ticks = 0;                             // reset seconds count

            cheatButton.Enabled = true;             // enable all buttons
            pauseButton.Enabled = true;
            checkProgressButton.Enabled = true;
            Reset.Enabled = true;

            timer1.Start();                         // start timer

        }


        /***********************************************************
        *                                                          *
        *  Method Name:  saveToolStripMenuItem_Click()             *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method runs when the save menustrip item *
        *            is clicked and saves the current attempt      *
        *            to the original puzzle file.                  *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (puzzle == null)     //if no puzzle is loaded, igonre reset button.
                return;

            save(0);        // call save function, 0 is to indicate that save method should state current grid and time (1 would mean not to save the current attempt and time)
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  save()                                    *
        *                                                          *
        *  Parameters: an int flag used to tell if the current     *
        *              grid and time should save to file.          *     
        *              0 means to save, 1 means not to save.       *
        *                                                          *
        *  Purpose:  This method runs when the save menustrip item *
        *            is clicked and saves the current attempt      *
        *            to the original puzzle file.                  *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void save(int flag)
        {
            int i = 0;

            // update puzzleInProgress with current values in cells to prepare for saving
            while (i < 81)
            {
                Control control = this.Controls["label" + (i + 1)];
                puzzleInProgress[i / 9, i % 9] = (control.Text.Length != 0 ? Convert.ToInt32(control.Text) : 0);
                i++;
            }


            StreamWriter streamWriter;               // create streamwriter



            try
            {
                streamWriter = new System.IO.StreamWriter("..\\..\\..\\puzzles\\" + puzzle.FilePathName); // intiailize streamwriter with path of puzzle txt file, this constructor for streamwriter overwrites all existing data in the file
            }
            catch (Exception)
            {
                Console.WriteLine("Error: \"" + puzzle.FilePathName + "\" file was not found or could not be opened for writing in saveToolStripMenuItem_Click.");
                return;
            }

            i = 0;

            // writing initial puzzle to file
            while (i < 81)
            {
                if (i % 9 == 0 && i != 0)
                    streamWriter.WriteLine();       // add a new line every 9 numbers

                //streamWriter.Write(0);
                streamWriter.Write(puzzle.Initial[i / 9, i % 9]);

                i++;
            }

            streamWriter.WriteLine();               // for spacing
            streamWriter.WriteLine();

            i = 0;

            // writing solution to file
            while (i < 81)
            {
                if (i % 9 == 0 && i != 0)
                    streamWriter.WriteLine();       // add a new line every 9 numbers

                streamWriter.Write(puzzle.Solution[i / 9, i % 9]);

                i++;
            }

            // this code block only executes when "save" menustrip item is pressed.  When a puzzle is successfully 
            // completed, flag will equal 1 and so the saved attempt if any, will be cleared or rather, the current grid values wont be saved/written.
            if (flag == 0)       
            {
                streamWriter.WriteLine();           // for spacing
                streamWriter.WriteLine();

                if(puzzle.Cheated == 0)     // 0 means puzzle WAS NOT cheated on
                    streamWriter.WriteLine("saved/" + _ticks);      // write saved string (to be used to check if there is a saved attempt when reading the files in) and attempt time
                else
                    streamWriter.WriteLine("cheatsave/" + _ticks);      // write cheatsave string (to be used to check if there is a cheated saved attempt when reading the files in) and attempt time


                i = 0;

                streamWriter.WriteLine();
                // writing puzzleInProgress to file
                while (i < 81)
                {
                    if (i % 9 == 0 && i != 0)
                        streamWriter.WriteLine();   // add a new line every 9 numbers

                    streamWriter.Write(puzzleInProgress[i / 9, i % 9]);

                    i++;
                }
            }




            // write fastest attempt time to file if there is one recorded for the puzzle greater than 0
            if (puzzle.FastestAttemptTime > 0)
            {
                streamWriter.WriteLine();           // for spacing
                streamWriter.WriteLine();
                streamWriter.WriteLine("fastest/" + puzzle.FastestAttemptTime);
            }


            streamWriter.Flush();   // flush streamwriter to write output to file

            messageLabel.Text = "Saved attempt and time to file";
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  load_Puzzle()                             *
        *                                                          *
        *  Parameters: an int flag used to tell which difficulty   *
        *              puzzle is being loaded or if its random.    *
        *              0 = easy                                    *
        *              1 = medium                                  *
        *              2 = hard                                    *
        *              3 = random (1st saved/unsolved puzzle found)*
        *                                                          *
        *  Purpose:  This method runs when the program first starts*
        *            and when a new game is started.               *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void load_Puzzle(int flag)
        {
            _ticks = 0;                 // set _ticks/timer to 0
            messageLabel.Text = "";     // remove any previous messages
            cheatedLabel.Text = "";     // reset cheated label text
            pauseButton.Text = "Pause";
            isPaused = false;

            cheatButton.Enabled = true;
            pauseButton.Enabled = true;
            checkProgressButton.Enabled = true;
            Reset.Enabled = true;

            timer1.Start();             // start the timer


            // clear last selected label if this isnt the first puzzle to be loaded.
            if (lastLabelSelected != null)
            {
                label_LeaveFocus(lastLabelSelected);
                lastLabelSelected = null;
            }


            // if there was a previous puzzle loaded into the grid, clear it before loading new one.
            if (puzzle != null)
            {
                // clear any previous cells from previous puzzle
                for (int i = 0; i < 81; i++)
                {
                    Control control = this.Controls["label" + (i + 1)];

                    control.Text = null;

                    control.ForeColor = Color.Black;
                    control.BackColor = Color.White;
                    control.Font = new Font(control.Font, FontStyle.Regular);
                    control.Enabled = true;
                }
            }


            pauseButton.Enabled = true;
            checkProgressButton.Enabled = true;

            Random rnd = new Random();              // create random number generator

            if (flag == 0)      // if easy puzzle is being loaded
            {
                int i = 0;


                foreach (Puzzle item in Global.PuzzlePool[0])       // check to see if there is a saved or unsolved puzzle
                {

                    if (item.PreviouslyCompleted == 1)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        puzzle = item;
                        break;
                    }



                }

                int randomPuzzle = rnd.Next(0, Global.PuzzlePool[0].Count);
                int z = 0;

                if (i == Global.PuzzlePool[0].Count)                    // if no saved or unsolved puzzles found, load a previously completed one
                {
                    foreach (Puzzle item in Global.PuzzlePool[0])
                    {

                        if (z == randomPuzzle)
                        {
                            puzzle = item;
                            break;
                        }
                        z++;
                    }
                }
            } 




            else if(flag == 1)      // if medium puzzle is being loaded
            {
                int i = 0;


                foreach (Puzzle item in Global.PuzzlePool[1])           // check to see if there is a saved or unsolved puzzle
                {

                    if (item.PreviouslyCompleted == 1)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        puzzle = item;
                        break;
                    }



                }

                int randomPuzzle = rnd.Next(0, Global.PuzzlePool[1].Count);
                int z = 0;

                if (i == Global.PuzzlePool[1].Count)                        // if no saved or unsolved puzzles found, load a previously completed one
                {
                    foreach (Puzzle item in Global.PuzzlePool[1])
                    {

                        if (z == randomPuzzle)
                        {
                            puzzle = item;
                            break;
                        }
                        z++;
                    }
                }
            } 
            else if (flag == 2)     // if hard puzzle is being loaded
            {
                int i = 0;


                foreach (Puzzle item in Global.PuzzlePool[2])               // check to see if there is a saved or unsolved puzzle
                {

                    if (item.PreviouslyCompleted == 1)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        puzzle = item;
                        break;
                    }



                }

                int randomPuzzle = rnd.Next(0, Global.PuzzlePool[2].Count);
                int z = 0;

                if (i == Global.PuzzlePool[2].Count)                        // if no saved or unsolved puzzles found, load a previously completed one
                {
                    foreach (Puzzle item in Global.PuzzlePool[2])
                    {

                        if (z == randomPuzzle)
                        {
                            puzzle = item;
                            break;
                        }
                        z++;
                    }
                }

            }

            // this region of code was to run when the program first started.  Mr. Rogness said that the program SHOULD NOT 
            // automatically load a puzzle when it starts.  THe user should select new game and a difficulty.
            #region
            //else if (flag == 3)         // if random puzzle (1st saved/unsolved puzzle found) is being loaded
            //{
            //    //Random rnd = new Random();

            //    int i = 0;

            //    while(i < 3)
            //    {
            //        foreach (Puzzle item in Global.PuzzlePool[i])
            //        {
            //            if (item.PreviouslyCompleted == 1)
            //            {
            //                continue;
            //            }
            //            else
            //            {
            //                puzzle = item;
            //                break;
            //            }
            //        }

            //        if (puzzle != null)
            //            break;                  // break out of while loop once a random puzzle has been chosen.

            //        i++;
            //    }


            //    //foreach (Puzzle item in Global.PuzzlePool[rnd.Next(0, 3)])
            //    //{

            //    //    if (item.PreviouslyCompleted == 1)
            //    //    {
            //    //        continue;
            //    //    }
            //    //    else
            //    //    {
            //    //        puzzle = item;
            //    //        break;
            //    //    }
            //    //}



            //    int randomDifficulty = rnd.Next(0, Global.PuzzlePool.Count);

            //    int randomPuzzle = rnd.Next(0, Global.PuzzlePool[randomDifficulty].Count);

            //    i = 0;

            //    if (puzzle == null)
            //    {
            //        foreach (Puzzle item in Global.PuzzlePool[randomDifficulty])
            //        {

            //            if (i < randomPuzzle)
            //            {
            //                i++;
            //                continue;
            //            }
            //            else
            //            {
            //                puzzle = item;
            //                break;
            //            }
            //        }
            //    }


            //}
            #endregion



            if (puzzle.PreviouslySaved == 1)        // if a saved puzzle was found, load it.
            {
                // load a saved puzzle
                Array.Copy(puzzle.Saved, puzzleInProgress, 81);
                messageLabel.Text = "Loaded a saved attempt that was found.";

                if(puzzle.Cheated == 1)         // if Cheated is set to 1, display message that completion time will not be recorded.
                {
                    cheatedLabel.Text = "This attempt was cheated on.  Completion time will not be saved!";
                }
            }
            else
            {
                // load new puzzle into cells
                Array.Copy(puzzle.Initial, puzzleInProgress, 81);
                messageLabel.Text = "No saved attempt found.  Starting new game.";
            }


            // fill the grid with the values of the saved attempt.
            for (int i = 0; i < 81; i++)
            {
                Control control = this.Controls["label" + (i + 1)];
                Label label = control as Label;

                if (puzzle.Initial[i / 9, i % 9] != 0)
                {
                    label.Text = puzzle.Initial[i / 9, i % 9].ToString();
                    label.Font = new Font(control.Font, FontStyle.Bold);                                    // set font ot bold and disable the label from being clicked on since this is a given value
                    label.Enabled = false;
                } else
                {
                    label.ForeColor = Color.DodgerBlue;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    if (puzzleInProgress[i / 9, i % 9] != puzzle.Initial[i / 9, i % 9])                      // if there was a saved attempt, fill in user input labels/cells
                        label.Text = puzzleInProgress[i / 9, i % 9].ToString();
                }
            }

            // if a saved attempt was loaded, set _ticks to the savedAttemptTime
            if (puzzle.SavedAttemptTime > 0)
            {
                _ticks = puzzle.SavedAttemptTime;
            }


            label83.Text = puzzle.FilePathName;         // set the pathname as the text for label83
        }


        /***********************************************************
        *                                                          *
        *  Method Name:  checkProgressButton_Click()               *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method runs when the check solution item *
        *            is clicked and saves the current attempt      *
        *            to the original puzzle file.                  *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void checkProgressButton_Click(object sender, EventArgs e)
        {

            if (puzzle == null)     //if no puzzle is loaded, igonre reset button.
                return;

            Control control;

            bool errorFound = false;

            // set background of all cells to white, except for those filled in with cheat.
            for (int c = 0; c < 81; c++)
            {
                control = this.Controls["label" + (c + 1)];

                if(control.BackColor != Color.LightGreen)
                    control.BackColor = Color.White;
            }

            int i = 0;

            // update puzzleInProgress with current values in cells to prepare for checking progress
            while (i < 81)
            {
                control = this.Controls["label" + (i + 1)];
                puzzleInProgress[i / 9, i % 9] = (control.Text.Length != 0 ? Convert.ToInt32(control.Text) : 0);
                i++;
            }




            List<Label> labelList = new List<Label>();

            // check 3x3 grids for duplicate values
            // received this 3 nested for loops code block from Mr. Rogness when speaking with him.
            for (int x = 0; x < 9; x++)
            {
                bool foundDuplicate = false;

                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {

                        control = this.Controls["label" + ((((x % 3) * 3 + j) * 9) + ((x / 3) * 3 + k) + 1)];
                        Label label = control as Label;

                        labelList.Add(label);
                    }
                }

                foreach (Label label in labelList)      // check each label against all other labels in labelList to see if they have duplicate values
                {

                    foreach (Label label2 in labelList)
                    {
                        if (label != label2 && (label.Text == label2.Text && label.Text != "" && label2.Text != ""))
                        {
                            label2.BackColor = Color.Red;   // set cell backColor to red for label with duplicate value
                            foundDuplicate = true;
                            errorFound = true;
                        }
                    }
                }

                if (foundDuplicate)
                {
                    foreach (Label label in labelList)
                    {
                        if (label.BackColor != Color.Red)
                            label.BackColor = Color.Tomato; // set all other cells in 3x3 block to tomato (lighter red).
                    }
                    foundDuplicate = false;
                }

                labelList.Clear();

            }




            // check each row for duplicate values
            labelList.Clear();

            for (int x = 0; x < 9; x++)
            {
                bool foundDuplicate = false;

                for (int z = 0; z < 9; z++)
                {

                    control = this.Controls["label" + (((x * 9) + z) + 1)];
                    Label label = control as Label;

                    labelList.Add(label);                   // add each label in each row to check for duplicates

                }

                // check each label in labelList to see if the other labels in their have a duplicate value and set that label's backColor to red
                foreach (Label label in labelList)
                {

                    foreach (Label label2 in labelList)
                    {
                        if (label != label2 && (label.Text == label2.Text && label.Text != "" && label2.Text != ""))
                        {
                            label2.BackColor = Color.Red;
                            foundDuplicate = true;
                            errorFound = true;
                        }
                    }
                }

                // if duplicate was found in a row, set all cells to light red (tomato) except for cells that are already incorrect
                if (foundDuplicate)
                {
                    foreach (Label label in labelList)
                    {
                        if (label.BackColor != Color.Red)
                            label.BackColor = Color.Tomato;
                    }
                    foundDuplicate = false;
                }

                labelList.Clear();
            }





            // check each column for duplicate values
            labelList.Clear();

            for (int x = 0; x < 9; x++)
            {
                bool foundDuplicate = false;

                for (int z = 0; z < 9; z++)
                {

                    control = this.Controls["label" + (((z * 9) + x) + 1)];         // add each label in the column to labelList
                    Label label = control as Label;

                    labelList.Add(label);                   // add each label in each column to check for duplicates

                }

                // check each label in labelList to see if the other labels in their have a duplicate value and set that label's backColor to red
                foreach (Label label in labelList)
                {

                    foreach (Label label2 in labelList)
                    {
                        if (label != label2 && (label.Text == label2.Text && label.Text != "" && label2.Text != ""))    // check for duplicate
                        {
                            label2.BackColor = Color.Red;
                            foundDuplicate = true;
                            errorFound = true;
                        }
                    }
                }

                // if duplicate was found in a column, set all cells to light red (tomato) except for cells that are already incorrect
                if (foundDuplicate)
                {
                    foreach (Label label in labelList)
                    {
                        if (label.BackColor != Color.Red)
                            label.BackColor = Color.Tomato;
                    }
                    foundDuplicate = false;
                }

                labelList.Clear();      // clear labelList for next column
            }




            bool isPuzzleFilled = true;
            
            i = 0;

            // check each cell if there is a value entered to see if it matches the solution and mark them red if they dont.
            while (i < 81)
            {
                control = this.Controls["label" + (i + 1)];
                if(control.Text == "")                      // check if all labels/cells are filled.  If any cell is empty, set isPuzzleFilled to false.
                {
                    isPuzzleFilled = false;
                }

                if (control.Text.Length > 0 && puzzle.Solution[i / 9, i % 9] != puzzleInProgress[i / 9, i % 9])
                {
                    control.BackColor = Color.Red;
                    errorFound = true;
                }
                i++;
            }




            if (!errorFound)    // if no error found
            {
                if (isPuzzleFilled)             // if there was no error or duplicate and isPuzzleFilled is true, that means that the user has completed the puzzle.
                {
                    timer1.Stop();
                    if (_ticks > puzzle.FastestAttemptTime && puzzle.Cheated != 1)  // update fastest time if _ticks is greater and if the puzzle has not been cheated on (1 means cheated).
                        puzzle.FastestAttemptTime = _ticks;
                    puzzle.PreviouslyCompleted = 1;
                    save(1);
                    messageLabel.ForeColor = Color.Green;
                    messageLabel.Text = "Congratulations!  You have solved the puzzle! Goto File->New Game and select a difficulty to start a new game.";

                    int attemptTimeSum = 0;         // variable to store sum of all completion times found for use calculating average for message box
                    int count = 0;                  // count to use to calculate average
                    int fastestAttemptTime = Int32.MaxValue;     // variable to store fastest time for given difficulty in message box, initialized to max value of an int32 as we are looking for fastest/minimum time
                    string difficulty;

                    if (puzzle.Difficulty == 0)      // get average and fastest time for easy difficulty
                    {
                        foreach (Puzzle item in Global.PuzzlePool[0])
                        {
                            if(item.FastestAttemptTime > 0)         // if a puzzle has a fastet attempt time recorded greater than 0, use it for calculations.
                            {
                                if (item.FastestAttemptTime < fastestAttemptTime)
                                {
                                    fastestAttemptTime = item.FastestAttemptTime;           // set fastestAttemptTime to which ever puzzle attempt time that is the fastest for given difficulty
                                }

                                attemptTimeSum += item.FastestAttemptTime;
                                count++;
                            }

                        }
                        difficulty = "easy";
                    } else if(puzzle.Difficulty == 1)   // get average and fastest time for medium difficulty
                    {
                        foreach (Puzzle item in Global.PuzzlePool[1])
                        {
                            if (item.FastestAttemptTime > 0)         // if a puzzle has a fastet attempt time recorded greater than 0, use it for calculations.
                            {
                                if (item.FastestAttemptTime < fastestAttemptTime)
                                {
                                    fastestAttemptTime = item.FastestAttemptTime;           // set fastestAttemptTime to which ever puzzle attempt time that is the fastest for given difficulty
                                }

                                attemptTimeSum += item.FastestAttemptTime;
                                count++;
                            }

                        }
                        difficulty = "medium";
                    }
                    else    // get average and fastest time for hard difficulty
                    {
                        foreach (Puzzle item in Global.PuzzlePool[2])
                        {
                            if (item.FastestAttemptTime > 0)         // if a puzzle has a fastet attempt time recorded greater than 0, use it for calculations.
                            {
                                if (item.FastestAttemptTime < fastestAttemptTime)
                                {
                                    fastestAttemptTime = item.FastestAttemptTime;           // set fastestAttemptTime to which ever puzzle attempt time that is the fastest for given difficulty
                                }

                                attemptTimeSum += item.FastestAttemptTime;
                                count++;
                            }

                        }
                        difficulty = "hard";
                    }

                    if (fastestAttemptTime == Int32.MaxValue)       // if fastestAttemptTime is still equal to Int32.MaxValue, set it to 0
                        fastestAttemptTime = 0;

                    float averageAttemptTime = 0;

                    if (attemptTimeSum > 0 && count > 0)            // only compute this if both values are not 0, otherwise you get NaN
                        averageAttemptTime = (float)attemptTimeSum / (float)count;

                    timeFormatter = TimeSpan.FromSeconds(_ticks);
                    TimeSpan timeFormatter2 = TimeSpan.FromSeconds(fastestAttemptTime);

                    TimeSpan timeformatter3 = TimeSpan.FromSeconds(Math.Round(averageAttemptTime));

                    DialogResult dialogResult;

                    if (puzzle.Cheated == 0)
                    {
                        // show message box with completion details
                        dialogResult = MessageBox.Show("Completion Time: " + timeFormatter.ToString() + " (HH:MM:SS)\nFastest Completion Time (" + difficulty + "): " + timeFormatter2.ToString() + " (HH:MM:SS)\nAverage Completion Time (" + difficulty + "): " + timeformatter3.ToString() + " (HH:MM:SS)", "Results", MessageBoxButtons.OK);
                    } else
                    {
                        // show message box with completion details
                        dialogResult = MessageBox.Show("Completion Time: " + timeFormatter.ToString() + " (HH:MM:SS)\nFastest Completion Time (" + difficulty + "): " + timeFormatter2.ToString() + " (HH:MM:SS)\nAverage Completion Time (" + difficulty + "): " + timeformatter3.ToString() + " (HH:MM:SS)\n\nCheating was detected.  Completion time will not be recorded!", "Results", MessageBoxButtons.OK);
                    }



                    for (int c = 0; c < 81; c++)
                    {
                        control = this.Controls["label" + (c + 1)];

                        control.Enabled = false;                        // disable the puzzle from being edited after it is completed
                    }

                    pauseButton.Enabled = false;
                    checkProgressButton.Enabled = false;
                    cheatButton.Enabled = false;
                }  
                else
                {
                    messageLabel.ForeColor = Color.Black;
                    messageLabel.Text = "No errors found, you are doing great!";        // display message if no error found
                }

            }
            else
            {
                messageLabel.ForeColor = Color.Red;
                messageLabel.Text = "Errors or duplicates found, check errors highlighted in dark red above!";  // display error if errors or duplicates found
            }
        }



        /***********************************************************
        *                                                          *
        *  Method Name:  cheatButton_Click()                       *
        *                                                          *
        *  Parameters: an object called sender representing the    *
        *              clicked object and an EventArgs object.     *
        *                                                          *
        *  Purpose:  This method runs when the cheat button        *
        *            is clicked. Functionally, this option should  *
        *            choose (1) a random, un-filled cell and       *
        *            insert the correct number or (if the puzzle   *
        *            is completely filled but incorrect) (2)       *
        *            choose the first cell found in a sequential   *
        *            search (down each row, for each and every     *
        *            row) that isn't correct, and change it to     *
        *            the correct value.                            *
        *                                                          *
        *  Return:  void, no return.                               *
        *                                                          *
        ************************************************************/
        private void cheatButton_Click(object sender, EventArgs e)
        {
            if (puzzle == null)     //if no puzzle is loaded, igonre reset button.
                return;

            messageLabel.Text = "";

            Control control;

            bool isPuzzleFilled = true;     // intially set to true, if the while loop below detects that one of the labels dont have any text entered in them, this will be set to false.
            bool flag = false;

            int i = 0;
            List<Label> labelList = new List<Label>();

            // update puzzleInProgress with current values in cells to prepare for checking progress
            while (i < 81)
            {
                control = this.Controls["label" + (i + 1)];
                Label label = control as Label;

                if(control.Text == null || control.Text == "" || control.Text.Length == 0) // if the while loop below detects that one of the labels dont have any text entered in them, set isPuzzleFilled to false.
                {
                    labelList.Add(label);

                    if (flag == false)
                    {
                        isPuzzleFilled = false;
                        flag = true;
                    }
                }
                
               i++;
            }


            if(isPuzzleFilled == false)  // if puzzle hasn't been filled yet, fill a random label/cell with the correct value.
            {
                Random rnd = new Random();

                Label randomLabel = labelList[rnd.Next(0, labelList.Count)];        // get random label from labelList

                string labelID = Regex.Replace(randomLabel.Name, "[^0-9]", "");     // removing all alphabetic characters and leaving only numeric values by using regex

                int labelToArrayIndex = Convert.ToInt32(labelID) - 1;               // subtract 1 since labels go from 1-81 and array index are 0-80

                randomLabel.Text = puzzle.Solution[labelToArrayIndex / 9, labelToArrayIndex % 9].ToString();    // 

                randomLabel.BackColor = Color.LightGreen;
            } else
            {
                i = 0;

                while (i < 81)                                                      // if all cells are filled but there are errors, do a sequential search for the first cell with an incorrect value change it
                {
                    control = this.Controls["label" + (i + 1)];
                    Label label = control as Label;

                    if (control.Text != puzzle.Solution[i / 9, i % 9].ToString()) // if the while loop below detects that one of the labels dont have any text entered in them, set isPuzzleFilled to false.
                    {
                        control.Text = puzzle.Solution[i / 9, i % 9].ToString();
                        control.BackColor = Color.LightGreen;
                        break;
                    }

                    i++;
                }
            }


            puzzle.Cheated = 1;     // set puzzle's Cheated int to 1 to show the puzzle was cheated on.

            cheatedLabel.Text = "This attempt was cheated on.  Completion time will not be saved!";
        }

    }
}
