using MatrixLib.MathTraits.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// Register <see cref="NumberBaseProxy{T}"/> accepter.
    /// </summary>
    public class NumberBaseProxyRegisterAccepter: NumberTypeAccepter {

        /// <summary>Create NumberBaseProxyRegisterAccepter.</summary>
        public NumberBaseProxyRegisterAccepter(): base() {
        }

        /// <summary>Create NumberBaseProxyRegisterAccepter, has child, acceptTypeAction params.</summary>
        /// <inheritdoc/>
        public NumberBaseProxyRegisterAccepter(INumberTypeAccepter? child, TypeAction? acceptTypeAction = null) : base(child, acceptTypeAction) {
        }

        public override void AcceptINumberBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(object? userData = null, int thisDeep = 0) {
            NumberBaseProxy<T>.CallerRegister();
            base.AcceptINumberBase<T>(userData, thisDeep);
            if (!ChildNoNewType) {
                Child?.AcceptINumberBase<NumberBaseProxy<T>>(userData);
            }
        }

    }
}
