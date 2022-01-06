using System;
using System.Collections.Generic;
using System.Linq;

public class RandomMazeGenerator
{
    Random rnd = new Random();
    bool[] visited;
    List<string> directions = new List<string> { "N", "E", "S", "W" };
    public List<int> GenerateNewMaze(int width)
    {
        Maze maze = new Maze
        {
            Width = width,
            Grid = GenerateGrid(width)
        };
        HuntAndKill(maze);
        return ReturnMaze(maze);
    }
    List<Cell> GenerateGrid(int width)
    {
        List<Cell> grid = new List<Cell>();
        for (int i = 0; i < (width * width); i++)
        {
            grid.Add(new Cell { TopWall = true, RightWall = true, BottomWall = true, LeftWall = true });
        }

        return grid;
    }
    void HuntAndKill(Maze grid, int? current = null) //Implementation of the Hunt and Kill maze generation algorithm
    {
        Console.WriteLine(current);
        if (current == -1) return;
        if (current is null)
        {
            visited = new bool[grid.Grid.Count];
            current = rnd.Next(0, grid.Grid.Count);
            visited[(int)current] = true;
        }
        Kill((int)current, grid);
        HuntAndKill(grid, Hunt(grid));
    }
    void Kill(int current, Maze grid)
    {
        visited[current] = true;
        var randomizedDirections = directions.OrderBy(d => rnd.Next()).ToList();
        foreach (var direction in randomizedDirections)
        {
            int neighbour = LocateNeighbour(current, direction, grid);
            if (CheckNeighbour(current, neighbour, grid.Width))
            {
                //Set neighbour as visited, connect current and neighbour, set neighbour as current and kill neighbour
                visited[neighbour] = true;
                ConnectNeighbours(current, neighbour, grid);
                Kill(neighbour, grid);
                break;
            }
        }
    }
    int Hunt(Maze maze)
    {
        for (int i = 0; i < maze.Grid.Count; i++)
        {
            if (visited[i] == false && HasVisitedNeighbour(i, maze.Width))
            {
                ConnectToVisitedNeighbour(i, maze);
                return i;
            }
        }
        return -1;
    }
    bool HasVisitedNeighbour(int cell, int width)
    {
        return (CellInBounds(cell + 1) ? visited[cell + 1] : false) || (CellInBounds(cell - 1) ? visited[cell - 1] : false) ||
            (CellInBounds(cell + width) ? visited[cell + width] : false) || (CellInBounds(cell - width) ? visited[cell - width] : false);
    }
    void ConnectToVisitedNeighbour(int cell, Maze grid)
    {
        var randomizedDirections = directions.OrderBy(d => rnd.Next()).ToList();
        foreach (var direction in randomizedDirections)
        {
            int neighbour = LocateNeighbour(cell, direction, grid);
            if (CellInBounds(neighbour) && visited[neighbour])
                if (!(cell % grid.Width == grid.Width - 1 && neighbour % grid.Width == 0) && !(cell % grid.Width == 0 && neighbour % grid.Width == grid.Width - 1))
                {
                    ConnectNeighbours(cell, neighbour, grid);
                    break;
                }
        }
    }
    int LocateNeighbour(int current, string direction, Maze maze)
    {
        switch (direction)
        {
            case "N":
                if (current + maze.Width <= (maze.Width * maze.Width))
                    return current + maze.Width;
                break;
            case "E":
                if (current + 1 <= (maze.Width * maze.Width))
                    return current + 1;
                break;
            case "S":
                if (current - maze.Width >= 0)
                    return current - maze.Width;
                break;
            case "W":
                if (current - 1 >= 0)
                    return current - 1;
                break;
            default:
                break;
        }
        return -1;
    }
    void ConnectNeighbours(int current, int neighbour, Maze maze)
    {
        var difference = current - neighbour;
        if (difference == -1)
        {
            maze.Grid[current].RightWall = false;
            maze.Grid[neighbour].LeftWall = false;
        }
        else if (difference == 1)
        {
            maze.Grid[current].LeftWall = false;
            maze.Grid[neighbour].RightWall = false;
        }
        else if (difference == -maze.Width)
        {
            maze.Grid[current].TopWall = false;
            maze.Grid[neighbour].BottomWall = false;
        }
        else
        {
            maze.Grid[current].BottomWall = false;
            maze.Grid[neighbour].TopWall = false;
        }
    }
    bool CheckNeighbour(int current, int neighbour, int width)
    {

        if (CellInBounds(neighbour) && !visited[neighbour])
            if (!(current % width == width - 1 && neighbour % width == 0) && !(current % width == 0 && neighbour % width == width - 1))
                return true;
        return false;
    }
    bool CellInBounds(int cell)
    {
        if (cell >= 0 && cell < visited.Count()) return true;
        return false;
    }
    List<int> ReturnMaze(Maze maze)
    {
        var finishedMaze = new List<int> { };
        for (int i = 0; i < maze.Width; i++)
        {
            for (int j = 0; j < maze.Width; j++) //bottom
            {
                if (maze.Grid[i * maze.Width + j].BottomWall)
                {
                    finishedMaze.Add(0);
                    finishedMaze.Add(0);
                    finishedMaze.Add(0);
                }
                else
                {
                    if (maze.Grid[i * maze.Width + j].LeftWall || maze.Grid[i * maze.Width + j - 1 - maze.Width].TopWall || maze.Grid[i * maze.Width + j - maze.Width].RightWall) // While this might seem weird, I'm just checking the other walls that connect to the corner
                    {
                        finishedMaze.Add(0);
                        finishedMaze.Add(254);
                        finishedMaze.Add(254);
                    }
                    else
                    {
                        finishedMaze.Add(254);
                        finishedMaze.Add(254);
                        finishedMaze.Add(254);
                    }
                }
            }
            if (maze.Grid[i * maze.Width + maze.Width - 1].RightWall)
            {
                finishedMaze.Add(0);
            }
            else
            {
                finishedMaze.Add(255);
            }
            for (int r = 0; r < 2; r++)
            {
                for (int j = 0; j < maze.Width; j++) //left
                {
                    if (maze.Grid[i * maze.Width + j].LeftWall)
                    {
                        finishedMaze.Add(0);
                        finishedMaze.Add(254);
                        finishedMaze.Add(254);
                    }
                    else
                    {
                        finishedMaze.Add(254);
                        finishedMaze.Add(254);
                        finishedMaze.Add(254);
                    }
                }
                if (maze.Grid[i * maze.Width + maze.Width - 1].RightWall)
                {
                    finishedMaze.Add(0);
                }
                else
                {
                    finishedMaze.Add(255);
                }
            }
        }
        for (int i = 0; i < maze.Width; i++)
        {
            finishedMaze.Add(0);
            finishedMaze.Add(0);
            finishedMaze.Add(0);
        }
        finishedMaze.Add(0);
        return finishedMaze;
    }
}
public class Cell
{
    public bool TopWall { get; set; }
    public bool LeftWall { get; set; }
    public bool BottomWall { get; set; }
    public bool RightWall { get; set; }
}
public class Maze
{
    public int Width { get; set; }
    public List<Cell> Grid { get; set; }
}