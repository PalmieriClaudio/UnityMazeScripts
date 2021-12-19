using System.Collections.Generic;
using UnityEngine;

public class GenerateMazesFromImage : MonoBehaviour
{
    public Texture2D sourceImage;
    public int wallThreshold = 100;
    public int wallHeight = 3;

    int _sourceWidth;
    int _sourceHeight;
    int sourceMipLevel = 0;

    Color32[] sourcePixelsAligned;
    List<int> sourcePixelsGreyscale;
    List<int[]> wallsList = new List<int[]> { };
    bool[] inWallStatus;
    void Start()
    {
        _sourceWidth = sourceImage.width;
        _sourceHeight = sourceImage.height;
        sourcePixelsAligned = sourceImage.GetPixels32(sourceMipLevel);
        sourcePixelsGreyscale = TurnGreyscale(sourcePixelsAligned);
        inWallStatus = new bool[sourcePixelsAligned.Length];
        FindWallsCoordinates();
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
        if (CheckIfPixelIsWall(i + 1)) return CheckWallRight(i + 1);
        return i;
    }
    /// <summary>
    /// Check if the wall extends downwards
    /// </summary>
    /// <param name="i">Index of the start of the wall</param>
    /// <returns>Returns null if there's no wall downwards, otherwise returns the index of the wall's end coordinate</returns>
    int CheckWallUp(int j)
    {
        if (j + _sourceWidth >= sourcePixelsGreyscale.Count) return j;
        inWallStatus[j] = true;
        if (CheckIfPixelIsWall(j + _sourceWidth)) return CheckWallUp(j + _sourceWidth);
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
}
