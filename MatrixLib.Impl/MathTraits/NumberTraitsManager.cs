using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数值类型萃取管理器. 它是线程安全的.
    /// </summary>
    public sealed class NumberTraitsManager : INumberTraitsManager {
        /// <summary>Instance (实例).</summary>
        public static NumberTraitsManager Instance { get; } = new();

        /// <summary>添加后的 Hash.</summary>
        public int AddedHash { get; internal set; } = 0;

        /// <summary>Type map (类型的映射表).</summary>
        internal ConcurrentDictionary<Type, INumberTraitsDefine> TypeMap { get; } = new();

        /// <summary>
        /// Static create NumberTraitsManager.
        /// </summary>
        static NumberTraitsManager() {
            NumberTraitsUtil.TraitsManager = Instance;
        }

        /// <summary>
        /// 注册类型. 本方法调用成功后, 才能调用 GetDefine.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <param name="caller">调用者. 若该类型具有 IBaseMathCaller 系列接口时, 可空.</param>
        /// <returns>返回是否是首次添加. 重复添加时, 会返回 false.</returns>
        /// <exception cref="ArgumentNullException">请传递 caller 参数!</exception>
        /// <exception cref="NotSupportedException">caller 参数不支持该类型!</exception>
        public bool Register<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(IBaseMathCaller? caller = null) {
            return Register(caller, default(T)!);
        }

        /// <inheritdoc cref="Register{T}(IBaseMathCaller?)"/>
        /// <param name="instance">实例. 值类型时可空, 引用类型时建议传递 零值. 它为 null 时, 会尝试调用 <see cref="Activator.CreateInstance"/> 创建实例, 可能会有异常.</param>
        public bool Register<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(IBaseMathCaller? caller, T instance) {
            return RegisterCore(caller, instance);
        }

        /// <inheritdoc cref="Register{T}(IBaseMathCaller?, T)"/>
        private bool RegisterCore<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(IBaseMathCaller? caller, T instance) {
            if (TypeMap.ContainsKey(typeof(T))) return false;
            if (instance == null) {
                instance = Activator.CreateInstance<T>();
            }
            // Make.
            NumberTraitsDefine<T> define = TraitsProviderUtil.GetDefine(caller, instance);
            // Register.
            if (define is not null) {
                if (define.Zero is null) {
                    // 当 Zero 为 null 时, 设为 instance.
                    define.Zero = instance;
                }
                TypeMap.TryAdd(typeof(T), define);
                // 预热.
                if (true) {
                    Preheat<T>();
                }
                return true;
            }
            return false;
        }

        /*
                /// <summary>
                /// 为泛型类型(`containerType&lt;elementType&gt;`)执行注册.
                /// </summary>
                /// <param name="elementType">Element type (元素类型).</param>
                /// <param name="containerType">Container type (容器类型). 它是1个类型参数的泛型类型, 且需支持无参构造方法. e.g. `typeof(StructNumberBase&lt;&gt;)`.</param>
                /// <param name="callerType">Caller type (调用者类型). 它是1个类型参数的泛型类型, 且需支持无参构造方法, 还需实现 IBaseMathCaller 接口. e.g. `typeof(ComplexCaller&lt;&gt;)`.</param>
                /// <returns>返回是否是首次添加. 重复添加时, 会返回 false.</returns>
                /// <exception cref="ArgumentNullException">请传递 caller 参数!</exception>
                /// <exception cref="NotSupportedException">caller 参数不支持该类型!</exception>
        #if NET7_0_OR_GREATER
                [RequiresDynamicCode("Not support AOT. Use INumberTypeAction on AOT.")]
        #endif // NET7_0_OR_GREATER
                public bool RegisterGeneric([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type elementType,
                    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type containerType,
                    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? callerType = null,
                    object? instance = null
                    ) {
                    MethodInfo? methodT = typeof(NumberTraitsManager).GetMethod(nameof(RegisterCore), BindingFlags.NonPublic| BindingFlags.Instance);
                    if (methodT is null) {
                        throw new NotSupportedException(nameof(RegisterCore));
                    }
                    // IL2055	https://learn.microsoft.com/dotnet/core/deploying/trimming/trim-warnings/il2055 Using member 'System.Type.MakeGenericType(params Type[])' which has 'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. The native code for this instantiation might not be available at runtime.
                    Type containerTypeClosed = containerType.MakeGenericType(elementType);
                    if (instance is null) {
                        instance = Activator.CreateInstance(containerTypeClosed);
                        if (instance is null) {
                            throw new NotSupportedException(nameof(containerType));
                        }
                    }
                    MethodInfo method = methodT.MakeGenericMethod(containerTypeClosed);
                    // caller.
                    object callerObject;
                    if (callerType is null) {
                        callerObject = instance;
                    } else {
                        Type callerTypeClosed = callerType.MakeGenericType(elementType);
                        callerObject = Activator.CreateInstance(callerTypeClosed)!;
                    }
                    // Invoke.
                    object[] parameters = [callerObject, instance];
                    return (bool)method.Invoke(this, parameters)!;
                }
        */


        /// <summary>
        /// 根据 <see cref="Type"/> 注册类型. 本方法调用成功后, 才能调用 GetDefine.
        /// </summary>
        /// <param name="numberType">Number type (数值类型). 它需支持无参构造方法.</param>
        /// <param name="callerType">Caller type (调用者类型). 它需支持无参构造方法, 且需实现 IBaseMathCaller 系列接口. 它为 null 时, 会使用 numberType, 用于数值类型实现了IBaseMathCaller接口时.</param>
        /// <param name="instance">实例. 值类型时可空, 引用类型时建议传递 零值. 它为 null 时, 会尝试调用 <see cref="Activator.CreateInstance"/> 创建实例, 可能会有异常.</param>
        /// <returns>返回是否是首次添加. 重复添加时, 会返回 false.</returns>
        /// <exception cref="ArgumentNullException">请传递 caller 参数!</exception>
        /// <exception cref="NotSupportedException">caller 参数不支持该类型!</exception>
#if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "At methodT.MakeGenericMethod")]
#endif // NET5_0_OR_GREATER
        public bool RegisterType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type numberType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type? callerType = null,
            object? instance = null
            ) {
            MethodInfo? methodT = typeof(NumberTraitsManager).GetMethod(nameof(RegisterCore), BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodT is null) {
                throw new NotSupportedException(nameof(RegisterCore));
            }
            // IL2055	https://learn.microsoft.com/dotnet/core/deploying/trimming/trim-warnings/il2055 Using member 'System.Type.MakeGenericType(params Type[])' which has 'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. The native code for this instantiation might not be available at runtime.
            if (instance is null) {
                instance = Activator.CreateInstance(numberType);
                if (instance is null) {
                    throw new NotSupportedException(nameof(numberType));
                }
            }
            MethodInfo method = methodT.MakeGenericMethod(numberType);
            // caller.
            object callerObject;
            if (callerType is null) {
                callerObject = instance;
            } else {
                callerObject = Activator.CreateInstance(callerType)!;
            }
            // Invoke.
            object[] parameters = [callerObject, instance];
            return (bool)method.Invoke(this, parameters)!;
        }

        /// <summary>
        /// Preheat (预热).
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        private void Preheat<T>() {
            int hash = TraitsProviderUtil.Preheat<T>();
            AddedHash ^= hash;
        }

        public NumberTraitsDefine<T>? GetDefine<T>() {
            INumberTraitsDefine itf = TypeMap[typeof(T)];
            if (itf is NumberTraitsDefine<T> define) {
                return define;
            }
            return null;
        }

    }
}
