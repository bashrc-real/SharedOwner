using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedOwner
{
    /// <summary>
    /// Transparent handle for managing ref counting for shared resources
    /// Do not store the object retrieved from this class after calling dispose on the Handle object 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Handle<T> : IDisposable where T : IDisposable
    {
        private SharedHandle<T> m_handle;
        bool disposed = false;
        internal Handle(SharedHandle<T> handle) { m_handle = handle; }

        public void Dispose()
        {
            if (!disposed)
            {
                m_handle.Decrement();
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        ~Handle()
        {
            if (!disposed)
            {
                m_handle.Decrement();
            }
        }

        public static implicit operator T(Handle<T> obj)
        {
            return obj.m_handle.GetInternalHandler();
        }
    }
}
