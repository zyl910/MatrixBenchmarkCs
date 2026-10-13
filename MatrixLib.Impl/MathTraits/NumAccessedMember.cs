global using DAMAttribute = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
global using static MatrixLib.MathTraits.NumAccessedMember;

using System.Diagnostics.CodeAnalysis;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// <see cref="DynamicallyAccessedMemberTypes"/> of number types (数值类型的动态访问成员类型).
    /// </summary>
    /// <remarks>
    /// <para>Usage‌:</para>
    /// <code>
    /// global using DAMAttribute = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
    /// global using static MatrixLib.MathTraits.NumAccessedMember;
    /// </code>
    /// </remarks>
    public static class NumAccessedMember {
        /// <summary>
        /// Access number types (访问数值类型).
        /// </summary>
        /// <remarks>
        /// <para>Old: <c>public static void DoSomething&lt;[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T&gt;() {}</c></para>
        /// <para>New: <c>public static void DoSomething&lt;[DAM(ANum)]T&gt;() {}</c></para>
        /// </remarks>
        public const DynamicallyAccessedMemberTypes ANum = DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

    }
}
