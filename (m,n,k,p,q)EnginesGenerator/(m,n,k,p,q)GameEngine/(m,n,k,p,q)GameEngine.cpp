// (m,n,k,p,q)GameEngine.cpp : Defines the exported functions for the DLL application.
//


#include <cstdint>
/*
#define M {_m_placeholder_}
#define N {_n_placeholder_}
#define K {_k_placeholder_}
#define P {_p_placeholder_}
#define Q {_q_placeholder_}
*/

#define _USE_LOCAL_DEFINES

#ifdef _USE_LOCAL_DEFINES
#define M 3
#define N 3
#define K 3
#define P 2
#define Q 1
#endif

#define BOARD_SIZE M*N
#define REQUIRES_ARRAYS BOARD_SIZE > 64


#define CHECK_BIT(var,pos) ((var) & (1<<(pos)))
#define SET_BIT(number,pos) number |= 1 << pos;
#define POSITION(x,y) (y*M + x)
typedef uint8_t coord;
typedef uint16_t arrayIndex_t;
/*
 *  m x n Board is like below (m rows, n columns)
 *        n
 *    **********
 *  m *        *
 *    **********
 */

class AI
{
	
};

struct Board
{
public:
	void Clear()
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

	void PlacePiece(coord x, coord y, bool isBlack)
	{

#if REQUIRES_ARRAYS
		if (isBlack)
		{
			blackPieces[POSITION(x, y)]=true;
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

	Board()
	{
	}

	Board(Board& b)
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

	bool IsEmpty(coord x, coord y) const
	{
		arrayIndex_t position = POSITION(x, y);
#if REQUIRES_ARRAYS
		return !blackPieces[position] && !whitePieces[position];
#else
		return !(CHECK_BIT(blackPieces, position)) && !(CHECK_BIT(whitePieces, position));
#endif		
	}

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
#else//if BOARD_SIZE > 4
	uint8_t blackPieces;
	uint8_t whitePieces;
#endif
};

enum Color
{
	Black,
	White
};

enum PlayerType
{
	Human,
	AI
};

class Game
{
public:
	Game() : board()
	{
	}

	void StartGame()
	{
		board.Clear();
		movesCount = 0;
		isFirstMove = true;
	}

	bool IsValidMove(coord x, coord y) const
	{
		return board.IsEmpty(x, y);
	}

	bool MakeMove(coord x, coord y)
	{
		if(board.IsEmpty(x,y) && players[currentColor]==PlayerType::Human)
		{
			board.PlacePiece(x, y, currentColor == Color::Black);
			return true;
		}
		return false;
	}

private:
	arrayIndex_t movesCount=0;
	bool isFirstMove=true;
	Board board;
	Color currentColor=Color::Black;
	PlayerType players[2];
} game;

struct Move
{
	coord x, y;
};