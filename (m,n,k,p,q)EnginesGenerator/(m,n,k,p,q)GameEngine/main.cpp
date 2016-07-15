// (m,n,k,p,q)GameEngine.cpp : Defines the entry point for the console application.
//

#include "Game.h"


int main(int argc, char* argv[])
{
	auto game = Game();

	game.GameLoop(argc, argv);
	return 0;
}
