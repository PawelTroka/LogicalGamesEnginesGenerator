#pragma once
#include "Board.h"
#include <string>

class Game
{
public:
	Game();

	void StartGame();

	bool IsValidMove(coord x, coord y) const;

	bool MakeMove(coord x, coord y);
	arrayIndex_t CountPieces(coord x, coord y, coord dist, Color color);
	bool GetMove();
	void WriteMove(coord x, coord y) const;
	void NextTurn();

	std::string engine_info(bool b);

	void GameLoop(int argc, char* argv[]);


private:
	arrayIndex_t movesLeft = Q;
	bool gameStarted = true;
	Board board;
	Color currentColor = Color::Black;
	PlayerType players[2];
};
