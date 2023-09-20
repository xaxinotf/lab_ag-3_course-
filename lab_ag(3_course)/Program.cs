using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Program
{
    // Stage 1: Calculation of Euler and Möbius functions. Finding the least common multiple of numbers.

    public static BigInteger Gcd(BigInteger a, BigInteger b)
    {
        while (b != 0)
        {
            BigInteger temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static BigInteger Lcm(BigInteger a, BigInteger b)
    {
        return a * b / Gcd(a, b);
    }

    public static BigInteger EulerPhi(BigInteger n)
    {
        BigInteger result = n;
        for (BigInteger i = 2; i * i <= n; i++)
        {
            if (n % i == 0)
            {
                while (n % i == 0)
                {
                    n /= i;
                }
                result -= result / i;
            }
        }
        if (n > 1)
        {
            result -= result / n;
        }
        return result;
    }

    public static BigInteger Mobius(BigInteger n)
    {
        if (n == 1)
        {
            return 1;
        }
        BigInteger result = 1;
        for (BigInteger i = 2; i * i <= n; i++)
        {
            if (n % i == 0)
            {
                int cnt = 0;
                while (n % i == 0)
                {
                    n /= i;
                    cnt++;
                }
                if (cnt > 1)
                {
                    return 0;
                }
                result = -result;
            }
        }
        if (n > 1)
        {
            return -result;
        }
        return result;
    }

    // Stage 2: Solving the system of linear congruences using the Chinese Remainder Theorem

    public static BigInteger ChineseRemainderTheorem(List<(BigInteger, BigInteger)> congruences)
    {
        BigInteger N = 1;
        foreach (var (a, m) in congruences)
        {
            N *= m;
        }
        BigInteger result = 0;
        foreach (var (a, m) in congruences)
        {
            BigInteger Ni = N / m;
            BigInteger Mi = ModInverse(Ni, m);
            result += a * Ni * Mi;
        }
        return result % N;
    }

    // Helper function to calculate modular inverse

    public static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m;
        BigInteger x0 = 0;
        BigInteger x1 = 1;

        while (a > 1)
        {
            BigInteger q = a / m;
            BigInteger t = m;

            m = a % m;
            a = t;
            t = x0;

            x0 = x1 - q * x0;
            x1 = t;
        }

        if (x1 < 0)
        {
            x1 += m0;
        }

        return x1;
    }

    // Stage 3: Calculation of Legendre and Jacobi symbols

    public static int LegendreSymbol(BigInteger a, BigInteger p)
    {
        if (a % p == 0)
        {
            return 0;
        }
        else if (BigInteger.ModPow(a, (p - 1) / 2, p) == 1)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public static int JacobiSymbol(BigInteger a, BigInteger n)
    {
        if (n <= 0 || n % 2 == 0)
        {
            throw new ArgumentException("Jacobi symbol is only defined for odd positive n");
        }
        a = a % n;
        if (a == 0)
        {
            return 0;
        }
        int result = 1;
        while (a != 0)
        {
            while (a % 2 == 0)
            {
                a /= 2;
                if (n % 8 == 3 || n % 8 == 5)
                {
                    result = -result;
                }
            }
            BigInteger temp = a;
            a = n;
            n = temp;
            if (a % 4 == 3 && n % 4 == 3)
            {
                result = -result;
            }
            a = a % n;
        }
        if (n == 1)
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    // Stage 4: Pollard's Rho Algorithm for Integer Factorization

    public static BigInteger PollardRho(BigInteger n)
    {
        BigInteger f(BigInteger x)
        {
            return (x * x + 1) % n;
        }

        BigInteger x = 2;
        BigInteger y = 2;
        BigInteger d = 1;
        while (d == 1)
        {
            x = f(x);
            y = f(f(y));
            d = BigInteger.GreatestCommonDivisor(BigInteger.Abs(x - y), n);
        }
        return d;
    }

    // Stage 5: Baby Step Giant Step Algorithm for Discrete Logarithm

    public static BigInteger BabyStepGiantStep(BigInteger baseValue, BigInteger target, BigInteger prime)
    {
        Dictionary<BigInteger, BigInteger> babySteps = new Dictionary<BigInteger, BigInteger>();
        BigInteger m = (int)Math.Ceiling(Math.Sqrt((double)(prime - 1)));

        // Precompute baby steps
        for (BigInteger j = 0; j < m; j++)
        {
            babySteps[BigInteger.ModPow(baseValue, j, prime)] = j;
        }

        // Giant step
        BigInteger invBaseM = ModInverse(baseValue, prime);
        BigInteger x = target;
        for (BigInteger i = 0; i < m; i++)
        {
            if (babySteps.ContainsKey(x))
            {
                return i * m + babySteps[x];
            }
            x = (x * invBaseM) % prime;
        }
        return -1; // No solution
    }

    // Stage 6: Chippola's Algorithm for Discrete Square Root

    public static BigInteger ChippolaAlgorithm(BigInteger a, BigInteger p)
    {
        if (LegendreSymbol(a, p) != 1)
        {
            return -1; // No solution
        }
        BigInteger s = p - 1;
        BigInteger t = 0;
        while (s % 2 == 0)
        {
            s /= 2;
            t++;
        }
        BigInteger u;
        if (t % 2 == 0)
        {
            u = 1;
        }
        else
        {
            u = 2;
            while (LegendreSymbol(u, p) != -1)
            {
                u++;
            }
        }
        BigInteger x = BigInteger.ModPow(a, (s + 1) / 2, p);
        BigInteger b = BigInteger.ModPow(a, s, p);
        BigInteger g = BigInteger.ModPow(u, s, p);
        BigInteger r = t;
        while (r > 0)
        {
            BigInteger m = 0;
            BigInteger z = b;
            while (z != 1)
            {
                z = (z * z) % p;
                m++;
                if (m >= r)
                {
                    return -1; // No solution
                }
            }
            if (m == 0)
            {
                return x;
            }
            BigInteger y = BigInteger.ModPow(g, BigInteger.Pow(2, (int)(r - m - 1)), p);
            x = (x * y) % p;
            g = BigInteger.ModPow(y, 2, p);
            b = (b * g) % p;
            r = m;
        }
        return -1; // No solution
    }

    // Stage 7: Miller-Rabin Primality Test

    public static bool MillerRabinTest(BigInteger n, int k)
    {
        if (n <= 1)
        {
            return false;
        }
        if (n <= 3)
        {
            return true;
        }
        if (n % 2 == 0)
        {
            return false;
        }

        BigInteger r = 0;
        BigInteger s = n - 1;
        while (s % 2 == 0)
        {
            r++;
            s /= 2;
        }

        Random random = new Random();
        for (int i = 0; i < k; i++)
        {
            BigInteger a = RandomBigInteger(2, n - 2, random);
            BigInteger x = BigInteger.ModPow(a, s, n);
            if (x == 1 || x == n - 1)
            {
                continue;
            }
            for (BigInteger j = 0; j < r - 1; j++)
            {
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                {
                    break;
                }
            }
            if (x != n - 1)
            {
                return false;
            }
        }
        return true;
    }

    public static BigInteger RandomBigInteger(BigInteger min, BigInteger max, Random random)
    {
        byte[] bytes = new byte[max.ToByteArray().Length];
        random.NextBytes(bytes);
        BigInteger value = new BigInteger(bytes);
        value = BigInteger.Abs(value % (max - min + 1)) + min;
        return value;
    }

    public static bool IsPrime(BigInteger n)
    {
        if (n <= 1)
        {
            return false;
        }
        if (n <= 3)
        {
            return true;
        }
        if (n % 2 == 0)
        {
            return false;
        }

        BigInteger k = 100; // Number of Miller-Rabin tests
        BigInteger t = n - 1;
        BigInteger s = 0;

        while (t % 2 == 0)
        {
            t /= 2;
            s++;
        }

        for (int i = 0; i < k; i++)
        {
            BigInteger a = RandomBigInteger(2, n - 2, new Random());
            BigInteger x = BigInteger.ModPow(a, t, n);
            if (x == 1 || x == n - 1)
            {
                continue;
            }
            for (BigInteger j = 0; j < s - 1; j++)
            {
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                {
                    break;
                }
            }
            if (x != n - 1)
            {
                return false;
            }
        }
        return true;
    }

    // Stage 8: RSA Cryptosystem

    public static (BigInteger, BigInteger) GenerateRsaKey(int bits)
    {
        BigInteger GeneratePrime(int bits)
        {
            Random random = new Random();
            BigInteger p;
            while (true)
            {
                p = RandomBigInteger(BigInteger.Pow(2, bits - 1), BigInteger.Pow(2, bits) - 1, random);
                if (IsPrime(p))
                {
                    return p;
                }
            }
        }

        BigInteger p = GeneratePrime(bits);
        BigInteger q = GeneratePrime(bits);
        BigInteger n = p * q;
        BigInteger phi = (p - 1) * (q - 1);

        BigInteger e;
        while (true)
        {
            e = RandomBigInteger(2, phi - 1, new Random());
            if (Gcd(e, phi) == 1)
            {
                break;
            }
        }

        BigInteger d = ModInverse(e, phi);
        return (n, e); // Public key
    }

    public static (BigInteger, BigInteger) GenerateRsaPrivateKey(BigInteger n, BigInteger e)
    {
        BigInteger phi = (n - 1) * (e - 1);
        BigInteger d = ModInverse(e, phi);
        return (n, d); // Private key
    }


    public static BigInteger RsaEncrypt(BigInteger message, (BigInteger, BigInteger) publicKey)
    {
        BigInteger n = publicKey.Item1;
        BigInteger e = publicKey.Item2;
        return BigInteger.ModPow(message, e, n);
    }

    public static BigInteger RsaDecrypt(BigInteger ciphertext, (BigInteger, BigInteger) privateKey)
    {
        BigInteger n = privateKey.Item1;
        BigInteger d = privateKey.Item2;
        return BigInteger.ModPow(ciphertext, d, n);
    }

    public static void Main()
    {
        // Example usage:

        // Stage 1
        Console.WriteLine("Stage 1: Euler Phi Function");
        Console.WriteLine(EulerPhi(30));
        Console.WriteLine("Stage 1: Möbius Function");
        Console.WriteLine(Mobius(10));
        Console.WriteLine("Stage 1: Least Common Multiple");
        Console.WriteLine(Lcm(12, 18));

        // Stage 2
        Console.WriteLine("Stage 2: Chinese Remainder Theorem");
        List<(BigInteger, BigInteger)> congruences = new List<(BigInteger, BigInteger)> { (2, 3), (3, 4), (2, 5) };
        Console.WriteLine(ChineseRemainderTheorem(congruences));

        // Stage 3
        Console.WriteLine("Stage 3: Legendre Symbol");
        Console.WriteLine(LegendreSymbol(5, 11));
        Console.WriteLine("Stage 3: Jacobi Symbol");
        Console.WriteLine(JacobiSymbol(7, 15));

        // Stage 4
        Console.WriteLine("Stage 4: Pollard's Rho Algorithm");
        Console.WriteLine(PollardRho(8051));

        // Stage 5
        Console.WriteLine("Stage 5: Baby Step Giant Step Algorithm");
        Console.WriteLine(BabyStepGiantStep(2, 11, 59));

        // Stage 6
        Console.WriteLine("Stage 6: Chippola's Algorithm");
        Console.WriteLine(ChippolaAlgorithm(223, 17));

        // Stage 7
        Console.WriteLine("Stage 7: Miller-Rabin Primality Test");
        Console.WriteLine(MillerRabinTest(13, 5));

        // Stage 8
        Console.WriteLine("Stage 8: RSA Cryptosystem");
        BigInteger n, e, d;
        (n, e) = GenerateRsaKey(1024);
        (n, d) = GenerateRsaPrivateKey(n, d);
        BigInteger message = 42;
        BigInteger encrypted = RsaEncrypt(message, (n, e));
        BigInteger decrypted = RsaDecrypt(encrypted, (n, d));
        Console.WriteLine($"Original Message: {message}");
        Console.WriteLine($"Encrypted Message: {encrypted}");
        Console.WriteLine($"Decrypted Message: {decrypted}");
    }
}
