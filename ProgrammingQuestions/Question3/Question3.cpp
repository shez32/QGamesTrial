// Please fill in the rest of the C++ 'slow_string' class written below. 
// Try to replicate the STL string class's functionality as much as possible. 
// This question is to test how good you are at encapsulating the C standard library functions such as strXXX. 
// You can keep that question if you are not familiar with C++.

/*
class slow_string
{
	char* data;
public:
	...
}
*/

#include <cstring>   
#include <stdexcept> 
#include <iostream>  

class slow_string {
private:
    char* data; 

public:
    // Default constructor
    // data points to a null string
    slow_string() : data(new char[1] {'\0'}) {}

    // Constructor from C-style string
    // data points to a copy of the provided c-style string
    slow_string(const char* str) {
        // if str is not null
        if (str) {
            size_t len = std::strlen(str);
            data = new char[len + 1];
            std::strcpy(data, str);
        }
        // if supplied string is null
        // data points to a null string
        else {
            data = new char[1] {'\0'};
        }
    }

    // Copy constructor
    // data points to a copy of the provided slow_string reference
    slow_string(const slow_string& other) {
        size_t len = std::strlen(other.data);
        data = new char[len + 1];
        std::strcpy(data, other.data);
    }

    // Copy assignment operator
    // override the assign operator to avoid shallow copy
    slow_string& operator=(const slow_string& other) {
        // if we are assigning the same object, simply return the value
        if (this == &other)
            return *this;

        // deletes allocated memory......important step to avoid memory leaks
        delete[] data;
        // allocates memory based on the data being copied
        // data points to a copy of the provided slow_string reference
        size_t len = std::strlen(other.data);
        data = new char[len + 1];
        std::strcpy(data, other.data);
        return *this;
    }

    // Destructor
    ~slow_string() {
        delete[] data;
    }

    // Length of the string
    size_t length() const {
        return std::strlen(data);
    }

    // Access character at position (with bounds checking)
    // return value can be modified
    char& operator[](size_t pos) {
        if (pos >= length())
            throw std::out_of_range("Index out of range");
        return data[pos];
    }

    // Read character at position (with bounds checking)
    // return value is const and cannot be modified
    const char& operator[](size_t pos) const {
        if (pos >= length())
            throw std::out_of_range("Index out of range");
        return data[pos];
    }

    // Concatenation
    // concatenates current slow_string with provided slow_string
    slow_string& operator+=(const slow_string& other) {
        size_t len1 = length();
        size_t len2 = other.length();
        char* new_data = new char[len1 + len2 + 1];
        std::strcpy(new_data, data);
        std::strcat(new_data, other.data);
        delete[] data;
        data = new_data;
        return *this;
    }

    // Comparison operators
    bool operator==(const slow_string& other) const {
        return std::strcmp(data, other.data) == 0;
    }

    bool operator!=(const slow_string& other) const {
        return !(*this == other);
    }

    bool operator<(const slow_string& other) const {
        return std::strcmp(data, other.data) < 0;
    }

    bool operator<=(const slow_string& other) const {
        return std::strcmp(data, other.data) <= 0;
    }

    bool operator>(const slow_string& other) const {
        return std::strcmp(data, other.data) > 0;
    }

    bool operator>=(const slow_string& other) const {
        return std::strcmp(data, other.data) >= 0;
    }

    // c_str() function to access the underlying C-style string
    const char* c_str() const {
        return data;
    }
};

