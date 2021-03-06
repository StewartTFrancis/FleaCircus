# FleaCircus (monte-carlo simulation)
## Task: Flea circus

A 30×30 grid of squares contains 900 fleas, initially one flea per square.
When a bell is rung, each flea jumps to an adjacent square at random (usually 4 possibilities, except for fleas on the edge of the grid or at the corners).

What is the expected number of unoccupied squares after 50 rings of the bell? Give your answer rounded to six decimal places.

Implementation plan:
- Implement single simulation
- Run multiple simulations and calculate average
- Run simulations in parallel
- Optimize for better speed, less memory allocations

To run:
Once you have the dotnet tools installed locally, clone this and run:
```
dotnet build
dotnet run
```

Output will look similar to:
`After 2038000 current average is 330.723406`
