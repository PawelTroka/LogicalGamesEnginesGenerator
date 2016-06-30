// (m,n,k,p,q)GameEngine.cpp : Defines the entry point for the console application.
//

#include <iostream>
#include "Game.h"


int main(int argc, char* argv[])
{
	Game game = Game();

	game.GameLoop(argc, argv);
	return 0;
}
