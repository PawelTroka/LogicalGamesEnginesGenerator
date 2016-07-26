#pragma once

class Random
{
public:
	Random();
	size_t GetValue(size_t min, size_t max) const;

	~Random();
private:
	mutable size_t g_seed;

	inline size_t fastrand() const
	{
		g_seed = 214013 * g_seed + 2531011;
		return (g_seed >> 16) & 0x7FFF;
	}
};
