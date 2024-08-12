using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlyphaeScripts
{
    /// <summary>
    /// Generates a perfect maze using IterativeBacktracker.
    /// Adapted from <see href="https://github.com/n-c0de-r/TaurosTraps">one of my own older projects</see>.
    /// As inspired by the visual demo of <see href="https://www.jamisbuck.org/mazes/">Jamis Buck</see>.
    /// </summary>
    public class MazeGenerator : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Stack for each of the parallelly running backtrackers
        /// </summary>
        private Stack<Cell>[] stacks;

        /// <summary>
        /// The stack of cells for iterative backtracker.
        /// </summary>
        private Stack<Cell> cells;

        /// <summary>
        /// Structs holding information on directions.
        /// </summary>
        private Direction[] directions;

        /// <summary>
        /// The maze grid, where 0 means no path, any other (1-15) a connection.
        /// </summary>
        private int[,] maze;

        /// <summary>
        /// Size of the quadratic maze.
        /// </summary>
        private int size;

        #endregion


        #region Methods

        /// <summary>
        /// Generates a new maze.
        /// Using the Iterative Backtracker method.
        /// <param name="newSize">The tile count of one maze size.</param>
        /// </summary>
        public int[,] GenerateMaze(int newSize = 5)
        {
            size = newSize;

            ResetAll();

            StartCoroutine(IterativeBacktracker());

            PaintMap();

            return maze;
        }

        /// <summary>
        /// Iterates over the given data and lays tiles accordingly.
        /// </summary>
        public void PaintMap()
        {
            // Visualize the data as numbers. Uncomment if needed
            String s = "Maze generated\n";
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (maze[x, y] < 10)
                    {
                        s += "0" + maze[x, y] + " ";
                    }
                    else
                    {
                        s += maze[x, y] + " ";
                    }
                }
                s += "\n";
            }
            //Debug.Log(s);
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Resets everything prior to generation.
        /// </summary>
        private void ResetAll()
        {
            directions = new Direction[] {
            new(1, 0, -1),    // North
            new(2, 1, 0),     // East
            new(4, 0, 1),     // South
            new(8, -1, 0),    // West
        };

            maze = new int[size, size];
            InitializeStack();
        }

        /// <summary>
        /// Clears all stacks and initializes first cell.
        /// </summary>
        private void InitializeStack()
        {
            cells = new Stack<Cell>();
            cells.Push(new Cell(Random.Range(0, size >> 1), Random.Range(0, size >> 1)));

            int halfX = size >> 1;    // Half the size, for starting positions of
            int halfY = size >> 1;    // the four starting cells in each quadrant

            // Starting Cells for each quadrant for the parallelly running backtrackers
            Cell[] starters = new Cell[4] {
            new(Random.Range(0, halfX),    Random.Range(0, halfY)),        // Up left 
            new(Random.Range(halfX, size),Random.Range(0, halfY)),        // Up right
            new(Random.Range(0, halfX),    Random.Range(halfY, size)),   // Down left
            new(Random.Range(halfX, size),Random.Range(halfY, size)),   // Down right
        };

            stacks = new Stack<Cell>[4] {
            new(),
            new(),
            new(),
            new()
        };

            for (int i = 0; i < stacks.Length; i++)
            {
                stacks[i].Push(starters[i]);
            }
        }

        /// <summary>
        /// The actual implementation of the generating algorithm.
        /// Using the Iterative Backtracking approach.
        /// </summary>
        /// <returns>An enumerator.</returns>
        private IEnumerator IterativeBacktracker()
        {
            while (cells.Count > 0)
            {
                Cell current = cells.Pop();

                ShuffleDirections(directions);

                foreach (Direction dir in directions)
                {
                    int newX = current.x + dir.x;
                    int newY = current.y + dir.y;

                    if (IsValidMove(newX, newY))
                    {
                        maze[current.x, current.y] |= dir.value;
                        maze[newX, newY] |= dir.opposite;

                        cells.Push(new Cell(newX, newY));
                    }
                }
            }
            yield return null;
        }

        /// <summary>
        /// Check if a move to the given coordinates is valid (within the bounds of the maze and not already visited)
        /// </summary>
        /// <param name="x">The X coordinate of the cell to check.</param>
        /// <param name="y">The Y coordinate of the cell to check.</param>
        /// <returns>True or false if the cell is valid or not.</returns>
        private bool IsValidMove(int x, int y)
        {
            return x >= 0 && x < size && y >= 0 && y < size && maze[x, y] == 0;
        }

        /// <summary>
        /// Shuffles the array of directions.
        /// </summary>
        /// <param name="original">The array to shuffle.</param>
        private void ShuffleDirections(Direction[] original)
        {
            int n = original.Length;
            while (n > 1)
            {
                int k = Random.Range(0, n--);
                (original[k], original[n]) = (original[n], original[k]);
            }
        }

        #endregion


        #region Structs

        /// <summary>
        /// Represents Cells in a maze
        /// </summary>
        public struct Cell
        {
            /// <summary>
            /// X coordinate, represents the cell to visit next.
            /// </summary>
            public int x;

            /// <summary>
            /// Y coordinate, represents the cell to visit next.
            /// </summary>
            public int y;

            /// <summary>
            /// Consturctor of a Cell struct.
            /// </summary>
            /// <param name="newX">New X coordinate to set.</param>
            /// <param name="newY">New Y coordinate to set.</param>
            public Cell(int newX, int newY)
            {
                x = newX;
                y = newY;
            }
        }

        /// <summary>
        /// Struct holding values relevant for carving directions in a maze.
        /// </summary>
        public struct Direction
        {
            /// <summary>
            /// Number representation of a direction, to use bitwise operations on.
            /// </summary>
            public int value;

            /// <summary>
            /// X coordinate, represents the cell to visit next.
            /// </summary>
            public int x;

            /// <summary>
            /// Y coordinate, represents the cell to visit next.
            /// </summary>
            public int y;

            /// <summary>
            /// The opposite value of the original, calculated automatically.
            /// </summary>
            public int opposite;

            /// <summary>
            /// Consturctor of a Direction struct.
            /// </summary>
            /// <param name="newValue">New value to set. </param>
            /// <param name="newX">New X coordinate to set.</param>
            /// <param name="newY">New Y coordinate to set.</param>
            public Direction(int newValue, int newX, int newY)
            {
                value = newValue;
                x = newX;
                y = newY;
                opposite = (value << 2) % 15; // Automatically calculates the opposite.
            }
        }

        #endregion
    }
}
