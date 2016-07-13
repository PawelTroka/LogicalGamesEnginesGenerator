#pragma once
#include <cstdint>
#include "Types.h"

/*

/*
*  m x n Board is like below (m rows, n columns)
*        n
*    **********
*    *        *
*  m *        *
*    *        *
*    **********
*/


class Board
{
public:
	void Clear();

	void PlacePiece(coord x, coord y, bool isBlack);

	Board();

	Board(const Board& b);
	Color GetColor(coord x, coord y) const;

	static inline coord Height()
	{
		return M;
	}

	static inline coord Width()
	{
		return N;
	}

#define POSITION(x,y) (y*N + x)
	bool IsEmpty(coord x, coord y) const;
	bool IsColor(coord x, coord y, Color color) const;
	uint16_t CountPieces(char x, char y, Color color, char dx, char dy, Color* breakingColor) const;
private:
#if BOARD_SIZE > 64
	bool blackPieces[M*N];
	bool whitePieces[M*N];
#elif BOARD_SIZE > 32
	uint64_t blackPieces;
	uint64_t whitePieces;
#elif BOARD_SIZE > 16
	uint32_t blackPieces;
	uint32_t whitePieces;
#elif BOARD_SIZE > 8
	uint16_t blackPieces;
	uint16_t whitePieces;
#else
	uint8_t blackPieces;
	uint8_t whitePieces;
#endif
};
