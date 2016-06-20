#pragma once
/*
#define M {_m_placeholder_}
#define N {_n_placeholder_}
#define K {_k_placeholder_}
#define P {_p_placeholder_}
#define Q {_q_placeholder_}
*/

//#define _USE_GENERATOR_DEFINES

#ifndef _USE_GENERATOR_DEFINES
#define _USE_LOCAL_DEFINES
#endif

#ifdef _USE_LOCAL_DEFINES
#define M 8
#define N 8
#define K 3
#define P 2
#define Q 1
//#define EXACTLY_K_TO_WIN
#endif

#ifndef EXACTLY_K_TO_WIN
#define K_OR_MORE_TO_WIN 
#endif

#define BOARD_SIZE M*N
#define REQUIRES_ARRAYS BOARD_SIZE > 64


#define CHECK_BIT(var,pos) ((var) & (1i64<<(pos)))
#define SET_BIT(number,pos) number |= 1i64 << pos;

#include <cstdint>
#include <iostream>
#include <ostream>
typedef uint8_t coord;
typedef uint16_t arrayIndex_t;

enum Color
{
	Black,
	White,
	None
};

enum PlayerType
{
	Human,
	AI
};



enum SyncCout { IO_LOCK, IO_UNLOCK };
 std::ostream& operator<<(std::ostream&, SyncCout);

#define sync_cout std::cout //<< IO_LOCK
#define sync_endl std::endl //<< IO_UNLOCK