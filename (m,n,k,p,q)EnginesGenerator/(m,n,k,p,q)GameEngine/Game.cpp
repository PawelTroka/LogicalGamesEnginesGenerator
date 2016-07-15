#include <sstream>
#include <iostream>
#include <vector>
#include <chrono>

#include <numeric>
#include "Types.h"
#include "Board.h"
#include "Game.h"
#include "Move.h"


Game::Game(): board(), aiPlayer(board)
{
	players[Color::Black] = PlayerType::Human;
	players[Color::White] = PlayerType::AI;
}

void Game::StartGame()
{
	board.Clear();
	movesLeft = Q;
	gameStarted = true;
	currentColor = Color::Black;
	movesMade = 0;
}

bool Game::IsValidMove(coord x, coord y) const
{
	return board.IsEmpty(x, y) && players[currentColor] == PlayerType::Human && x < N && y < M && movesLeft > 0;
}

bool Game::MakeMove(coord x, coord y)
{
	if (board.IsEmpty(x, y) && players[currentColor] == PlayerType::Human && x < N && y < M && movesLeft > 0)
	{
		board.PlacePiece(x, y, currentColor == Color::Black);
		return true;
	}
	return false;
}

Move* Game::GetMoves() const
{
	auto moves = new Move[BOARD_SIZE - movesMade];
	auto i = 0;
	for (coord y = 0; y < board.Height(); y++)
		for (coord x = 0; x < board.Width(); x++)
			if (board.IsEmpty(x, y))
			{
				Move m;
				m.x = x;
				m.y = y;
				moves[i++] = m;
			}

	return moves;
}

bool Game::GetMove()
{
	if (players[currentColor] == PlayerType::AI && movesLeft > 0 && gameStarted)
	{
		auto t1 = std::chrono::high_resolution_clock::now();

		auto aiMove = aiPlayer.GetMove();
		
		board.PlacePiece(aiMove.x, aiMove.y, currentColor == Color::Black);
		auto t2 = std::chrono::high_resolution_clock::now();
		auto duration = std::chrono::duration_cast<std::chrono::nanoseconds>(t2 - t1).count();
		aiGetMoveTimes.push_back(duration);

		WriteMove(aiMove.x, aiMove.y);
		movesMade++;
		if (!CheckGameEnd(aiMove.x, aiMove.y))
			NextTurn();
		return true;
	}
	return false;
}

bool Game::CheckWin(coord x, coord y, coord& x1, coord& y1, coord& x2, coord& y2) const
{
	uint16_t c1, c2;
	char xs[] = {1, 1, 0, -1}, ys[] = {0, 1, 1, 1};

	if (board.IsEmpty(x, y))
		return false;

	for (char i = 0; i < 4; i++)
	{
		c1 = board.CountPieces(x, y, currentColor, xs[i], ys[i], nullptr);
		c2 = board.CountPieces(x, y, currentColor, -xs[i], -ys[i], nullptr);

#if defined(K_OR_MORE_TO_WIN)
		if (c1 + c2 - 1 >= K)
#elif defined(EXACTLY_K_TO_WIN)
		if (c1 + c2 - 1 == K)
#endif
		{
			x1 = coord(x + xs[i] * (c1 - 1));
			y1 = coord(y + ys[i] * (c1 - 1));
			x2 = coord(x - xs[i] * (c2 - 1));
			y2 = coord(y - ys[i] * (c2 - 1));
			return true;
		}
	}
	return false;
}

void Game::WriteMove(coord x, coord y) const
{
	sync_cout << "move " << (currentColor == Color::Black ? "black " : "white ") << std::to_string(x + 1) << " " << std::to_string(y + 1) << sync_endl;
}

bool Game::CheckGameEnd(coord x, coord y)
{
	coord x1, x2, y1, y2;

	auto t1 = std::chrono::high_resolution_clock::now();

	auto isWin = CheckWin(x, y, x1, y1, x2, y2);

	auto t2 = std::chrono::high_resolution_clock::now();
	auto duration = std::chrono::duration_cast<std::chrono::nanoseconds>(t2 - t1).count();
	checkGameEndTimes.push_back(duration);

	if (isWin)
	{
		sync_cout << "winner is " << (currentColor == Color::Black ? "black" : "white") << sync_endl;
		sync_cout << "wining line is from " << std::to_string(x1 + 1) << " " << std::to_string(y1 + 1) << " to " << std::to_string(x2 + 1) << " " << std::to_string(y2 + 1) << sync_endl;
		gameStarted = false;
		return true;
	}
	else if (movesMade == BOARD_SIZE)
	{
		sync_cout << "draw" << sync_endl;
		gameStarted = false;
		return true;
	}
	return false;
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
	if (b)
	{
		auto ret = "(" + std::to_string(M) + "," + std::to_string(N) + "," + std::to_string(K) + "," + std::to_string(P) + "," + std::to_string(Q) + ")";

#if defined(K_OR_MORE_TO_WIN)
		ret += "K_OR_MORE_TO_WIN";
#elif defined(EXACTLY_K_TO_WIN)
		ret += "EXACTLY_K_TO_WIN";
#endif
		return ret;
	}
	else
		return "(m,n,k,p,q)GameEngine v1.0.0 by Pawel Troka";
}

void Game::GameLoop(int argc, char* argv[])
{
	std::string token, cmd;


	for (auto i = 1; i < argc; ++i)
		cmd += std::string(argv[i]) + " ";

	do
	{
		if (gameStarted)
			while (GetMove())
			{
			}

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
			for (uint8_t i = 0; is >> token; i++)
			{
				if (token == "black")
				{
					if (is >> token)
					{
						if (token == "ai")
							players[Color::Black] = PlayerType::AI;
						else if (token == "human")
							players[Color::White] = PlayerType::Human;
					}
				}
				else if (token == "white")
				{
					if (is >> token)
					{
						if (token == "ai")
							players[Color::White] = PlayerType::AI;
						else if (token == "human")
							players[Color::White] = PlayerType::Human;
					}
				}
			}
			StartGame();
			sync_cout << "game started" << sync_endl;
		}
		else if (token == "isready") sync_cout << "readyok" << sync_endl;
		else if (token == "perf")
		{
			sync_cout << "average AIPlayer::GetMove() execution is " << std::accumulate(aiGetMoveTimes.begin(), aiGetMoveTimes.end(), 0.0) / aiGetMoveTimes.size() << " ns " << sync_endl;
			sync_cout << "average Game::CheckWin() execution is " << std::accumulate(checkGameEndTimes.begin(), checkGameEndTimes.end(), 0.0) / checkGameEndTimes.size() << " ns " << sync_endl;
			sync_cout << "average Game::GetMoves() execution is " << std::accumulate(getMovesTimes.begin(), getMovesTimes.end(), 0.0) / getMovesTimes.size() << " ns " << sync_endl;
		}
		else if (token == "printboard")
		{
			for (coord y = 0; y < board.Height(); y++)
			{
				for (coord x = 0; x < board.Width(); x++)
				{
					if (board.IsEmpty(x, y))
					sync_cout << " ";
					else if (board.IsColor(x, y, Color::Black))
					sync_cout << "X";
					else if (board.IsColor(x, y, Color::White))
					sync_cout << "O";
				}
				sync_cout << sync_endl;
			}
		}
		else if (token == "info")
		sync_cout << "engine info: " << engine_info(true) << sync_endl;

		else if (token == "quit"
			|| token == "exit" || token == "stop")
		sync_cout << "(m,n,k,p,q)GameEngine has exited" << sync_endl;


		//makemove x y
		else if (token == "makemove" && gameStarted)
		{
			is >> token;
			coord x = coord(atoi(token.c_str())) - 1;
			is >> token;
			coord y = coord(atoi(token.c_str())) - 1;
			if (MakeMove(x, y))
			{
				WriteMove(x, y);
				movesMade++;
				if (!CheckGameEnd(x, y))
					NextTurn();
			}
			else
			sync_cout << "invalid move" << sync_endl;
		}
		else if (token == "getplayer" && gameStarted)
		{
			sync_cout << (currentColor == Color::Black ? "black" : currentColor == Color::White ? "white" : "none") << sync_endl;
		}
		else if (token == "getmoves" && gameStarted)
		{
			auto t1 = std::chrono::high_resolution_clock::now();
			auto moves = GetMoves();
			auto t2 = std::chrono::high_resolution_clock::now();
			auto duration = std::chrono::duration_cast<std::chrono::nanoseconds>(t2 - t1).count();

			getMovesTimes.push_back(duration);

			sync_cout << "moves:";

			for (arrayIndex_t i = 0; i < BOARD_SIZE - movesMade; i++)
			sync_cout << " (" << std::to_string(moves[i].x + 1) << " " << std::to_string(moves[i].y + 1) << ")";

			delete[] moves;

			sync_cout << sync_endl;
		}
		else if (token == "movesleft" && gameStarted)
		{
			sync_cout << movesLeft << sync_endl;
		}
		else if (token == "movesmade" && gameStarted)
		{
			sync_cout << movesMade << sync_endl;
		}
		else if (!gameStarted)
		{
			sync_cout << "game NOT started or ended, call newgame first" << sync_endl;
		}

		else
		sync_cout << "Unknown command: " << cmd << sync_endl;
	}
	while (token != "quit" && token != "exit" && token != "stop" && argc == 1); // Passed args have one-shot behaviour
}
