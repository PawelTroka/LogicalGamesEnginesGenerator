// (m,n,k,p,q)GameEngine.cpp : Defines the entry point for the console application.
//

#include <iostream>

double hotFunction(double x)
{
	return x*x;
}

int main(int argc, char *argv[])
{
	double x;
	std::cout << hotFunction(13)<<std::endl;
	std::cin >> x;
	std::cout << hotFunction(x) << std::endl;
    return 0;
}

