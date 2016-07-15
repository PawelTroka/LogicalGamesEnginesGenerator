#pragma once
#include "Board.h"
#include "Move.h"
#include "Random.h"

class AIPlayer
{
public:
	AIPlayer(Board& board);
	~AIPlayer();
	Move GetMove() const;
	bool IsAdjacent(coord x, coord y) const;

private:
	Random random;
	Board* board;
};
