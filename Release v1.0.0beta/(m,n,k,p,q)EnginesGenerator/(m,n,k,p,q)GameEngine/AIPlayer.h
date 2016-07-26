#pragma once
#include "Board.h"
#include "Move.h"
#include "Random.h"

class AIPlayer
{
public:
	AIPlayer(const Board& board);
	~AIPlayer();
	Move GetMove() const;
private:
	bool IsAdjacent(coord x, coord y) const;
	Random random;
	const Board* board;
};
