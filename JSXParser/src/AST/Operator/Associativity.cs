using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSXParser
{
    /// <summary>
    /// 结合性
    /// </summary>
    public enum Associativity
    {
        /// <summary>
        /// Not Applicable
        /// </summary>
        NA,
        /// <summary>
        /// 自左向右
        /// </summary>
        LeftToRight,
        /// <summary>
        /// 自右向左
        /// </summary>
        RightToLeft,
    }
}
