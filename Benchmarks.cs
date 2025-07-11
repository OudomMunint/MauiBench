﻿using MauiBench.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MauiBench
{
    public class Benchmarks
    {
        public class HashingBenchmark : IDisposable
        {
            private int N;
            private byte[]? data;
            private readonly SHA256 sha256 = SHA256.Create();
            private readonly SHA512 sha512 = SHA512.Create();
            private readonly MD5 md5 = MD5.Create();

            public HashingBenchmark()
            {
                if (Debugger.IsAttached)
                {
                    N = 500000000;
                }
                else
                {
                    N = 2000000000;
                }
                data = new byte[N];
                new Random(42).NextBytes(data);
            }

            public byte[] Sha256()
            {
                if (data == null)
                {
                    throw new InvalidOperationException("Data buffer is not initialized.");
                }
                return sha256.ComputeHash(data);
            }

            public byte[] Sha512()
            {
                if (data == null)
                {
                    throw new InvalidOperationException("Data buffer is not initialized.");
                }
                return sha512.ComputeHash(data);
            }

            public byte[] Md5()
            {
                if (data == null)
                {
                    throw new InvalidOperationException("Data buffer is not initialized.");
                }
                return md5.ComputeHash(data);
            }

            public int CombinedHashingExport()
            {
                System.Diagnostics.Debug.WriteLine($"Running Hash on SHA256, SHA512, MD5... Hashing {N / 1_000_000_000} GB...");
                Stopwatch stopwatch = Stopwatch.StartNew();

                Sha256();
                Sha512();
                Md5();

                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine($"Hashing completed in {stopwatch.ElapsedMilliseconds} ms.");

                Dispose();

                long score = (long)(4100.0 / stopwatch.ElapsedMilliseconds * 100);
                long result = score;
                return (int)result;
            }

            public void Dispose()
            {
                data = null;
                N = 0;
                sha256.Dispose();
                sha512.Dispose();
                md5.Dispose();
                GCHelper.CleanUp();
            }
        }

        public class EncryptionBenchmark : IDisposable
        {
            private long TotalSize;
            private int ChunkSize = 100_000_000; // 100MB per operation
            private int Iterations;
            private byte[]? dataChunk;
            private byte[] key;
            private byte[] iv;
            private Aes aes;

            public EncryptionBenchmark()
            {
                if (Debugger.IsAttached)
                {
                    TotalSize = 2L * 1_000_000_000; // 1GB
                }
                else
                {
                    TotalSize = 10L * 1_000_000_000; // 16GB
                }
                Iterations = (int)(TotalSize / ChunkSize);
                aes = Aes.Create();
                aes.KeySize = 256;
                aes.GenerateKey();
                aes.GenerateIV();

                key = aes.Key;
                iv = aes.IV;

                dataChunk = new byte[ChunkSize];
                new Random().NextBytes(dataChunk);
            }

            public byte[] AesEncrypt(byte[] data)
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "Data cannot be null.");
                }

                using var encryptor = aes.CreateEncryptor(key, iv);
                return encryptor.TransformFinalBlock(data, 0, data.Length);
            }

            public byte[] AesDecrypt(byte[] encryptedData)
            {
                if (encryptedData == null)
                {
                    throw new ArgumentNullException(nameof(encryptedData), "Encrypted data cannot be null.");
                }

                using var decryptor = aes.CreateDecryptor(key, iv);
                return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            }

            public int RunEncryptBenchmark()
            {
                if (dataChunk == null)
                {
                    throw new InvalidOperationException("Data chunk is not initialized.");
                }

                int threadCount = Environment.ProcessorCount;
                System.Diagnostics.Debug.WriteLine($"Running AES-256 Encryption... processing {TotalSize / 1_000_000_000} GB with {threadCount} threads...");

                Stopwatch stopwatch = Stopwatch.StartNew();

                Parallel.For(0, threadCount, _ =>
                {
                    for (int i = 0; i < Iterations / threadCount; i++)
                    {
                        byte[] encrypted = AesEncrypt(dataChunk);
                        byte[] decrypted = AesDecrypt(encrypted);
                    }
                });

                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine($"Encryption completed in {stopwatch.ElapsedMilliseconds} ms.");

                Dispose();

                long score = (long)(1500.0 / stopwatch.ElapsedMilliseconds * 100);
                long result = score;
                return (int)result;
            }

            public void Dispose()
            {
                aes.Dispose();
                aes.Clear();
                dataChunk = null;
                ChunkSize = 0;
                key = Array.Empty<byte>();
                iv = Array.Empty<byte>();
                TotalSize = 0;
                GCHelper.CleanUp();
            }
        }

        public class CPUBenchmark
        {
            public int CpuPrimeCompute()
            {
                int iterations;

                if (Debugger.IsAttached)
                {
                    iterations = 100_000_000;
                }
                else
                {
                    iterations = 400_000_000; // Default value if not debugging
                }

                int taskCount = Environment.ProcessorCount;
                int iterationsPerThread = iterations / taskCount;

                System.Diagnostics.Debug.WriteLine($"Running Prime Compute with {taskCount} threads...");

                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = taskCount
                };

                Stopwatch stopwatch = Stopwatch.StartNew();

                Parallel.For(0, taskCount, options, _ =>
                {
                    ComputePrimes(iterationsPerThread);
                });

                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine($"Prime compute completed in {stopwatch.ElapsedMilliseconds} ms.");

                long score = (long)(6100.0 / stopwatch.ElapsedMilliseconds * 100);
                long result = score;
                return (int)result;
            }

            private static int ComputePrimes(int limit)
            {
                int count = 0;
                for (int i = 2; i < limit; i++)
                {
                    if (IsPrime(i))
                        count++;
                }
                return count;
            }

            private static bool IsPrime(int number)
            {
                if (number < 2) return false;
                if (number % 2 == 0 && number != 2) return false;
                for (int i = 3; i * i <= number; i += 2)
                {
                    if (number % i == 0)
                        return false;
                }
                return true;
            }
        }

        public class MatrixMultiplicationBenchmark
        {
            private readonly int N; // Matrix size
            private readonly double[,] matrixA;
            private readonly double[,] matrixB;
            private readonly double[,] result;

            public MatrixMultiplicationBenchmark()
            {
                if (Debugger.IsAttached)
                {
                    N = 1024;
                }
                else
                {
                    N = 2048;
                }
                matrixA = new double[N, N];
                matrixB = new double[N, N];
                result = new double[N, N];

                Random random = new(42);
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        matrixA[i, j] = random.NextDouble() * 100;
                        matrixB[i, j] = random.NextDouble() * 100;
                    }
                }
            }

            public int MultiplyMatrix()
            {
                System.Diagnostics.Debug.WriteLine($"Running Matrix Multiplication with {Environment.ProcessorCount} threads...");
                Stopwatch stopwatch = Stopwatch.StartNew();

                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                Parallel.For(0, N, options, i =>
                {
                    for (int j = 0; j < N; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < N; k++)
                        {
                            sum += matrixA[i, k] * matrixB[k, j];
                        }
                        result[i, j] = sum;
                    }
                });

                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine($"Matrix multiplication completed in {stopwatch.ElapsedMilliseconds} ms.");

                long score = (long)(6700.0 / stopwatch.ElapsedMilliseconds * 100);
                long result2 = score;
                return (int)result2;
            }
        }

        // WIP - MMUL with SIMD
        public class MatrixMultiplicationBenchmarkSIMD
        {
            private readonly int N;
            private readonly int VectorSize;
            private readonly double[] matrixA;
            private readonly double[] matrixB_T;
            private readonly double[] result;

            public MatrixMultiplicationBenchmarkSIMD()
            {
                N = Debugger.IsAttached ? 1024 : 2048;
                VectorSize = 2; // usually 2 (128-bit) or 4 (256-bit)

                // Pad N to next multiple of VectorSize
                if (N % VectorSize != 0)
                    N += VectorSize - (N % VectorSize);

                matrixA = new double[N * N];
                matrixB_T = new double[N * N];
                result = new double[N * N];

                var random = new Random(42);
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        double value = random.NextDouble() * 100;
                        matrixA[i * N + j] = value;
                        matrixB_T[j * N + i] = value; // Transposed for cache-friendly SIMD
                    }
                }
            }

            public int MultiplyMatrix()
            {
                System.Diagnostics.Debug.WriteLine($"Running SIMD MMUL with N={N} and VectorSize={VectorSize}, Threads={Environment.ProcessorCount}...");
                Stopwatch stopwatch = Stopwatch.StartNew();

                Parallel.For(0, N, i =>
                {
                    for (int j = 0; j < N; j++)
                    {
                        double sum = 0;

                        int k = 0;
                        for (; k <= N - VectorSize; k += VectorSize)
                        {
                            var va = new Vector<double>(matrixA, i * N + k);
                            var vb = new Vector<double>(matrixB_T, j * N + k);
                            sum += Vector.Dot(va, vb);
                        }

                        result[i * N + j] = sum;
                    }
                });

                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine($"SIMD MMUL completed in {stopwatch.ElapsedMilliseconds} ms.");

                long score = (long)(6700.0 / stopwatch.ElapsedMilliseconds * 100);
                long results = score;
                return (int)results;
            }
        }

        // WIP
        public class MemoryBenchmark : IDisposable
        {
            public string MTMemBandwidth()
            {
                uint[]? data = new uint[10000000 * 32];
                List<(long Sum, double Bandwidth)> AllResults = new();
                (long Sum, double Bandwidth) BestResult;

                for (int j = 0; j < 15; j++)
                {
                    long totalSum = 0;
                    var sw = Stopwatch.StartNew();
                    int chunkSize = data.Length / Environment.ProcessorCount;
                    object lockObj = new();

                    Parallel.For(0, Environment.ProcessorCount, threadId =>
                    {
                        int start = threadId * chunkSize;
                        int end = (threadId == Environment.ProcessorCount - 1) ? data.Length : start + chunkSize;
                        uint localSum = 0;

                        for (int i = start; i < end; i += 64)
                        {
                            localSum += data[i] + data[i + 16] + data[i + 32] + data[i + 48];
                        }

                        lock (lockObj)
                        {
                            totalSum += localSum;
                        }
                    });

                    sw.Stop();
                    long dataSize = data.Length * 4;
                    double bandwidth = dataSize / sw.Elapsed.TotalSeconds / (1024 * 1024 * 1024);
                    Dispose();

                    //System.Diagnostics.Debug.WriteLine("{1:0.000} GB/s", totalSum, bandwidth);
                    AllResults.Add((totalSum, bandwidth));
                }

                BestResult = AllResults.OrderByDescending(x => x.Bandwidth).First(); // Sort for highest
                System.Diagnostics.Debug.WriteLine($"Memory Bandwidth: {BestResult.Bandwidth:0.000} GB/s");

                string benchResult = $"{BestResult.Bandwidth:0.0} GB/s";
                return benchResult;
            }

            public void Dispose()
            {
                GCHelper.CleanUp();
            }
        }

        public class CombinedBenchmark
        {
            public long RunAllBenchmarks()
            {
                var encBenchmark = new EncryptionBenchmark();
                var hashBenchmark = new HashingBenchmark();
                var cpuBenchmark = new CPUBenchmark();
                var matrixBenchmark = new MatrixMultiplicationBenchmark();
                long results = hashBenchmark.CombinedHashingExport() +
                                encBenchmark.RunEncryptBenchmark() +
                                cpuBenchmark.CpuPrimeCompute() +
                                matrixBenchmark.MultiplyMatrix();
                var totalScore = results;
                System.Diagnostics.Debug.WriteLine($"Total Score: {totalScore:F2} points");
                encBenchmark.Dispose();
                hashBenchmark.Dispose();
                return results;
            }
        }
    }
}