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
using System.Windows.Forms;

namespace Sudoku
{
    /************************************************************
     *                                                          *
     *  Class Name:  Program                                    *
     *                                                          *
     *  Purpose:  This is the driving class of the program.     *
     *            It initializes the data pools for the app     *
     *            and then launches the form gui.               *
     *                                                          *
     ************************************************************/
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.IO.StreamReader puzzleStreamReader;      // for pathnames of puzzles from directory file
            //System.IO.StreamReader individualPuzzleStreamReader;    // for individual puzzles read via pathnames from directory file


            // try catches below attempt to open the files and if they can't open or aren't found, an error message prints and the program returns/exits.
            // input files are placed in the base folder of the project structure
            try
            {
                puzzleStreamReader = new System.IO.StreamReader("..\\..\\..\\puzzles\\directory.txt");
            }
            catch (Exception)
            {
                Console.WriteLine("Error: directory.txt input file was not found or could not be read.");
                return;
            }


            // these while loops below are reading the input from the input files one line at a time and being
            // added to their respective pools ("global" variables) to be used by the entire application.
            string input;   // input is use as a buffer to hold the input coming in from the input files while it is being handled.
            while ((input = puzzleStreamReader.ReadLine()) != null)
            {
                string[] parsedInput = input.Split('/');
                string[] parsedSavedTime;
                string[] parsedFastestTime;

                System.IO.StreamReader individualPuzzleStreamReader;

                try
                {
                    individualPuzzleStreamReader = new System.IO.StreamReader("..\\..\\..\\puzzles\\" + input);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error: " + input + " file was not found or could not be read.");
                    return;
                }

                Puzzle puzzle = new Puzzle(input);


                int[,] initialPuzzleState = new int[9, 9];
                int[,] solutionState = new int[9, 9];
                int[,] savedState = new int[9, 9];
                string puzzleLine;

                int z = 0;

                while ((puzzleLine = individualPuzzleStreamReader.ReadLine()) != null)
                {
                    int c = 0;

                    if (z < 9)                                              // read in intial puzzle state that will always be in the first 9 lines
                    {
                        foreach (char character in puzzleLine)
                        {
                            int x = (int)Char.GetNumericValue(character);

                            initialPuzzleState[z, c] = x;

                            c++;
                        }

                        z++;
                    }
                    else if (z >= 10 && z < 19)                             // read in solution state that will always be in the file at lines 10-19
                    {
                        foreach (char character in puzzleLine)
                        {
                            int x = (int)Char.GetNumericValue(character);

                            solutionState[(z -1) % 9, c] = x;

                            c++;
                        }
                        z++;
                    } else if(z == 20)                                      // if the while loop executes again, there is another line here, check if it is a saved time or fastest time
                    {
                        if (puzzleLine.Contains("saved"))
                        {
                            parsedSavedTime = puzzleLine.Split('/');
                            puzzle.SavedAttemptTime = Convert.ToInt32(parsedSavedTime[1]);  // assign saved time to puzzle.SavedAttemptTime if found in file.
                            puzzle.PreviouslySaved = 1;
                            puzzle.Cheated = 0;
                        }
                        else if (puzzleLine.Contains("cheatsave"))
                        {
                            parsedSavedTime = puzzleLine.Split('/');
                            puzzle.SavedAttemptTime = Convert.ToInt32(parsedSavedTime[1]);  // assign cheatsaved time to puzzle.SavedAttemptTime if found in file.
                            puzzle.PreviouslySaved = 1;
                            puzzle.Cheated = 1;                                 // set Cheated to 1 to let program know the saved attempt was cheated on before, so that it will not record completion time for this puzzle.
                        }
                        else if (puzzleLine.Contains("fastest"))
                        {
                            parsedFastestTime = puzzleLine.Split('/');
                            puzzle.FastestAttemptTime = Convert.ToInt32(parsedFastestTime[1]);  // assign fastest time to puzzle.FastestAttemptTime if found in file.
                            puzzle.PreviouslyCompleted = 1;

                        }
                        z++;
                    }
                    else if (z >= 22 & z < 31)                              // if z progressed to 22, it means there is a saved state saved here.  IF THERE WAS NO SAVE STATE.  Z WOULD NEVER GET TO 22. 
                    {
                        foreach (char character in puzzleLine)
                        {
                            int x = (int)Char.GetNumericValue(character);

                            savedState[(z - 4) % 9, c] = x;

                            c++;
                        }
                        z++;
                    }
                    else if (z == 32)                                       // if z got to 32, there is a fastest time saved AFTER a saved state, read it in.
                    {
                        parsedFastestTime = puzzleLine.Split('/');
                        puzzle.FastestAttemptTime = Convert.ToInt32(parsedFastestTime[1]);  // assign fastest time to puzzle.FastestAttemptTime if found in file.
                        puzzle.PreviouslyCompleted = 1;
                    }
                    else
                    {
                        z++;
                    }

                }

                // if the puzzle has not been attempted before and file only contains initial state and solution
                if(z >= 19)
                {
                    if (parsedInput[0] == "easy")
                        puzzle.Difficulty = 0;
                    else if (parsedInput[0] == "medium")
                        puzzle.Difficulty = 1;
                    else
                        puzzle.Difficulty = 2;


                    //puzzle.PreviouslyCompleted = 0;

                    //puzzle.FilePathName = input;

                    puzzle.Initial = initialPuzzleState;
                    puzzle.Solution = solutionState;
                    
                    if(z >= 22)
                    {
                        //puzzle.PreviouslySaved = 1;
                        puzzle.Saved = savedState;

                    }
                }

                switch (puzzle.Difficulty)
                {
                    case 0:
                        Global.PuzzlePool[0].Add(puzzle);   // add puzzle to "easy" puzzle list in puzzlePool[0]
                        break;
                    case 1:
                        Global.PuzzlePool[1].Add(puzzle);   // add puzzle to "medium" puzzle list in puzzlePool[1]
                        break;
                    case 2:
                        Global.PuzzlePool[2].Add(puzzle);   // add puzzle to "hard" puzzle list in puzzlePool[2]
                        break;
                    default:
                        break;
                }

            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
