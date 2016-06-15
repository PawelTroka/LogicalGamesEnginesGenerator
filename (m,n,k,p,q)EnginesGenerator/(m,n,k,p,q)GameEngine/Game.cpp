// (m,n,k,p,q)GameEngine.cpp : Defines the exported functions for the DLL application.
//


#include <sstream>
#include <iostream>

#include <ctime>

#include "Types.h"
#include "Board.h"
#include "Game.h"
#include "Move.h"


Game::Game(): board()
{
	players[Color::Black] = PlayerType::Human;
	players[Color::White] = PlayerType::AI;
	std::srand(std::time(nullptr)); //use current time as seed for random generator

}

void Game::StartGame()
{
	board.Clear();
	movesLeft = Q;
	isFirstMove = true;
	GetMove();
}

bool Game::IsValidMove(coord x, coord y) const
{
	return board.IsEmpty(x, y) && players[currentColor] == PlayerType::Human && x<N && y<M && movesLeft>0;
}

bool Game::MakeMove(coord x, coord y)
{
	if (board.IsEmpty(x, y) && players[currentColor] == PlayerType::Human && x<N && y<M && movesLeft>0)
	{
		board.PlacePiece(x, y, currentColor == Color::Black);
		NextTurn();
		return true;
	}
	return false;
}

bool Game::GetMove()
{
	if(players[currentColor] == PlayerType::AI && movesLeft>0)
	{
		coord x = (std::rand() + 1) % N;
		coord y = (std::rand() + 1) % M;
		board.PlacePiece(x, y, currentColor == Color::Black);
		WriteMove(x, y);
		NextTurn();
		return true;
	}
	return false;
}

void Game::WriteMove(coord x, coord y) const
{
	if(currentColor==Color::Black)
	{
		sync_cout << "black move " << x << " " << y << sync_endl;
	}
	else if (currentColor == Color::White)
	{
		sync_cout << "white move " << x << " " << y << sync_endl;
	}
}

void Game::NextTurn()
{
	movesLeft--;
	if (movesLeft == 0)
	{
		if (currentColor == Color::Black)
			currentColor = Color::White;
		else
			currentColor = Color::Black;
		movesLeft = P;
	}
	else
		GetMove();
}

std::string Game::engine_info(bool b)
{
	return "(m,n,k,p,q)GameEngine v1.0.0 by Pawel Troka";
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

		//black ai
		//white human
		if (token == "newgame")
		{

			for(uint8_t i=0; is>>token;i++)
			{
				if(token=="black")
				{
					if(is >> token)
					{
						if (token == "ai")
							players[Color::Black] = PlayerType::AI;
						else if (token == "human")
							players[Color::White] = PlayerType::Human;
					}
				}
				else if(token=="white")
				{
					if (is >> token)
					{
						if (token == "ai")
							players[Color::White] = PlayerType::AI;
						else if(token=="human")
							players[Color::White] = PlayerType::Human;
					}
				}
			}
			StartGame();
		}
		else if (token == "info")
		sync_cout << "engine info: " << engine_info(true) << sync_endl;

		//setmove x y
		else if(token=="setmove")
		{
			is >> token;
			coord x = atoi(token.c_str());
			is >> token;
			coord y = atoi(token.c_str());
			if (MakeMove(x, y))
			{
				WriteMove(x, y);
				//sync_cout << "move ok" << cmd << sync_endl;
			}
			else
				sync_cout << "invalid move" << cmd << sync_endl;
		}


		else if (token == "isready") sync_cout << "readyok" << sync_endl;

		else if (token == "perft")
		{
		}
		else
		sync_cout << "Unknown command: " << cmd << sync_endl;
	}
	while (token != "quit" && token != "exit" && token != "stop" && argc == 1); // Passed args have one-shot behaviour
}
