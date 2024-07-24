using UnityEngine;
using System;
public class Randomizer
{
    private ulong[] s = new ulong[2];

    // 생성자: 초기 시드 설정
    public Randomizer(ulong seed1, ulong seed2)
    {
        s[0] = seed1;
        s[1] = seed2;
    }

    public ulong Next()
    {
        ulong x = s[0];
        ulong y = s[1];
        s[0] = y;
        x ^= x << 23;
        s[1] = x ^ y ^ (x >> 17) ^ (y >> 26);
        return s[1] + y;
    }

    // 0 ~ maxRange-1 범위의 정수 난수 생성
    public int NextInt(int maxRange)
    {
        return (int)(Next() % (ulong)maxRange);
    }

    // minRange ~ maxRange-1 범위의 정수 난수 생성
    public int NextInt(int minRange, int maxRange)
    {
        return minRange + (int)(Next() % (ulong)(maxRange - minRange));
    }

    // 0.0 ~ 1.0 범위의 실수 난수 생성
    public double NextDouble()
    {
        return (Next() >> 11) * (1.0 / (1UL << 53));
    }

    // minRange ~ maxRange 범위의 실수 난수 생성
    public double NextDouble(double minRange, double maxRange)
    {
        return minRange + NextDouble() * (maxRange - minRange);
    }

    // 0.0 ~ 1.0 범위의 부동 소수점 난수 생성
    public float NextFloat()
    {
        return (float)NextDouble();
    }

    // minRange ~ maxRange 범위의 부동 소수점 난수 생성
    public float NextFloat(float minRange, float maxRange)
    {
        return minRange + NextFloat() * (maxRange - minRange);
    }
}