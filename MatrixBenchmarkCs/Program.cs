using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using MatrixBenchmarkCs.MultiplyMatrix;
using MatrixLib;
using MatrixLib.MathTraits;
using MatrixLib.MathTraits.CallerNumbers;
using MatrixLib.MathTraits.Providers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

namespace MatrixBenchmarkCs {
    internal class Program {

            private class LocalRecvTye : IRecvNumberType {
                public static LocalRecvTye Instance { get; } = new ();
                public void RecvType<
#if NET5_0_OR_GREATER
                    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
                T>()
#if NET9_0_OR_GREATER
                    where T : allows ref struct
#endif // NET9_0_OR_GREATER
                {
                    //NumberTraitsManager.Instance.Add<StructNumberBase<T>>();
                }
            };

        static void Main(string[] args) {
            TextWriter writer = Console.Out;
            writer.WriteLine("MatrixBenchmarkCs");
            // init.
            NumberTraitsCache<StructNumberBase<Int64>>.Register();
            ZeroOfTypes<StructNumberBase<Int64>>.Register(default);
            // NumberTraitsManager.
            NumberTraitsGlobal.Init();
            //NumberTraitsManager.Instance.Add<StructNumberBase<Int64>>(); // Build-in caller.
            //NumberTraitsManager.Instance.Add<StructNumberBase<Int64>>(new StructNumberBase<Int64>()); // Build-out caller.
            NumberTraitsGlobal.SendTypesCommon(LocalRecvTye.Instance);
            // benchmarkMode
            // 0: Benchmark all with my BenchmarkMain.
            // 1: Benchmark all with BenchmarkDotNet.
            // 2: Benchmark item with BenchmarkDotNet.
            // 3: Running special method (AloneTest).
            int benchmarkMode = 1;
            if (args.Length >= 1) {
                if (!int.TryParse(args[0], out benchmarkMode)) {
                    benchmarkMode = 1;
                }
            }
            MatrixLibEnvironment.Init();
            try {
                writer.WriteLine("MatrixMath.SupportedInstructionSets:\t{0}", MatrixMath.SupportedInstructionSets);
            } catch (Exception ex) {
                writer.WriteLine(ex.ToString());
            }
            //MklDemo.Call(writer);
            if (benchmarkMode == 3) {
                AloneTestUtil.AloneTestByCommand(writer, args);
            } else if (benchmarkMode > 0) {
                Architecture architecture = RuntimeInformation.OSArchitecture;
                var config = DefaultConfig.Instance;
                if (architecture == Architecture.X86 || architecture == Architecture.X64) {
                    config = config.AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(maxDepth: 3, printSource: true, printInstructionAddresses: true, exportGithubMarkdown: true, exportHtml: true)));
                } else {
                    // Message: Arm64 is not supported (Iced library limitation)
                }
                config = config.AddJob(Job.MediumRun
                    //.WithToolchain(InProcessEmitToolchain.Instance)
                    //.WithId("InProcess")
                    );
                if (benchmarkMode >= 2) {
                    var summary = BenchmarkRunner.Run<MatrixNMultiplyBenchmark_Int32>(config);
                    writer.WriteLine(summary);
                } else {
                    var summary = BenchmarkRunner.Run(typeof(MatrixNMultiplyBenchmark_Int32).Assembly, config);
                    writer.WriteLine("Length={0}, {1}", summary.Length, summary);
                }
            } else {
                string indent = "";
                writer.WriteLine();
                BenchmarkUtil.OutputEnvironment(writer, indent);
                writer.WriteLine();
                BenchmarkUtil.ParseCommand(args);
                BenchmarkMain.RunBenchmark(writer, indent);
                writer.WriteLine();
                AloneTestUtil.AloneTestByCommand(writer, args);
            }
            //Console.ReadLine();
        }
    }
}
