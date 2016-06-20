#include "AIPlayer.h"
#include <vector>
#include <ctime>

AIPlayer::AIPlayer(Board& board) : random()
{
	this->board = &board;
	//std::srand(std::time(nullptr)); //use current time as seed for random generator
}

AIPlayer::~AIPlayer()
{
}

Move AIPlayer::GetMove() 
{
	Move move;
	std::vector<Move> empties;

	for (move.y = 0; move.y<board->Height(); move.y++)
		for (move.x = 0; move.x<board->Width(); move.x++)
		{
			if (IsAdjacent(move.x, move.y))
				empties.push_back(move);
		}
	
	if(empties.empty())
	{
		move.y = board->Height() / 2;
		move.x = board->Width() / 2;
		return move;
	}

	auto index = random.GetValue(0, empties.size());//rand() % empties.size();

	return empties[index];
}


bool AIPlayer::IsAdjacent(coord x, coord y) const
{
	if (!(this->board->IsEmpty(x,y)))
		return false;
	for (char dy = -1; dy < 2; dy++)
	{
		for (char dx = -1; dx < 2; dx++)
		{
			if (dx==0 && dy==0)
				continue;
			Color breakingColor;
			auto count = board->CountPieces(x, y, Color::None, dx, dy, &breakingColor);
			if (count < 2 && breakingColor != Color::None)
				return true;
		}
	}
	return false;
}


