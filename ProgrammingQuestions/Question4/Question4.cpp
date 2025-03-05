// Please implement the standard functionality required from a 3 element vector for a math library. 
// The design should be object oriented and protect its data members from public access. 
// Please support simple operations on the vector such as addition, subtraction, multiplication and division as well as dot and cross product.


#include <iostream>
#include <cmath>

class Vector3 {
private:
    // internal representation of a 3D vector
    // we keep these variables private to prevent public access
    float x;
    float y;
    float z;

public:
    // default constructor
    Vector3(float x = 0.0f, float y = 0.0f, float z = 0.0f) : x(x), y(y), z(z) {}

    // Addition of two vectors
    Vector3 operator+(const Vector3& other) const 
    {
        return Vector3(x + other.x, y + other.y, z + other.z);
    }

    // Subtraction of two vectors
    Vector3 operator-(const Vector3& other) const 
    {
        return Vector3(x - other.x, y - other.y, z - other.z);
    }

    // Scalar multiplication
    Vector3 operator*(float scalar) const 
    {
        return Vector3(x * scalar, y * scalar, z * scalar);
    }

    // Scalar division (handles divide-by-zero case)
    Vector3 operator/(float scalar) const 
    {
        if (scalar == 0) throw std::runtime_error("Division by zero is not allowed");
        return Vector3(x / scalar, y / scalar, z / scalar);
    }

    // Dot product of two vectors
    // Sum of the products of the corresponding elements x,y,z
    float dot(const Vector3& other) const 
    {
        return x * other.x + y * other.y + z * other.z;
    }

    // Cross product of two vectors
    // using matrix notation and determinants
    Vector3 cross(const Vector3& other) const 
    {
        return Vector3(
            y * other.z - z * other.y, // x
            z * other.x - x * other.z, // y
            x * other.y - y * other.x  // z
        );
    }

    // print to console the provided vector3
    void print() const {
        std::cout << "(" << x << ", " << y << ", " << z << ")" << std::endl;
    }
};

int main()
{
    Vector3 v1(1, 2, 3);
    Vector3 v2(4, 5, 6);

    Vector3 sum = v1 + v2;
    Vector3 difference = v1 - v2;
    Vector3 scaled = v1 * 2;
    Vector3 divided = v2 / 2;
    float dotProduct = v1.dot(v2);
    Vector3 crossProduct = v1.cross(v2);

    std::cout << "Sum: "; sum.print();
    std::cout << "Difference: "; difference.print();
    std::cout << "Scaled: "; scaled.print();
    std::cout << "Divided: "; divided.print();
    std::cout << "Dot Product: " << dotProduct << std::endl;
    std::cout << "Cross Product: "; crossProduct.print();
}
