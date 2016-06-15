
#include "Types.h"
#include "Board.h"

Board::Board()
{
}

Board::Board(Board& b)
{
#if REQUIRES_ARRAYS
	for (arrayIndex_t i = 0; i<BOARD_SIZE; i++)
	{
		blackPieces[i] = b[i];
		whitePieces[i] = b[i];
	}
#else
	blackPieces = b.blackPieces;
	whitePieces = b.whitePieces;
#endif
}

bool Board::IsEmpty(coord x, coord y) const
{
	arrayIndex_t position = POSITION(x, y);
#if REQUIRES_ARRAYS
	return !blackPieces[position] && !whitePieces[position];
#else
	return !(CHECK_BIT(blackPieces, position)) && !(CHECK_BIT(whitePieces, position));
#endif		
}

bool Board::IsColor(coord x, coord y,Color color) const
{
	arrayIndex_t position = POSITION(x, y);

	if (color == Color::Black)
	{
#if REQUIRES_ARRAYS
		return blackPieces[position];
#else
		return CHECK_BIT(blackPieces, position);
#endif
	}
	else if(color == Color::White)
	{
#if REQUIRES_ARRAYS
		return whitePieces[position];
#else
		return CHECK_BIT(whitePieces, position);
#endif
	}
	return false;
}

void Board::PlacePiece(coord x, coord y, bool isBlack)
{
#if REQUIRES_ARRAYS
	if (isBlack)
	{
		blackPieces[POSITION(x, y)] = true;
	}
	else
	{
		whitePieces[POSITION(x, y)] = true;
	}
#else
	if (isBlack)
	{
		SET_BIT(blackPieces, POSITION(x, y));
	}
	else
	{
		SET_BIT(whitePieces, POSITION(x, y));
	}
#endif
}

void Board::Clear()
{
#if REQUIRES_ARRAYS
	for (arrayIndex_t i = 0; i<BOARD_SIZE; i++)
	{
		blackPieces[i] = false;
		whitePieces[i] = false;
	}
#else
	blackPieces = 0;
	whitePieces = 0;
#endif
}
