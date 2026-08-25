using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        /// <summary>调用者的类型萃取提供者.</summary>
        internal CallerTraitsProvider CallerProvider { get; } = new();

        /// <summary>添加后的 Hash.</summary>
        public int AddedHash { get; internal set; } = 0;

        /// <summary>Provider list (提供者列表).</summary>
        internal List<INumberTraitsProvider> ProviderList { get; } = [];

        /// <summary>Type map (类型的映射表).</summary>
        internal ConcurrentDictionary<Type, INumberTraitsDefine> TypeMap { get; } = new();

        /// <summary>
        /// Static create NumberTraitsManager.
        /// </summary>
        static NumberTraitsManager() {
            NumberTraitsUtil.TraitsManager = Instance;
        }

        /// <summary>
        /// Create NumberTraitsManager.
        /// </summary>
        public NumberTraitsManager() {
            RegisterProvider(new SelfTraitsProvider());
            //RegisterProvider(CallerProvider);
        }

        /// <summary>
        /// 添加类型. Add 成功后, 才能调用 GetDefine. 若返回false, 请检查是否已调用了 RegisterCaller, RegisterProvider .
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <returns>返回是否成功.</returns>
        public bool Add<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>() {
            return Add(default(T)!);
        }

        /// <inheritdoc cref="Add{T}()"/>
        /// <param name="instance">实例. 它为 null 时, 会尝试调用 <see cref="Activator.CreateInstance"/> 创建实例, 可能会有异常.</param>
        public bool Add<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(T instance) {
            if (TypeMap.ContainsKey(typeof(T))) return true;
            if (instance == null) {
                instance = Activator.CreateInstance<T>();
            }
            NumberTraitsDefine<T>? define = MakeDefine(instance);
            if (define is not null) {
                TypeMap.TryAdd(typeof(T), define);
                // 预热.
                if (true) {
                    int hash = 0;
                    var itf = NumberTraitsCacheV3<T>.NumberBase;
                    if (itf is not null) {
                        hash = itf.GetHashCode();
                    }
                    AddedHash ^= hash;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 注册提供者.
        /// </summary>
        /// <param name="provider">提供者.</param>
        /// <returns>返回是否成功.</returns>
        public bool RegisterProvider(INumberTraitsProvider provider) {
            if (provider == null) return false;
            lock(ProviderList) {
                if (ProviderList.Contains(provider)) return true;
                ProviderList.Add(provider);
            }
            return true;
        }

        public NumberTraitsDefine<T>? GetDefine<T>() {
            INumberTraitsDefine itf = TypeMap[typeof(T)];
            if (itf is NumberTraitsDefine<T> define) {
                return define;
            }
            return null;
        }

        /// <summary>
        /// 构造类型萃取定义.
        /// </summary>
        /// <typeparam name="T">Element type (元素类型).</typeparam>
        /// <returns>返回已构造的类型萃取项目. 失败时返回 null.</returns>
        internal NumberTraitsDefine<T>? MakeDefine<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(T instance) {
            NumberTraitsDefine<T> define = new();
            bool hasFill = false;
            List<INumberTraitsProvider> list;
            lock(ProviderList) {
                list = [.. ProviderList];
            }
            foreach (var p in list) {
                if (p is null) continue;
                if (p.FillDefine(define, instance)) {
                    hasFill = true;
                }
            }
            if (!hasFill) return null;
            return define;
        }
    }
}
