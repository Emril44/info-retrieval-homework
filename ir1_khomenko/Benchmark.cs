using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    internal class Benchmark
    {
        private DictionaryBuilder builder;

        [GlobalSetup]
        public void GlobalSetup()
        {
            builder = new DictionaryBuilder();
        }

        [Benchmark]
        public void BuildInvertedIndexBenchmark()
        {

        }

        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmark>();
        }
    }
}
