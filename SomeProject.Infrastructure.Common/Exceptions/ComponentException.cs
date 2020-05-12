using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Infrastructure.Common.Exceptions
{
    /// <summary>
    /// 组件异常类
    /// </summary>
    [Serializable]
    public class ComponentException : System.Exception
    {
        public ComponentException() { }

        public ComponentException(string message)
            : base(message) { }

        public ComponentException(string message, System.Exception inner)
            : base(message, inner) { }

        protected ComponentException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
