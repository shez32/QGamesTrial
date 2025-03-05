// Given that you have a 2 dimensional array of height values (say 10x10) that represent a height field. 
// Assume that the distance between each height point in X and Y is 1 
// and that from the grid we form a triangulated mesh such that 
// for each set of 4 grid points A=(x,y), B=(x+1,y) C=(x+1,y+1), and D=(x,y+1) 
// there are two triangles ABC and ACD (so they are seperated by the line x=y in the local coordinate system of the grid). 
// Explain how you would find the *exact* height of an arbitrary point x,y on this mesh.


/*
* based on the info above the height map must look something like this:
 (0,2) ------ (1,2) ------ (2,2)
    |           |           |
    |           |           |
 (0,1) ------ (1,1) ------ (2,1)
    |           |           |
    |           |           |
 (0,0) ------ (1,0) ------ (2,0)


 Triangles ACD and ABC look something like this:

 D(x,y+1)        C(x+1,y+1)
   ● ------------ ●
   |    ╲       /  |
   |     ╲    /    |
   |      ╲ /      |
   |       ●       |
   |      / \      |
   |     /   ╲     |
   |    /     ╲    |
   ● ------------ ●
A(x,y)           B(x+1,y)


where the line AC (in this case a straight line x = y divides the square into 2 triangles)

Given an arbitrary point (px, py) we can find the exact height by:

 D(x,y+1)        C(x+1,y+1)
   ● ------------ ●
   |    ╲       /  |
   |     ╲    /    |
   |   ● (px, py)  |
   |       ●       |
   |      / \      |
   |     /   ╲     |
   |    /     ╲    |
   ● ------------ ●
A(x,y)           B(x+1,y)


1) we must first identify which triangle our arbitrary point falls into
    a) we can find the bottom left corner A(x, y) by simplying flooring the input (px, py)
    b) the other three corners can be calculated automatically with simple arithmetics
    c) we can calculate which triangle the point lies in by the formula { (px - Ax) >= (py - Ay) }

2) Since the triangles are formed exactly from the grid points, the height at any point inside them can be determined precisely using barycentric coordinates
    a) if (px,py) is in triangle ABC, we can express it as:
        H(x, y) = (Weight A * Height A) + (Weight B * Height B) + (Weight C * Height C)
        where height is the value at the provided co-ordinate (i.e Height A would be int[x][y])
              weight is the area ratio
    b) else if (px, py) is in triangle ACD, we can express it as:
        H(x, y) = (Weight A * Height A) + (Weight C * Height C) + (Weight D * Height D)

*/


// Please implement the method from the previous question in in C++ (or C# if you are not familiar with C++)

#include <iostream>
#include <vector>

using namespace std;

// Function to compute the exact height at (x, y) given a 2D height map
// Thanks to this tutorial..............https://www.youtube.com/watch?v=6E2zjfzMs7c

double getExactHeight(const vector<vector<double>>& heightMap, double x, double y) {
    // Get the integer grid cell that contains (x, y)
    // We are basically flooring the value to get the bottom corner closest to the pont
    // (2.3, 4.7) would become (2,4)
    int Ax = static_cast<int>(x);
    int Ay = static_cast<int>(y);

    // Ensure we don't go out of bounds
    if (Ax < 0 || Ay < 0 || Ax >= heightMap.size() - 1 || Ay >= heightMap[0].size() - 1) {
        throw std::out_of_range("Point is outside the height map bounds.");
    }

    // Define the four corners of the grid square
    double Ha = heightMap[Ax][Ay];          // A (Bottom-left)
    double Hb = heightMap[Ax + 1][Ay];      // B (Bottom-right)
    double Hc = heightMap[Ax + 1][Ay + 1];  // C (Top-right)
    double Hd = heightMap[Ax][Ay + 1];      // D (Top-left)

    // Local coordinates inside the grid cell
    double dx = x - Ax;
    double dy = y - Ay;

    // Determine which triangle (ABC or ACD)
    if (dx >= dy) {
        // Triangle ABC
        double lambdaA = 1 - dx;       // Weight for A
        double lambdaB = dx - dy;      // Weight for B
        double lambdaC = dy;           // Weight for C
        return lambdaA * Ha + lambdaB * Hb + lambdaC * Hc;
    }
    else {
        // Triangle ACD
        double lambdaA = 1 - dy;       // Weight for A
        double lambdaC = dx;           // Weight for C
        double lambdaD = dy - dx;      // Weight for D
        return lambdaA * Ha + lambdaC * Hc + lambdaD * Hd;
    }
}

int main()
{
    // Example height map (5x5 grid)
    std::vector<std::vector<double>> heightMap = {
        {1, 2, 3, 4, 5},
        {2, 3, 4, 5, 6},
        {3, 4, 5, 6, 7},
        {4, 5, 6, 7, 8},
        {5, 6, 7, 8, 9}
    };

    // Example arbitrary point
    double x = 1.3, y = 2.7;
    double exactHeight = getExactHeight(heightMap, x, y);

    std::cout << "Exact height at (" << x << ", " << y << ") = " << exactHeight << std::endl;
    return 0;
}
