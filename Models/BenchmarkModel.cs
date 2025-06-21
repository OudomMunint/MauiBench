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

        public string TestName { get; set; }

        public Type BenchmarkType { get; set; }

        public string Result { get; set; }

        public enum Type
        {
            Partial,
            Full
        }
    }
}