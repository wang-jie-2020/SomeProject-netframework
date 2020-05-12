using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Infrastructure.Common.Exceptions
{
    /// <summary>
    /// 数据访问层异常类，用于封装数据访问层引发的异常，以供 业务逻辑层 抓取
    /// </summary>
    public class DataAccessException : System.Exception
    {
        public DataAccessException() { }

        public DataAccessException(string message)
            : base(message) { }

        public DataAccessException(string message, System.Exception inner)
            : base(message, inner) { }

        protected DataAccessException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
