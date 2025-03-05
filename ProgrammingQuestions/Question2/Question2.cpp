//Please use the C++ STL vector class to make a triangle list class, for use in 3d drawing.For each triangle please store 3 vertex positions, 3 colours, and 1 face normal.


#include <iostream>
#include <vector>


// custom struct to define a vector point in 3D space
struct Vector3 {
    float x, y, z;

    Vector3(float x = 0, float y = 0, float z = 0) : x(x), y(y), z(z) {}
};

// likewise, a similar struct to hold color data for vertices
struct Color {
    float r, g, b;

    Color(float r = 255, float g = 255, float b = 255) : r(r), g(g), b(b) {}
};


struct Triangle {
    Vector3 vertices[3];    // array of 3 vertices required for creating a triangle
    Color colors[3];        // array of 3 colors assigned to each vertex - we can also create a color variable inside the vector3 struct
    Vector3 normal;         // Face normal of the triangle - for sake of complexity, we will assume the normal has already been calculated

    Triangle(const Vector3& v1, const Vector3& v2, const Vector3& v3,
        const Color& c1, const Color& c2, const Color& c3,
        const Vector3& normal) :
        vertices{v1, v2, v3}, colors{c1, c2, c3}, normal(normal) {}
};

class TriangleList {
private:
    std::vector<Triangle> triangles;    // vector to store list of triangles

public:
    TriangleList() = default;

    // add a triangle to the list
    void addTriangle(const Triangle& triangle) {
        triangles.push_back(triangle);
    }

    // retrieve a triangle from the specified index
    const Triangle& getTriangle(size_t index) const {
        if (index >= triangles.size()) {
            throw std::out_of_range("Index is out of range!");
        }

        return triangles[index];
    }

    // get the total number of triangles inside the list
    size_t size() const {
        return triangles.size();
    }

    // we can add further methods like below to augment functionality................
    void drawTriangle(size_t index) {
        //draw specified triangle at the given index
    }

};

int main()
{
    TriangleList triangleList;

    //--------------------------------------------------------
    Vector3 v1(0.0f, 0.0f, 0.0f); // Origin
    Vector3 v2(1.0f, 0.0f, 0.0f); // X-Unit vector
    Vector3 v3(0.0f, 1.0f, 0.0f); // Y-Unit Vector

    Color c1(1.0f, 0.0f, 0.0f); // Red
    Color c2(0.0f, 1.0f, 0.0f); // Green
    Color c3(0.0f, 0.0f, 1.0f); // Blue

    Vector3 normal(0.0f, 0.0f, 1.0f); // Z-Axis normal

    //--------------------------------------------------------
   

    Triangle triangle(v1, v2, v3, c1, c2, c3, normal);

    triangleList.addTriangle(triangle);
    triangleList.addTriangle(triangle);

    std::cout << "Number of triangles: " << triangleList.size() << std::endl;

}

