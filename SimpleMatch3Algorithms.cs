using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SimpleMatch3Algorithms
{
    private static int minNeighbourCountForMatch = 2;

    public enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }

    /// <summary>
    /// This method checks if any match exists when swapping items.
    /// </summary>
    /// <param name="board">the board reference to make moves.</param>
    /// <param name="sourceIndex">source index.</param>
    /// <param name="destinationIndex">destination index.</param>
    public static bool MatchExists(Board board, int sourceIndex, int destinationIndex)
    {
        int destinationElement = board.Grid[sourceIndex];

        board.Grid[sourceIndex] = board.Grid[destinationIndex];
        board.Grid[destinationIndex] = destinationElement;

        bool returnvalue = CheckMatch(board, sourceIndex) || CheckMatch(board, destinationIndex);

        destinationElement = board.Grid[sourceIndex];

        board.Grid[sourceIndex] = board.Grid[destinationIndex];
        board.Grid[destinationIndex] = destinationElement;

        return returnvalue;
    }


    /// <summary>
    /// This method checks if any match occurred after swapped items.
    /// </summary>
    /// <param name="board">the board reference to make moves.</param>
    /// <param name="index">the index that the move made on.</param>
    private static bool CheckMatch(Board board, int index)
    {
        int horizontalCount = GetTotal(board, index, Direction.Right) + GetTotal(board, index, Direction.Left);
        int verticalCount = GetTotal(board, index, Direction.Up) + GetTotal(board, index, Direction.Down);
        return (horizontalCount >= minNeighbourCountForMatch) || (verticalCount >= minNeighbourCountForMatch);
    }

    /// <summary>
    /// This method returns the total items that are similar to moved item.
    /// </summary>
    /// <param name="board">the board reference to make moves.</param>
    /// <param name="index">the index that the move made on.</param>
    /// <param name="direction">the direction to look forward if there is any similar item.</param>
    private static int GetTotal(Board board, int index, Direction direction)
    {
        int nextIndex = index;
        int currentNeighbourCount = 0;

        while (true)
        {
            nextIndex = GetIndexByDirection(board, nextIndex, direction);

            if (nextIndex == -1)
            {
                break;
            }

            if (board.Grid[index] != board.Grid[nextIndex])
            {
                break;
            }

            currentNeighbourCount++;
        }

        return currentNeighbourCount;
    }


    /// <summary>
    /// This method is called for getting index by the direction.
    /// It returns -1 if it is out of boundaries.
    /// </summary>
    /// <param name="board">the board reference to make moves.</param>
    /// <param name="currentIndex">the index that the move made on.</param>
    /// <param name="direction">the direction to look forward.</param>
    private static int GetIndexByDirection(Board board, int currentIndex, Direction direction)
    {
        int width = board.Width;
        int height = board.Height;
        int nextIndex;

        switch (direction)
        {
            case Direction.Right:

                nextIndex = currentIndex + 1;

                if ((nextIndex % width != 0) & (nextIndex < width * height))
                {
                    return nextIndex;
                }

                break;

            case Direction.Left:

                nextIndex = currentIndex - 1;

                if ((nextIndex > -1) & (currentIndex % width != 0))
                {
                    return nextIndex;
                }

                break;

            case Direction.Up:

                nextIndex = currentIndex - width;

                if (nextIndex > -1)
                {
                    return nextIndex;
                }
                break;

            case Direction.Down:

                nextIndex = currentIndex + width;

                if (nextIndex < width * height)
                {
                    return nextIndex;
                }

                break;

            default:
                break;
        }

        return -1;
    }

    /// <summary>
    /// This method returns all the possible matches that can be made on the current board.
    /// </summary>
    /// <param name="board">the board reference to check for possible matches.</param>
    public static List<Tuple<int, int>> GetAllPossibleMatches(Board board)
    {
        int gridCount = board.Width * board.Height;

        List<Tuple<int, int>> allPossibleMoves = new();

        for (int i = 0; i < gridCount; i++)
        {
            for (int j = i; j < gridCount; j++)
            {
                if (IsValidMove(board, i, j))
                {
                    if (MatchExists(board, i, j))
                    {
                        allPossibleMoves.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
        }

        return allPossibleMoves;
    }


    /// <summary>
    /// This method checks if the move is valid or not.
    /// </summary>
    /// <param name="board">the board reference that the move made on.</param>
    /// <param name="sourceIndex">the index that the move made from.</param>
    /// <param name="destinationIndex">the index that the move made to.</param>
    private static bool IsValidMove(Board board, int sourceIndex, int destinationIndex)
    {
        for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
        {
            if (GetIndexByDirection(board, sourceIndex, (Direction)i) == destinationIndex)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// This method shuffles the current board.
    /// </summary>
    /// <param name="board">the board reference that shall be shuffled.</param>
    public static void Shuffle(Board board)
    {
        int gridCount = board.Width * board.Height;
        List<int> gridValues;
        Board shuffledBoard;

        int currentIndex;
        int randomElement;
        int numberOfTries;
        int maxNumberOfTries = 30;

        do
        {
            gridValues = new List<int>(board.Grid);
            shuffledBoard = new Board(board.Width, board.Height, new int[gridCount]);

            numberOfTries = 0;
            currentIndex = 0;

            while (currentIndex < gridCount)
            {
                randomElement = Random.Range(0, gridValues.Count);

                shuffledBoard.Grid[currentIndex] = gridValues[randomElement];

                if (!CheckMatch(shuffledBoard, currentIndex))
                {
                    gridValues.RemoveAt(randomElement);
                    currentIndex++;
                    numberOfTries = 0;
                }
                else
                {
                    numberOfTries++;
                }

                if (numberOfTries == maxNumberOfTries)
                {
                    break;
                }
            }

        } while (!AnyPossibleMatch(shuffledBoard) || numberOfTries == maxNumberOfTries || CheckArrayEquality(shuffledBoard.Grid, board.Grid));
      
        for (int i = 0; i < board.Grid.Length; i++)
        {
            board.Grid[i] = shuffledBoard.Grid[i];
        }                
        
    }


    /// <summary>
    /// This method returns true if there is any possible match, vice versa.
    /// </summary>
    /// <param name="board">the board reference to check for possible matches.</param>
    private static bool AnyPossibleMatch(Board board)
    {
        int gridCount = board.Grid.Length;

        for (int i = 0; i < gridCount; i++)
        {
            for (int j = i; j < gridCount; j++)
            {
                if (IsValidMove(board, i, j))
                {
                    if (MatchExists(board, i, j))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    /// <summary>
    /// This method checks if there is a match made in the current board.
    /// </summary>
    /// <param name="board">the board reference that shall be checked.</param>
    public static bool IsAnyMatchExistsInBoard(Board board)
    {
        for (int i = 0; i < board.Height * board.Width; i++)
        {
            if(CheckMatch(board, i))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// This method checks if the two arrays are similar or not.
    /// </summary>
    /// <param name="array1">the first array.</param>
    /// <param name="array1">the second array.</param>
    private static bool CheckArrayEquality(int[] array1, int[] array2)
    {
        if(array1 == null || array2 == null)
        {
            return false;
        }

        if(array1.Length != array2.Length)
        {
            return false;
        }

        for(int i = 0; i < array1.Length; i++)
        {
            if(array1[i] != array2[i])
            {
                return false;
            }
        }

        return true;
    }
}
