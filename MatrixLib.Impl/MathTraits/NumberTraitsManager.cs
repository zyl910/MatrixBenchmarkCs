using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        /// 添加类型. Add 成功后, 才能调用 GetDefine.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <param name="caller">调用者. 若该类型具有 IBaseMathCaller 系列接口时, 可空.</param>
        /// <returns>返回是否是首次添加. 重复添加时, 会返回 false.</returns>
        /// <exception cref="ArgumentNullException">请传递 caller 参数!</exception>
        /// <exception cref="NotSupportedException">caller 参数不支持该类型!</exception>
        public bool Add<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(IBaseMathCaller? caller = null) {
            return Add(caller, default(T)!);
        }

        /// <inheritdoc cref="Add{T}(IBaseMathCaller?)"/>
        /// <param name="instance">实例. 值类型时可空, 引用类型时建议传递 零值. 它为 null 时, 会尝试调用 <see cref="Activator.CreateInstance"/> 创建实例, 可能会有异常.</param>
        public bool Add<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif // NET5_0_OR_GREATER
        T>(IBaseMathCaller? caller, T instance) {
            if (TypeMap.ContainsKey(typeof(T))) return false;
            if (instance == null) {
                instance = Activator.CreateInstance<T>();
            }
            // Make.
            NumberTraitsDefine<T> define = TraitsProviderUtil.GetDefine(caller, instance);
            // Add.
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
