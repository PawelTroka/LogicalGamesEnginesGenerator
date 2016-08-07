#include "Random.h"
#include <ctime>


Random::Random()
{
	g_seed = std::time(nullptr);
}

size_t Random::GetValue(size_t min, size_t max) const
{
	return (min + fastrand()) % max;
}

Random::~Random()
{
}
