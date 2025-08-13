#if NET9_0_OR_GREATER
#define ALLOWS_REF_STRUCT // C# 13 - ref struct interface; allows ref struct. https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13#ref-struct-interfaces
#endif // NET9_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.Extensions {
    /// <summary>
    /// Extensions of references (managed pointers) (引用(托管指针)的扩展).
    /// </summary>
    public static class RefExtensions {

        /// <inheritdoc cref="Unsafe.Add{T}(ref T, nint)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T At<T>(this ref T source, nint elementOffset)
            where T : struct // CS8337 The first parameter of a 'ref' extension method 'At' must be a value type or a generic type constrained to struct.
#if ALLOWS_REF_STRUCT
            , allows ref struct
#endif // ALLOWS_REF_STRUCT
            {
            return ref Unsafe.Add(ref source, elementOffset);
        }

        /// <summary>
        /// Adds 1 offset to the given reference.
        /// </summary>
        /// <typeparam name="T">The element type (元素的类型).</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The added.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Inc<T>(this ref T source)
            where T : struct
#if ALLOWS_REF_STRUCT
            , allows ref struct
#endif // ALLOWS_REF_STRUCT
            {
            return ref Unsafe.Add(ref source, 1);
        }

    }
}
