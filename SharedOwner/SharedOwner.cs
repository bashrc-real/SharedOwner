using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedOwner
{
    public class SharedHandle<T> : object where T : IDisposable
    {
        private T m_object;
        private volatile int m_refCounter = 0;

        public static SharedHandle<T> MakeSharedHandle(Func<T> createSharedOwner)
        {
            var obj = new SharedHandle<T>(createSharedOwner());
            return obj;
        }

        public SharedHandle(T obj)
        {
            AddReference();
            m_object = obj;
        }

        public Handle<T> GetHandle()
        {
            if (m_refCounter == 0)
            {
                throw new ObjectDisposedException("SharedHandle disposed");
            }

            AddReference();
            return new Handle<T>(this);
        }

        /// <summary>
        /// To be used only for debugging purposes
        /// </summary>
        public int ReferenceCount {  get { return this.m_refCounter; } }

        internal void Decrement()
        {
            if (Interlocked.Decrement(ref m_refCounter) == 0)
            {
                m_object.Dispose();
                m_object = default(T);
            }
        }

        internal T GetInternalHandler() { return m_object; }

        private void AddReference()
        {
            Interlocked.Increment(ref m_refCounter);
        }

        ~SharedHandle()
        {
            m_object.Dispose();
        }
    }
}
