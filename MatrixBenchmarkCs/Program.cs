using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using MatrixBenchmarkCs.MultiplyMatrix;
using MatrixLib;
using MatrixLib.MathTraits;
using MatrixLib.MathTraits.CallerNumbers;
using MatrixLib.MathTraits.Providers;
using MatrixLib.MathTraits.Proxies;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MatrixBenchmarkCs {
    internal class Program {

#if NET7_0_OR_GREATER
        interface IFoo {
            public string Report();
            public string ReportB();
            public static string ReportStatic() {
                return "IFoo.ReportStatic";
            }
            public static abstract string ReportStaticVirtual();
        }

        interface IFoo1: IFoo {
            //public new string Report() { // 虽然能编译通过. 但它会创建一个新的同名方法, 而不是改写.
            //    return "IFoo1.Report_new";
            //}
            string IFoo.Report() {
                return "IFoo1.Report";
            }
            string IFoo.ReportB() {
                return "IFoo1.ReportB";
            }
            public new static string ReportStatic() {
                return "IFoo1.ReportStatic_new";
            }
            //static string IFoo.ReportStatic() { // CS0539 'Program.IFoo1.ReportStatic()' in explicit interface declaration is not found among members of the interface that can be implemented
            //    return "IFoo1.ReportStatic";
            //}
            static string IFoo.ReportStaticVirtual() {
                return "IFoo1.ReportStaticVirtual";
            }
        }

        interface IFoo2 : IFoo1 {
            //public new string Report() {
            //    return "IFoo2.Report_new";
            //}
            string IFoo.Report() { // 成功修改了 IFoo1 的 Report 的实现.
                return "IFoo2.Report";
            }
            public new static string ReportStatic() {
                return "IFoo2.ReportStatic_new";
            }
            //static string IFoo.ReportStatic() {
            //    return "IFoo2.ReportStatic";
            //}
            static string IFoo.ReportStaticVirtual() {
                // 接口默认实现里, 仅 static 的可以通过类型名来调用（故可以根据类型名来指定谁的方法）, 无法通过泛型访问, 可以new来重新定义, 可看作普通静态方法; 而 static abstract、static virtual 需要通过泛型来调用, 无法通过类型名来调用, 且推荐用 显式实现来重新定义（IFoo.SayStaticCall）.
                return "IFoo2.ReportStaticVirtual. " + IFoo1.ReportStatic();
                //return "IFoo2.ReportStaticVirtual. " + IFoo1.ReportStaticVirtual(); // CS8926 A static virtual or abstract interface member can be accessed only on a type parameter.
            }
        }

        interface IFooB : IFoo {
            public static string StaticReportB() {
                return "IFooB.StaticReportB";
            }
            //string IFoo.ReportB() {
            //    return StaticReportB();
            //}
        }

        // `interface IFoo2B : IFoo2, IFooB` 中有 `IFoo.ReportB`:  接口 IFoo2B 不会报错, 但在使用时报错 (Foo : IFoo2B): 
        interface IFoo2B : IFoo2
            //, IFooB // 是否集继承IFooB, 无区别.
            {
            //string IFoo.ReportB() {  // 以覆写接口成员的实现为准.
            //    return IFooB.StaticReportB();
            //}
        }

        // class Foo : IFoo2, IFooB // CS8705 Interface member 'Program.IFoo.ReportB()' does not have a most specific implementation. Neither 'Program.IFoo1.Program.IFoo.ReportB()', nor 'Program.IFooB.Program.IFoo.ReportB()' are most specific.
        class Foo : IFoo2B {
            //void IFoo.Report() {
            //    //base(IFoo2).Say();
            //    //IFoo2.base.Say();
            //    //((IFoo2)this).Say();
            //    // Stack overflow. Repeated 32150 times
            //}

            // 如果 IFoo1/IFoo2 使用`public new string Report`, 且本类没有 `Report`. 会报错: CS0535 'Program.Foo' does not implement interface member 'Program.IFoo.Report()'
            // If has `Report` member: Stack overflow. Repeated 32150 times
            //public string Report() {
            //    //return base(IFoo2).Report(); // Error
            //    //return IFoo2.base.Report(); // Error
            //    return ((IFoo2)this).Report();
            //}
            public string Report2() {
                return ((IFoo2)this).Report(); // 在不存在同名方法时, 才能用这种办法转发接口的非静态默认实现.
            }

            public static string ReportStatic() {
                return "Foo-" + IFoo2.ReportStatic();
            }
        }

        //static string CallReportStatic<T>() where T : IFoo {
        //    return T.ReportStatic(); // 仅 static 的无法通过泛型访问. CS0704 Cannot do non-virtual member lookup in 'T' because it is a type parameter.
        //}

        static string CallReportStaticVirtual<T>() where T : IFoo {
            return T.ReportStaticVirtual();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void TestInterface() {
            TextWriter writer = Console.Out;
            Foo foo = new Foo();
            //writer.WriteLine("foo.Report(): " + foo.Report()); // Stack overflow. Repeated 32150 times
            writer.WriteLine("((IFoo)foo).Report(): " + ((IFoo)foo).Report());
            writer.WriteLine("((IFoo1)foo).Report(): " + ((IFoo1)foo).Report());
            writer.WriteLine("((IFoo2)foo).Report(): " + ((IFoo2)foo).Report());
            writer.WriteLine("CallReportStaticVirtual<Foo>(): " + CallReportStaticVirtual<Foo>());
            //writer.WriteLine("CallReportStaticVirtual<IFoo>(): " + CallReportStaticVirtual<IFoo>()); // CS8920 The interface 'Program.IFoo' cannot be used as type argument. Static member 'Program.IFoo.SayStatic()' does not have a most specific implementation in the interface.
            writer.WriteLine("CallReportStaticVirtual<IFoo1>(): " + CallReportStaticVirtual<IFoo1>());
            writer.WriteLine("CallReportStaticVirtual<IFoo2>(): " + CallReportStaticVirtual<IFoo2>());
            writer.WriteLine("Foo.ReportStatic(): " + Foo.ReportStatic());
            writer.WriteLine("IFoo.ReportStatic(): " + IFoo.ReportStatic());
            writer.WriteLine("IFoo1.ReportStatic(): " + IFoo1.ReportStatic());
            writer.WriteLine("IFoo2.ReportStatic(): " + IFoo2.ReportStatic());
            writer.WriteLine("((IFoo)foo).ReportB(): " + ((IFoo)foo).ReportB());
            // Output:
            //((IFoo)foo).Report(): IFoo2.Report
            //((IFoo1)foo).Report(): IFoo2.Report
            //((IFoo2)foo).Report(): IFoo2.Report
            //CallReportStaticVirtual<Foo>(): IFoo2.ReportStaticVirtual.IFoo1.ReportStatic_new
            //CallReportStaticVirtual<IFoo1>(): IFoo1.ReportStaticVirtual
            //CallReportStaticVirtual<IFoo2>(): IFoo2.ReportStaticVirtual.IFoo1.ReportStatic_new
            //Foo.ReportStatic(): Foo - IFoo2.ReportStatic_new
            //IFoo.ReportStatic(): IFoo.ReportStatic
            //IFoo1.ReportStatic(): IFoo1.ReportStatic_new
            //IFoo2.ReportStatic(): IFoo2.ReportStatic_new
        }
#endif // NET7_0_OR_GREATER

        static void Main(string[] args) {
            TextWriter writer = Console.Out;
            writer.WriteLine("MatrixBenchmarkCs");
            // init.
            NumberTraitsCache<NumberBaseProxy<Int64>>.Register();
            ZeroOfTypes<NumberBaseProxy<Int64>>.Register(default);
            // NumberTraitsManager.
            NumberTraitsBoot.Init();
            //NumberTraitsManager.Instance.Register<NumberBaseProxy<Int64>>(); // Build-in caller.
            //NumberTraitsManager.Instance.Register<NumberBaseProxy<Int64>>(new NumberBaseProxy<Int64>()); // Build-out caller.
            //NumberBaseProxy<Int64>.RegisterCaller(default(NumberBaseProxy<Int64>));
            //NumberBaseProxy<Int64>.CallerRegister();
            //NumberTraitsManager.Instance.RegisterGeneric(typeof(Int64), typeof(NumberBaseProxy<>));
            //NumberTraitsManager.Instance.RegisterType(typeof(NumberBaseProxy<Int64>));
            NumberTraitsBoot.AcceptTypesCommon(new NumberBaseProxyRegisterAccepter());
            //writer.WriteLine(NumberTraitsManager.Instance.GetTypes().ToList());
            writer.WriteLine("NumberTraitsManager.GetTypes: " + string.Join(", ", NumberTraitsManager.Instance.GetTypes().OrderBy(f => f.ToString())));
#if NET7_0_OR_GREATER
            try {
                TestInterface();
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
#endif // NET7_0_OR_GREATER
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
