#pragma once
//#define _USE_GENERATOR_DEFINES

#ifndef _USE_GENERATOR_DEFINES
#define _USE_LOCAL_DEFINES
#endif

#ifdef _USE_LOCAL_DEFINES
#define M 3
#define N 3
#define K 3
#define P 1
#define Q 1
//#define EXACTLY_K_TO_WIN
#endif

#ifndef EXACTLY_K_TO_WIN
#define K_OR_MORE_TO_WIN 
#endif

#define BOARD_SIZE M*N
#define REQUIRES_ARRAYS BOARD_SIZE > 64

#if BOARD_SIZE > 32
#define CHECK_BIT(var,pos) ((var) & (1i64<<(pos)))
#define SET_BIT(number,pos) number |= 1i64 << pos;
#else
#define CHECK_BIT(var,pos) ((var) & (1 << (pos)))
#define SET_BIT(number,pos) number |= 1 << pos;
#endif

#include <cstdint>
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


enum SyncCout
{
	IO_LOCK,
	IO_UNLOCK
};

std::ostream& operator<<(std::ostream&, SyncCout);

#define sync_cout std::cout //<< IO_LOCK
#define sync_endl std::endl //<< IO_UNLOCK
