// (m,n,k,p,q)GameEngine.cpp : Defines the exported functions for the DLL application.
//


#include <sstream>
#include <iostream>
#include <vector>
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
	gameStarted = true;
	currentColor = Color::Black;
	//GetMove();
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

		return true;
	}
	return false;
}


arrayIndex_t Game::CountPieces(coord x, coord y, coord dist, Color color)
{
	return 0;
}

bool Game::GetMove()
{
	if(players[currentColor] == PlayerType::AI && movesLeft>0)
	{
		coord x = N/2;
		coord y = M/2;

		std::vector<Move> empties;

		for (coord iy = 0; iy<M; iy++)
			for (coord ix = 0; ix<N; ix++)
			{
				
			}
		

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
		sync_cout << "move black " << std::to_string(x+1) << " " << std::to_string(y+1) << sync_endl;
	}
	else if (currentColor == Color::White)
	{
		sync_cout << "move white " << std::to_string(x+1) << " " << std::to_string(y+1) << sync_endl;
	}
}

void Game::NextTurn()
{
	movesLeft--;
	if (movesLeft <= 0)
	{
		if (currentColor == Color::Black)
			currentColor = Color::White;
		else
			currentColor = Color::Black;
		movesLeft = P;
	}
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
		if(gameStarted)
			while (GetMove()){}

		if (argc == 1 && !getline(std::cin, cmd)) // Block here waiting for input or EOF
			cmd = "quit";

		std::istringstream is(cmd);

		token.clear(); // getline() could return empty or blank line
		is >> std::skipws >> token;

		//black ai
		//white human
		if (token == "newgame")
		{
			players[Color::Black] = PlayerType::Human;
			players[Color::White] = PlayerType::AI;
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
			sync_cout << "game started" << sync_endl;
		}
		else if (token == "info")
		sync_cout << "engine info: " << engine_info(true) << sync_endl;

		//makemove x y
		else if(token=="makemove")
		{
			is >> token;
			coord x = atoi(token.c_str())-1;
			is >> token;
			coord y = atoi(token.c_str())-1;
			if (MakeMove(x, y))
			{
				WriteMove(x, y);
				NextTurn();
				//sync_cout << "move ok" << cmd << sync_endl;
			}
			else
				sync_cout << "invalid move"<< sync_endl;
		}


		else if (token == "isready") sync_cout << "readyok" << sync_endl;

		else if (token == "printboard")
		{
			for (arrayIndex_t y = 0; y<M; y++)
			
			{
				for (arrayIndex_t x = 0; x<N; x++)
				{
					
					if (board.IsColor(x, y,Color::Black))
						sync_cout << "X";
					else if(board.IsColor(x, y, Color::White))
						sync_cout << "O";
					else
						sync_cout << " ";
				}
				sync_cout << sync_endl;
			}
		}
		else
		sync_cout << "Unknown command: " << cmd << sync_endl;
	}
	while (token != "quit" && token != "exit" && token != "stop" && argc == 1); // Passed args have one-shot behaviour
}
