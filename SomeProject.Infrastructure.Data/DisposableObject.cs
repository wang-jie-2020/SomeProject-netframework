using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Infrastructure.Data
{
    public class DisposableObject : IDisposable
    {
        /// <summary>
        /// 获取或设置一个值。该值指示资源已经被释放。
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 由终结器调用以释放资源。
        /// </summary>
        ~DisposableObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// 派生类中重写此方法时，需要释放派生类中额外使用的资源。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                // 清理托管资源
                // if (managedResource != null)
                // {
                //     managedResource.Dispose();
                //     managedResource = null;
                // }
            }

            // 清理非托管资源
            // if (nativeResource != IntPtr.Zero)
            // {
            //     Marshal.FreeHGlobal(nativeResource);
            //     nativeResource = IntPtr.Zero;
            // }

            // 标记已经被释放。
            _disposed = true;
        }
    }
}
