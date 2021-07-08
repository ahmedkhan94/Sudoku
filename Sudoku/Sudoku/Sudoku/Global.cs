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

using System.Collections.Generic;

namespace Sudoku
{
    /************************************************************
     *                                                          *
     *  Class Name:  Global                                     *
     *                                                          *
     *  Purpose:  C# doesn't support global variables directly, *
     *            had to make a static class with static fields *
     *            to imitate global variables.                  *
     *                                                          *
     *  Note:     this class is never instantiated.             *
     ************************************************************/
    static class Global
    {
        // holds all the puzzles seperated by difficulty, sorted by whether they have been completed followed by saved or unsolved
        static List<SortedSet<Puzzle>> puzzlePool = new List<SortedSet<Puzzle>> { new SortedSet<Puzzle>(), new SortedSet<Puzzle>(), new SortedSet<Puzzle>() };

        public static List<SortedSet<Puzzle>> PuzzlePool { get { return puzzlePool; } set { } }

    }
}
