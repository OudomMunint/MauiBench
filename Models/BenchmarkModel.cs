using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBench.Models
{
    public class BenchmarkModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Name
        {
            get
            {
                switch (TestNameValue)
                {
                    case TestName.Hashing:
                        return "Hashing";
                    case TestName.Encryption:
                        return "Encryption";
                    case TestName.Prime:
                        return "Prime Compute";
                    case TestName.MatrixMultiplication:
                        return "Matrix Multiplication";
                    case TestName.MemoryBandwidth:
                        return "Memory Bandwidth";
                    case TestName.Full:
                        return "Full Benchmark";
                    default:
                        return "Unknown";
                }
            }
        }

        public Type BenchmarkType { get; set; }

        public string? Result { get; set; }

        public TestName TestNameValue { get; set; }

        public enum Type
        {
            Partial,
            Full
        }

        public enum TestName
        {
            Hashing,
            Encryption,
            Prime,
            MatrixMultiplication,
            MemoryBandwidth,
            Full
        }
    }
}