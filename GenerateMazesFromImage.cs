using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMazesFromImage : MonoBehaviour
{
    public Texture2D sourceImage;
    public int wallThreshold = 100;
    public int wallHeight = 3;
    public GameObject wallPrefab;
    public GameObject groundPrefab;
    public int labirintWidth = 30;
    GameObject wall;

    int _sourceWidth;
    int _sourceHeight;
    int sourceMipLevel = 0;

    Color32[] sourcePixelsAligned;
    List<int> sourcePixelsGreyscale;
    List<int[]> wallsList = new List<int[]> { };
    bool[] inWallStatus;
    void Start()
    {
        try
        {
            _sourceWidth = sourceImage.width;
            _sourceHeight = sourceImage.height;
            sourcePixelsAligned = sourceImage.GetPixels32(sourceMipLevel);
            sourcePixelsGreyscale = TurnGreyscale(sourcePixelsAligned);
            inWallStatus = new bool[sourcePixelsGreyscale.Count];
        }
        catch (Exception)
        {
            RandomMazeGenerator generator = new RandomMazeGenerator();
            _sourceWidth = 3 * labirintWidth + 1;
            _sourceHeight = 3 * labirintWidth + 1;
            sourcePixelsGreyscale = generator.GenerateNewMaze(labirintWidth);
            inWallStatus = new bool[sourcePixelsGreyscale.Count];
        }
        FindWallsCoordinates();
        InstantiateMaze();
        InstantiateGround();
    }
    void FindWallsCoordinates()
    {
        for (int i = 0; i < sourcePixelsGreyscale.Count; i++)
        {
            if (CheckIfPixelIsWall(i) && PixelNotInWall(i))
            {
                int tempCoordinate = CheckWallRight(i);
                if (i == tempCoordinate) tempCoordinate = CheckWallUp(i);
                wallsList.Add(new int[2] { i, tempCoordinate });
            }
        }
    }
    bool PixelNotInWall(int i)
        => !inWallStatus[i];
    /// <summary>
    /// Checks if the pixel value is a wall based on the deifined wallThreshold
    /// </summary>
    /// <returns>returns true if the pixel is a wall, false otherwise</returns>
    bool CheckIfPixelIsWall(int i)
        => (sourcePixelsGreyscale[i] <= wallThreshold);
    /// <summary>
    /// Check if the wall extends forewards
    /// </summary>
    /// <param name="i">Index of the start of the wall</param>
    /// <returns>Returns null if there's no wall to the right, otherwise returns the index of the wall's end coordinate</returns>
    int CheckWallRight(int i)
    {
        inWallStatus[i] = true;
        if (i % _sourceWidth == _sourceWidth - 1) return i;
        if (CheckIfPixelIsWall(i + 1) && PixelNotInWall(i + 1)) return CheckWallRight(i + 1);
        return i;
    }
    /// <summary>
    /// Check if the wall extends upwards
    /// </summary>
    /// <param name="i">Index of the start of the wall</param>
    /// <returns>Returns null if there's no wall upwards, otherwise returns the index of the wall's end coordinate</returns>
    int CheckWallUp(int j)
    {
        if (j + _sourceWidth >= sourcePixelsGreyscale.Count) return j;
        inWallStatus[j] = true;
        if (CheckIfPixelIsWall(j + _sourceWidth) && PixelNotInWall(j + _sourceWidth)) return CheckWallUp(j + _sourceWidth);
        return j;
    }
    /// <summary>
    /// Converts a pixel space in greyscale ignoring alpha
    /// </summary>
    /// <param name="inputArray">Pixel space to convert as a Color32[]</param>
    /// <returns>Grayscaled pixel space</returns>
    List<int> TurnGreyscale(Color32[] inputArray)
    {
        List<int> _greyscaleList = new List<int> { };
        foreach (var colouredPixel in inputArray)
        {
            var grayscalePixel = Mathf.FloorToInt((colouredPixel.r + colouredPixel.b + colouredPixel.g) / 3);
            _greyscaleList.Add(grayscalePixel);
        }
        return _greyscaleList;
    }
    void InstantiateMaze()
    {
        foreach (var wall in wallsList)
        {
            var wallStart = new Vector2(wall[0] % _sourceWidth, (wall[0] - wall[0] % _sourceWidth) / _sourceWidth);
            var wallEnd = new Vector2(wall[1] % _sourceWidth, (wall[1] - wall[1] % _sourceWidth) / _sourceWidth);
            InstantiateWall(wallStart, wallEnd);
        }
    }
    void InstantiateWall(Vector2 wallStart, Vector2 wallEnd)
    {
        wall = Instantiate(wallPrefab, new Vector3(wallStart.x, 0, wallStart.y), Quaternion.identity);
        var wallLength = Vector2.Distance(wallStart, wallEnd) + 1;
        wall.transform.position = new Vector3((wallStart.x + wallEnd.x) / 2, (wall.transform.position.y + wallHeight / 2) + .5f, (wallStart.y + wallEnd.y) / 2);
        Quaternion newRotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(wallStart.y - wallEnd.y, wallStart.x - wallEnd.x) * Mathf.Rad2Deg, 0));
        wall.transform.localScale = new Vector3(wallLength, wallHeight, 1);
        wall.transform.rotation = newRotation;
    }
    void InstantiateGround()
    {
        GameObject ground = Instantiate(groundPrefab, new Vector3((_sourceWidth / 2) - .5f, -.5f, (_sourceHeight / 2) - .5f), Quaternion.identity);
        ground.transform.localScale = new Vector3(_sourceWidth, ground.transform.localScale.y, _sourceHeight);
    }
}
