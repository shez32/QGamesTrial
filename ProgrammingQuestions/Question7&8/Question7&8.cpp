// Please think of an algorithm that given 4 arbitrary points produces a smooth curve that connects all of them
// Then write a function that takes 5 parameters (4 points and a 'time' parameter between 0 and 1) and returns the point on the curve at an arbitrary 'time'

// Now write a function that given an arbitrary number of points smoothly interpolates between them at a given input 'time' parameter between 0 and 1, 
// making use of the function/functions you wrote in previous question if appropriate.
// Some form of visual output is a plus.

#include <iostream>
#include <vector>
#include <cmath>

using namespace std;

// Represents a 2D point in space.
struct Point {
    double x, y;
};

// I am using catmull-rom spline for this implementation instead of brezier curves
// https://qroph.github.io/2018/07/30/smooth-paths-using-catmull-rom-splines.html
// Computes a point on the Catmull-Rom spline given four control points and a parameter t in [0,1]
// This function interpolates smoothly between P1 and P2 using P0 and P3 for curvature control
Point catmullRom(const Point& P0, const Point& P1, const Point& P2, const Point& P3, double t) {
    double t2 = t * t;  // t squared
    double t3 = t2 * t; // t cubed

    // Compute x and y coordinates of the interpolated point using the Catmull-Rom formula
    double x = 0.5 * (
        (2 * P1.x) +
        (-P0.x + P2.x) * t +
        (2 * P0.x - 5 * P1.x + 4 * P2.x - P3.x) * t2 +
        (-P0.x + 3 * P1.x - 3 * P2.x + P3.x) * t3
        );

    double y = 0.5 * (
        (2 * P1.y) +
        (-P0.y + P2.y) * t +
        (2 * P0.y - 5 * P1.y + 4 * P2.y - P3.y) * t2 +
        (-P0.y + 3 * P1.y - 3 * P2.y + P3.y) * t3
        );

    return { x, y };
}

// Generates a smooth curve passing through P1 and P2 using Catmull-Rom interpolation
// P0 and P3 influence the shape of the curve but are not directly part of the curve
vector<Point> generateSmoothCurve(const Point& P0, const Point& P1, const Point& P2, const Point& P3, int numSamples) {
    vector<Point> curvePoints;

    // Generate 'numSamples' evenly spaced points along the curve
    for (int i = 0; i <= numSamples; ++i) {
        double t = static_cast<double>(i) / numSamples;  // Normalize t in range [0,1]
        curvePoints.push_back(catmullRom(P0, P1, P2, P3, t));
    }

    return curvePoints;
}

Point getPointOnCurve(const Point& P0, const Point& P1, const Point& P2, const Point& P3, double time) {
    if (time < 0.0 || time > 1.0) {
        throw std::out_of_range("Time parameter must be in the range [0,1]");
    }
    return catmullRom(P0, P1, P2, P3, time);
}

void plotCurve(const vector<Point>& curve) {
    const int width = 80;  // Grid width (columns)
    const int height = 80; // Grid height (rows)

    // Find min and max coordinates
    double minX = curve[0].x, maxX = curve[0].x;
    double minY = curve[0].y, maxY = curve[0].y;
    for (const auto& p : curve) {
        if (p.x < minX) minX = p.x;
        if (p.x > maxX) maxX = p.x;
        if (p.y < minY) minY = p.y;
        if (p.y > maxY) maxY = p.y;
    }

    // Create an empty grid
    vector<vector<char>> grid(height, vector<char>(width, ' '));

    // Map curve points to the grid
    for (const auto& p : curve) {
        int x = static_cast<int>((p.x - minX) / (maxX - minX) * (width - 1));
        int y = static_cast<int>((p.y - minY) / (maxY - minY) * (height - 1));
        y = height - 1 - y; // Flip Y-axis for correct orientation
        grid[y][x] = '*';
    }

    // Print the grid
    for (const auto& row : grid) {
        for (char c : row) {
            cout << c;
        }
        cout << endl;
    }
}

int main()
{
    // Define four control points.
    // P1 and P2 are the main curve points, while P0 and P3 provide curvature control
    Point P0 = { 0, 1 };  // First control point 
    Point P1 = { 2, 3 };  // Start of curve
    Point P2 = { 3, 3 };  // End of curve
    Point P3 = { 4, 1 };  // Last control point 

    // Generate a smooth curve between P1 and P2 with 100 sampled points
    vector<Point> curve = generateSmoothCurve(P0, P1, P2, P3, 100);

    /* Output the computed curve points for debugging
    for (const auto& p : curve) {
        cout << "(" << p.x << ", " << p.y << ")\n";
    }
    */

    plotCurve(curve);

    // Get an interpolated point at a specific time (e.g., 0.7)
    float time = 0.7;
    Point pointAtTime = getPointOnCurve(P0, P1, P2, P3, time);

    cout << "At time " << time << " the following point was observed on the curve : " << "(" << pointAtTime.x << ", " << pointAtTime.y << ")\n";
    
}

