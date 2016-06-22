#pragma once
#include "Board.h"
#include <string>
#include <vector>
#include "AIPlayer.h"

class Game
{
public:
	Game();

	void StartGame();

	bool IsValidMove(coord x, coord y) const;

	bool MakeMove(coord x, coord y);
	bool GetMove();
	bool CheckWin(coord x, coord y, coord& x1, coord& y1, coord& x2, coord& y2) const;
	void WriteMove(coord x, coord y) const;
	bool CheckGameEnd(coord x, coord y);
	void NextTurn();

	static std::string engine_info(bool b);

	void GameLoop(int argc, char* argv[]);


private:
	std::vector<double> aiGetMoveTimesInMicroseconds;
	std::vector<double> checkGameEndTimesInMicroseconds;
	uint16_t movesLeft = Q;
	uint16_t movesMade = 0;
	bool gameStarted = false;
	Board board;
	AIPlayer aiPlayer;
	Color currentColor = Color::Black;
	PlayerType players[2];
};
