// (m,n,k,p,q)GameEngine.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

extern "C"
{//http://stackoverflow.com/questions/10109590/unable-to-find-an-entry-point-when-calling-c-dll-in-c-sharp/10109659#10109659
	
	__declspec(dllexport) double hotFunction(double x)
	{
		return x*x;
	}
}