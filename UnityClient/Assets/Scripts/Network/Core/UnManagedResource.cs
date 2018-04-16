using System;

namespace NGE
{
    /// <summary>
    /// 非托管资源基类
    /// </summary>
    public abstract class UnManagedResource : IDisposable
    {
        private static long m_resource_count = 0;
        private static object m_resource_obj = new object();
        //析构函数自动生成 Finalize 方法和对基类的 Finalize 方法的调用.默认情况下,一个类是没有析构函数的,也就是说,对象被垃圾回收时不会被调用Finalize方法 
        public UnManagedResource()
        {
            lock (m_resource_obj)
            {
                if (0 == m_resource_count)
                {
                    //CoreSystem.StartUp();
                }
                m_resource_count++;
            }
        }

        ~UnManagedResource()
        {
            Dispose(false);
            lock (m_resource_obj)
            {
                m_resource_count--;
                if (0 == m_resource_count)
                {
                    //CoreSystem.ClearUp();
                }
            }
        }


        // 无法被客户直接调用 
        // 如果 disposing 是 true, 那么这个方法是被客户直接调用的,那么托管的,和非托管的资源都可以释放 
        // 如果 disposing 是 false, 那么函数是从垃圾回收器在调用Finalize时调用的,此时不应当引用其他托管对象所以,只能释放非托管资源 
        protected virtual void Dispose(bool disposing)
        {
            // 那么这个方法是被客户直接调用的,那么托管的,和非托管的资源都可以释放 
            if (disposing)
            {
                // 释放 托管资源 
                //OtherManagedObject.Dispose();
            }

            //释放非托管资源 
            //DoUnManagedObjectDispose();
        }

        //可以被客户直接调用 
        public void Dispose()
        {
            //必须以Dispose(true)方式调用,以true告诉Dispose(bool disposing)函数是被客户直接调用的 
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
