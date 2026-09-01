//#define ALLOW_CALLLER_TYPE

#if ALLOW_CALLLER_TYPE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// 调用者的类型萃取提供者. 若该类型是通过外部类型来提供 IBaseMathCaller 系列接口的, 则使用本类型来处理. 【注意】它对 AOT的兼容差.
    /// </summary>
    /// <remarks>
    /// <para>RegisterCaller 支持2种类型：</para>
    /// <para>- 封闭泛型类型.</para>
    /// <para>- 1个泛型参数的开放泛型类型. 泛型类型参数就是 T, 且封闭后的泛型类支持无参构造函数.</para>
    /// </remarks>
    public class CallerTypeTraitsProvider : INumberTraitsProvider {

        /// <summary>Caller list (调用者列表).</summary>
        internal List<
            Type> CallerList { get; private set; } = [];

        /// <summary>Caller list of work (工作中的调用者列表).</summary>
        private List<
            Type> CallerListWork { get; } = [];

        public bool FillDefine<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, T instance) {
            bool hasFill = false;
            List<Type> list = CallerList;
            lock (list) {
                foreach (var p in list) {
                    if (p is null) continue;
                    // IL2072	https://learn.microsoft.com/dotnet/core/deploying/trimming/trim-warnings/il2072		'callerType' argument does not satisfy 'DynamicallyAccessedMemberTypes.All' in call to 'CallerTypeTraitsProvider.FillDefineBy<T>(NumberTraitsDefine<T>, T, Type)'. The return value of method 'System.Collections.Generic.List<T>.Enumerator.Current.get' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
                    if (FillDefineBy(define, instance, p)) {
                        hasFill = true;
                    }
                }
            }
            return hasFill;
        }

        protected bool FillDefineBy<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, T instance,
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
            Type callerType) {
            bool hasFill = false;
            IBaseMathCaller? caller = null;
            // Try create.
            //if (callerType.isGen)
            try {
                if (callerType.ContainsGenericParameters) {
                    Type[] typeArguments = callerType.GenericTypeArguments;
                    if (typeArguments.Length != 1) {
                        return hasFill;
                    }
                    // IL2055	https://learn.microsoft.com/dotnet/core/deploying/trimming/trim-warnings/il2055	Call to 'System.Type.MakeGenericType(params Type[])' can not be statically analyzed. It's not possible to guarantee the availability of requirements of the generic type.	
                    // IL3050	https://learn.microsoft.com/dotnet/core/deploying/native-aot/warnings/il3050	Using member 'System.Type.MakeGenericType(params Type[])' which has 'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. The native code for this instantiation might not be available at runtime.	
                    Type type = callerType.MakeGenericType(typeof(T));
                    caller = Activator.CreateInstance(type) as IBaseMathCaller;
                } else {
                    caller = Activator.CreateInstance(callerType) as IBaseMathCaller;
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
            // Call.
            if (caller is not null) {
                if (FillDefineByObject(define, instance, caller)) {
                    hasFill = true;
                }
            }
            return hasFill;
        }

        protected bool FillDefineByObject<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
        T>(NumberTraitsDefine<T> define, T instance, IBaseMathCaller caller) {
            return TraitsProviderUtil.FillDefine(define, instance, caller);
        }

        public bool RegisterCaller(
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif // NET5_0_OR_GREATER
            Type callerType) {
            if (callerType == null) return false;
            if (typeof(IBaseMathCaller).IsAssignableFrom(callerType)) return false;
            if (callerType.IsValueType) {
                // OK.
            } else {
                bool hasAnyDefault = callerType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, Type.EmptyTypes, null) != null;
                if (!hasAnyDefault) {
                    throw new ArgumentException($"The type `{callerType.FullName}` must has parameterless constructor!");
                }
            }
            lock (CallerListWork) {
                if (CallerListWork.Contains(callerType)) return true;
                CallerListWork.Add(callerType);
                CallerList = [.. CallerListWork];
            }
            return true;
        }

    }
}
#endif // ALLOW_CALLLER_TYPE
