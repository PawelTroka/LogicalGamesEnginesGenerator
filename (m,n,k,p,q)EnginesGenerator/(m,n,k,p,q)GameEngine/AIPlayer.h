#pragma once
#include "Board.h"
#include "Move.h"

class AIPlayer
{
public:
	AIPlayer(const Board& board);
	~AIPlayer();
	Move GetMove() const;
	bool IsAdjacent(coord x, coord y) const;

private:



	Board board;
};

