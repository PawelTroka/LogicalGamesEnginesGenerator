// (m,n,k,p,q)GameEngine.cpp : Defines the exported functions for the DLL application.
//


#include <sstream>
#include <iostream>

#include "Types.h"
#include "Board.h"
#include "Game.h"





Game::Game(): board()
{
}

void Game::StartGame()
{
	board.Clear();
	movesCount = 0;
	isFirstMove = true;
}

bool Game::IsValidMove(coord x, coord y) const
{
	return board.IsEmpty(x, y);
}

bool Game::MakeMove(coord x, coord y)
{
	if (board.IsEmpty(x, y) && players[currentColor] == PlayerType::Human)
	{
		board.PlacePiece(x, y, currentColor == Color::Black);
		return true;
	}
	return false;
}

std::string Game::engine_info(bool b)
{
	return "";
}

void Game::GameLoop(int argc, char* argv[])
{
	std::string token, cmd;


	for (int i = 1; i < argc; ++i)
		cmd += std::string(argv[i]) + " ";

	do
	{
		if (argc == 1 && !getline(std::cin, cmd)) // Block here waiting for input or EOF
			cmd = "quit";

		std::istringstream is(cmd);

		token.clear(); // getline() could return empty or blank line
		is >> std::skipws >> token;

		if (token == "newgame")
		{
			StartGame();
		}
		else if (token == "info")
		sync_cout << "engine info: " << engine_info(true) << sync_endl;


		else if (token == "isready") sync_cout << "readyok" << sync_endl;

		else if (token == "perft")
		{
		}
		else
		sync_cout << "Unknown command: " << cmd << sync_endl;
	}
	while (token != "quit" && token != "exit" && token != "stop" && argc == 1); // Passed args have one-shot behaviour
}
