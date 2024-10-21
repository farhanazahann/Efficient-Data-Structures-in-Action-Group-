/*  Names: Farhana Zahan, Khushi Chauhan, Anubhav Mehandru
 *  Part 1:Point Me in the Right Direction                                               |w
 *  PointQuadTree                                                                        |
 *  ************************************************************************************ |
 *  This program demonstrates the implementation of a PointQuadTree class, which is used | 
|   to map d-dimensional points to an index representing a region or quadrant.           |
|   The MapToIndex method in the PointQuadTree class compares the coordinates of two     | 
|   dimensional points and maps the result to an index of branches (B).                  |
|   If a new point's coordinate is less than the current point's coordinate, the         |   
|   corresponding bit in the index is set to 1; otherwise, it's set to 0.                |
|    If the new point's coordinate is equal to the current point's coordinate, and the   |
|   index is 0, the corresponding bit is also set to 1 to differentiate the case.        |
|   The Main method tests the PointQuadTree class with 2D, 3D, and 4D dimensions using   |
|   sample points and outputs the index in both binary and decimal formats.              |
*****************************************************************************************/


//link:https://www.tutorialspoint.com/point-quadtrees-in-data-structure
// link:https://youtu.be/otpkYtipubI?si=BZFHfmXlSsY2bQ7q
using System;

class PointQuadTree
{
    private int dimensions;

    public PointQuadTree(int dimensions)
    {
        this.dimensions = dimensions;
    }

    // Function to compare d-dimensional points and map the result to an index of branches (B)
    public int MapToIndex(int[] currentPoint, int[] newPoint)
    {
        int index = 0;
        for (int i = 0; i < dimensions; i++)
        {
            // Compare coordinates of the new point with the current point
            if (newPoint[i] < currentPoint[i])
            {
                // If the new point's coordinate is less than the current point's coordinate,
                // set the corresponding bit in the index
                index |= 1 << (dimensions - 1 - i);
            }
            else if (newPoint[i] > currentPoint[i] || (newPoint[i] == currentPoint[i] && index == 0))
            {
                // If the new point's coordinate is greater than the current point's coordinate,
                // or if it is equal but the index is 0, set the corresponding bit in the index
                index &= ~(1 << (dimensions - 1 - i));
            }
            // If the new point's coordinate is equal to the current point's coordinate, no action is needed
        }
        return index;
    }
}

class MainClass
{
    public static void Main(string[] args)
    {
        // Testing 2D
        Test2D();

        // Testing 3D
        Test3D();

        // Testing 4D
        Test4D();
    }

    // Test method for 2D dimension
    public static void Test2D()
    {
        Console.WriteLine("Testing 2D:");

        // Example dimensions (2D)
        int dimensions = 2;

        // Initialize the point quadtree with the specified dimensions
        PointQuadTree quadTree = new PointQuadTree(dimensions);

        // Example current point's coordinates
        int[] currentPoint = { 35, 50 };

        // Example new point's coordinates
        int[] newPoint = { 15, 60 };

        // Map the result to an index of branches (B) in the current coordinates
        int index = quadTree.MapToIndex(currentPoint, newPoint);

        // Output the index as binary and decimal
        Console.WriteLine("Index (Binary): " + Convert.ToString(index, 2).PadLeft(dimensions, '0'));
        Console.WriteLine("Index (Decimal): " + index);
        Console.WriteLine();
    }

    // Test method for 3D dimension
    public static void Test3D()
    {
        Console.WriteLine("Testing 3D:");

        // Example dimensions (3D)
        int dimensions = 3;

        // Initialize the point quadtree with the specified dimensions
        PointQuadTree quadTree = new PointQuadTree(dimensions);

        // Example current point's coordinates
        int[] currentPoint = { 35, 50, 70 };

        // Example new point's coordinates
        int[] newPoint = { 15, 60, 80 };

        // Map the result to an index of branches (B) in the current coordinates
        int index = quadTree.MapToIndex(currentPoint, newPoint);

        // Output the index as binary and decimal
        Console.WriteLine("Index (Binary): " + Convert.ToString(index, 2).PadLeft(dimensions, '0'));
        Console.WriteLine("Index (Decimal): " + index);
        Console.WriteLine();
    }

    // Test method for 4D dimension
    public static void Test4D()
    {
        Console.WriteLine("Testing 4D:");

        // Example dimensions (4D)
        int dimensions = 4;

        // Initialize the point quadtree with the specified dimensions
        PointQuadTree quadTree = new PointQuadTree(dimensions);

        // Example current point's coordinates
        int[] currentPoint = { 35, 50, 70, 90 };

        // Example new point's coordinates
        int[] newPoint = { 15, 60, 80, 100 };

        // Map the result to an index of branches (B) in the current coordinates
        int index = quadTree.MapToIndex(currentPoint, newPoint);

        // Output the index as binary and decimal
        Console.WriteLine("Index (Binary): " + Convert.ToString(index, 2).PadLeft(dimensions, '0'));
        Console.WriteLine("Index (Decimal): " + index);
        Console.WriteLine();
    }
}