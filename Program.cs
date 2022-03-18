using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FleaCircus
{
    class Program
    {
        static void Main(string[] args)
        {
            RunOnce();

            RunLooped();
        }
        
        // Implement a single run.
        static void RunOnce()
        {
            // Make it 30x30 w/ 1 flea each
            Simulation sim = new Simulation(30, 30, 1);
            // Convenience func to do multiple steps
            sim.Step(50);

            // Get the grid after 50 steps
            var grid = sim.currentGrid;
            var empties = 0;

            // Count the empties.
            foreach (var ele in grid)
            {
                if (ele == 0)
                    empties++;
            }

            Console.WriteLine(empties);
        }

        // Continuous running multithreaded 2k at a time
        // Keep running monte-carlo average
        static void RunLooped()
        {
            // There are trade offs in how we store these
            // and do the average. I've opted for most precise
            // Even though it does have a performance implication
            ulong simulationCount = 0;
            ulong totEmpty = 0;

            var simulationTasks = new List<Task<ulong>>();
            while(true)
            {
                // There are other (async/await) ways to do this
                // however there are tradeoffs in pushing async dependancies up
                // in a console app
                //
                // This is performant (2k takes about 1 second locally)
                // which is a trade off between time blocking the console
                // and providing quick updates
                for(var i = 0; i < 2000; i++)
                {
                    // Queue up tasks
                    simulationTasks.Add(Task.Run(() => {
                        Simulation sim = new Simulation(30, 30, 1);
                        sim.Step(50);

                        var grid = sim.currentGrid;
                        ulong empties = 0;

                        foreach (var ele in grid)
                        {
                            if (ele == 0)
                                empties++;
                        }

                        return empties;
                    }));
                }

                // Block until they finish
                Task.WhenAll(simulationTasks);

                // Up the sim count and add the total
                foreach (var task in simulationTasks)
                {
                    simulationCount++;
                    totEmpty += task.Result;
                }

                // Clear the queue
                simulationTasks.Clear();

                // This fails w/ an IOException if running in a non-terminal environment
                try {
                    Console.Clear();
                } catch (IOException)
                {}
                
                // Print current running monte-carlo average
                Console.WriteLine($"After {simulationCount} current average is {(decimal)totEmpty / (decimal)simulationCount:F6}");
            }
        }
    }
}
