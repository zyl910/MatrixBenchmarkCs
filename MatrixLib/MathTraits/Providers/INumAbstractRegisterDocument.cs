using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixLib.MathTraits.Providers {
    /// <summary>
    /// Document of IBaseMathCallerRegister.
    /// </summary>
    public interface IBaseMathCallerRegisterDocument {

        /// <summary>
        /// 注册调用者. 它会调用 `NumberTraitsManager.Instance.Register`.
        /// </summary>
        /// <returns>返回是否是首次添加. 重复添加时, 会返回 false.</returns>
        /// <exception cref="NotSupportedException">不支持该类型!</exception>
        public bool CallerRegister();

    }
}
