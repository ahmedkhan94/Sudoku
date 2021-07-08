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

namespace Sudoku
{
    /************************************************************
     *                                                          *
     *  Class Name:  Puzzle                                     *
     *                                                          *
     *  Purpose:  This class is used to represent a puzzle and  *
     *            its attributes.                               *
     *                                                          *
     ************************************************************/
    class Puzzle : IComparable
    {

        // data members

        private int difficulty;                     // holds difficulty, 0 for easy, 1 for medium, 2 for hard
        private int previouslyCompleted = 0;        // for sorting purposes, 1 means not completed, 0 means completed
        private int previouslySaved = 0;            // tells if puzzle has been saved before, 1 means it was saved before.
        private int cheated = 0;                    // 0 means haven't cheated, 1 means have cheated.

        private int[,] initial;                     // holds initial state of puzzle
        private int[,] solution;                    // holds solution state of puzzle
        private int[,] saved;                       // holds saved state of puzzle

        private int savedAttemptTime = 0;           // holds saved attempt time
        private int fastestAttemptTime = 0;         // holds fastest attempt time

        private int newSavedTime = 0;

        private string filePathName;                // holds pathname of the original puzzle file

        // constructor
        public Puzzle(string file)
        {
            FilePathName = file;
        }



        // properties (getters/setters)
        public int[,] Initial { get; set; }
        public int[,] Solution { get; set; }
        public int[,] Saved { get; set; }


        public int Difficulty { get; set; }
        public int PreviouslyCompleted { get; set; }
        public int PreviouslySaved { get; set; }
        public int Cheated { get; set; }
        public int SavedAttemptTime { get; set; }
        public int FastestAttemptTime { get; set; }
        public int NewSavedTime { get; set; }

        public string FilePathName { get; set; }




        /***********************************************************
        *                                                          *
        *  Method Name:  CompareTo                                 *
        *                                                          *
        *  Parameters: object obj                                  *
        *                                                          *
        *  Purpose:  This method provides an implementation for    *
        *            the IComparable interface so that Puzzle      *
        *            objects can be easily sorted and compared     *
        *                                                          *
        *  Return:  an int that is used to compare the Puzzle      *
        *           object that called the method with another     *
        *           Puzzle object.                                 *
        *                                                          *
        ************************************************************/
        public int CompareTo(object obj)
        {
            if (obj == null) throw new ArgumentNullException();   // checking for null values
            Puzzle puzzle = obj as Puzzle;   // typecasting obj as Puzzle and storing as Puzzle

            if (puzzle != null)
            {
                if(Difficulty.CompareTo(puzzle.Difficulty) == 0)
                {
                    if (puzzle.PreviouslyCompleted.CompareTo(PreviouslyCompleted) == 0)
                    //if (PreviouslyCompleted.CompareTo(puzzle.PreviouslyCompleted) == 0)
                    {

                        if (puzzle.PreviouslySaved.CompareTo(PreviouslySaved) == 0)
                        //if (PreviouslyCompleted.CompareTo(puzzle.PreviouslyCompleted) == 0)
                        {
                            return (FilePathName.CompareTo(puzzle.FilePathName));
                        }
                        else
                        {
                            return (puzzle.PreviouslySaved.CompareTo(PreviouslySaved));
                        }

                        //return (filePathName.CompareTo(puzzle.filePathName));
                    }
                    else
                    {
                        return (puzzle.PreviouslyCompleted.CompareTo(PreviouslyCompleted));
                        //return (PreviouslyCompleted.CompareTo(puzzle.PreviouslyCompleted));
                    }
                        
                } else
                    return (Difficulty.CompareTo(puzzle.Difficulty));     // primitive data types already have this Icomparable interface implemented
            }                                            // thus we can make use of CompareTo for uint data types instead of writing conditions.
            else
            {
                throw new ArgumentException("[Student]:CompareTo argument is not a Puzzle");
            }
        }
    }
}
