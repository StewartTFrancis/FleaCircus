using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Simulation
{
    // To reduce memory allocations we can just reuse
    // an active and working grid and swap between them
    private int[,] grid;
    private int [,] workingGrid;

    private List<Directions> validDirections = new List<Directions>();

    // For our purposes psuedoranom should be fine
    private Random random = new Random();

    // Provide a read-only accessor for the grid
    // And clone it to avoid them being able to
    // change anything accidentally
    public int[,] currentGrid {
        get {
            return grid.Clone() as int[,];
        }
    }

    public Simulation(int width, int height, int fleasPerSquare)
    {
        // I know we typically think in X, Y coordinates
        // But the way the array is set the first dimension is really Y
        grid = new int[height, width];
        workingGrid = new int[height, width];

        // initialize grid
        for (var y=0; y < height; y++)
            for (var x=0; x < width; x++)
                grid[y, x] = fleasPerSquare;
    }

    private enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    public void Step(int steps)
    {

        // Just added an output of grid state per step to debug.

        // System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // sb.AppendLine($" --- Step 0 --- ");
        // for(var y = 0; y < grid.GetLength(0); y++)
        // {
        //     for(var x = 0; x < grid.GetLength(1);x++)
        //     {
        //         sb.Append($"{grid[y, x]}");
        //     }
        //     sb.Append("\n");
        // }

        for (var i = 0; i < steps; i++)
        {
            Step();

            // This might not be super obvious, but we're printing the post-step
            // grid and working grid side by side

            // sb.AppendLine($" --- Step {i + 1} --- ");
            // for(var y = 0; y < grid.GetLength(0); y++)
            // {
            //     for(var x = 0; x < grid.GetLength(1); x++)
            //     {
            //         sb.Append($"{grid[y, x]}");
            //     }

            //         sb.Append("\t");

            //     for(var x = 0; x < workingGrid.GetLength(1);x++)
            //     {
            //         sb.Append($"{workingGrid[y, x]}");
            //     }
            //     sb.Append("\n");
            // }
        }

        // This should print out to the current working directory
        // System.IO.File.WriteAllText("output.txt", sb.ToString());
    }
    public void Step()
    {
        // For our purposes it'll always be 30, but just in case we've init'd with a different size.
        for (var y=0; y < grid.GetLength(0); y++)
        {
            for (var x=0; x < grid.GetLength(1); x++)
            {
                // Clear our current valid directions.
                // Technically the Y values are still good
                // But it's actually slightly less performant
                // To iterate the list and remove than just recreate
                validDirections.Clear();

                // Check our position and find what directions are valid
                if (y > 0)
                    validDirections.Add(Directions.Up);
            
                if (y < grid.GetLength(0) - 1)
                    validDirections.Add(Directions.Down);

                if (x > 0)
                    validDirections.Add(Directions.Left);
            
                if (x < grid.GetLength(1) - 1)
                    validDirections.Add(Directions.Right);

                // For however many fleas are on this spot
                for (var i=0; i < grid[y, x]; i++)
                {
                    // In the event we want true random (at a performance penalty)
                    // We can use the RandomNumberGenerator
                    // Both are from 0 - Count (exclusive), which works for us using 0-based indexing 
                    var rnd = random.Next(validDirections.Count); // RandomNumberGenerator.GetInt32(validDirections.Count);

                    // Move in the direction picked
                    // Always set on the workingGrid (which should reset to 0 between steps)
                    // That way we don't double count fleas that jump right/down
                    switch (validDirections[rnd])
                    {
                        case Directions.Up:
                            workingGrid[y-1, x]++;
                            break;
                        case Directions.Down:
                            workingGrid[y+1, x]++;
                            break;
                        case Directions.Left:
                            workingGrid[y, x-1]++;
                            break;
                        case Directions.Right:
                            workingGrid[y, x+1]++;
                            break;
                    }
                }

                // After we've finished our jumps clear this spot
                // So we can use it as our next working grid
                grid[y, x] = 0;
            }
        }
        
        // Alright, I'd never do this in production
        // But we're just swapping the current good grid 
        // and working grid without an intermediate temp var
        (grid, workingGrid) = (workingGrid, grid);
    }
}