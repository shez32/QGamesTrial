//Please implement the following mathematical function, in the way you think is best(explain your choice).

/*
F(0) = 0
F(n) = n + F(n - 1)
n = 1..20
*/

//Please answer in C++ (or C# if you are not familiar with C++).

#include <iostream>

// The best way to do the sum of numbers mathematical function is to simply use the Sum of Natural Numbers formula
// formula can be found here: https://www.cuemath.com/sum-of-natural-numbers-formula/
// constant O(1) execution time i.e. best performance
int SumOfNumbers(int n) {
    if (n < 0 || n > 20) {
        std::cerr << "Warning Input is out of specified range.\n";
        return -1;
    }

    return (n * (n + 1) / 2);
}

// We can also do this with recursion
// however for large numbers this would be a bad/ unoptimized approach
int recursiveSumOfNumbers(int n) {
    if (n < 0 || n > 20) {
        std::cerr << "Warning Input is out of specified range.\n";
        return -1;
    }

    return n == 0 ? 0 : n + recursiveSumOfNumbers(n - 1);
}

int main()
{
    int userInput;

    std::cout << "Enter a number (1 - 20): ";
    std::cin >> userInput;

    std::cout << "Sum of first " << userInput << " natural numbers is : " << SumOfNumbers(userInput) << std::endl;
}




