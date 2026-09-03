using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib.MathTraits {
    /// <summary>
    /// 数值类型萃取的全局配置.
    /// </summary>
    public static class NumberTraitsGlobal {

        /// <summary>是否已初始化.</summary>
        public static bool Inited { get; private set; } = false;

        /// <summary>初始化后的 Hash.</summary>
        public static int InitedHash { get; private set; } = 0;

        /// <summary>
        /// Static create NumberTraitsGlobal.
        /// </summary>
        static NumberTraitsGlobal() {
            InitedHash ^= NumberTraitsManager.Instance.GetHashCode();
        }

        /// <summary>
        /// 初始化.
        /// </summary>
        public static void Init() {
            if (Inited) return;
            Inited = true;
            // Preheat common numeric types (常用数值类型).
            // Init Others.
        }

    }
}
