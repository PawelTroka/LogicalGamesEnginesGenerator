
#include "Types.h"
#include "Board.h"

// ReSharper disable once CppPossiblyUninitializedMember
Board::Board()
{
}

Board::Board(const Board& b)
{
#if REQUIRES_ARRAYS
	for (arrayIndex_t i = 0; i<BOARD_SIZE; i++)
	{
		blackPieces[i] = b.blackPieces[i];
		whitePieces[i] = b.whitePieces[i];
	}
#else
	blackPieces = b.blackPieces;
	whitePieces = b.whitePieces;
#endif
}

Color Board::GetColor(coord x, coord y) const
{
	if (IsEmpty(x, y))
		return Color::None;
	if (IsColor(x, y, Color::Black))
		return Color::Black;
	return Color::White;
}

bool Board::IsEmpty(coord x, coord y) const
{
	//arrayIndex_t position = POSITION(x, y);
#if REQUIRES_ARRAYS
	return !blackPieces[POSITION(x, y)] && !whitePieces[POSITION(x, y)];
#else
	return !(CHECK_BIT(blackPieces, POSITION(x, y))) && !(CHECK_BIT(whitePieces, POSITION(x, y)));
#endif		
}

bool Board::IsColor(coord x, coord y,Color color) const
{
	//arrayIndex_t position = POSITION(x, y);

	if (color == Color::None)
		return IsEmpty(x, y);
	else if (color == Color::Black)
	{
#if REQUIRES_ARRAYS
		return blackPieces[POSITION(x, y)];
#else
		return CHECK_BIT(blackPieces, POSITION(x, y))==1;
#endif
	}
	else if(color == Color::White)
	{
#if REQUIRES_ARRAYS
		return whitePieces[POSITION(x, y)];
#else
		return CHECK_BIT(whitePieces, POSITION(x, y))==1;
#endif
	}
	return false;
}

uint16_t Board::CountPieces(char x, char y, Color color, char dx, char dy, Color* breakingColor) const
{
	arrayIndex_t count;
	auto currentColor=Color::None;

	if (dx==0 && dy==0)
		return IsColor(x,y,color) ? 1 : 0;
	for (count = 0; x >= 0 && x < Width() && y >= 0 && y < Height(); count++)
	{	
		currentColor = GetColor(x, y);
		if (color!=currentColor)
			break;
		x += dx;
		y += dy;
	}
	if (breakingColor != nullptr)
	// ReSharper disable once CppLocalVariableMightNotBeInitialized
		*breakingColor = currentColor;

	return count;
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
