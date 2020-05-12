using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Infrastructure.Common.Exceptions
{
    /// <summary>
    /// 数据访问层异常类，用于封装业务逻辑层引发的异常，以供 UI 层抓取
    /// </summary>
    [Serializable]
    public class BusinessException : System.Exception
    {
        public BusinessException() { }

        public BusinessException(string message)
            : base(message) { }

        public BusinessException(string message, System.Exception inner)
            : base(message, inner) { }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
