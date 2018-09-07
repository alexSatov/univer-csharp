using System;

namespace Memory.API
{
    public class APIObject : IDisposable
    {
        private readonly int resource;
        private bool isDisposed;

        public APIObject(int resource)
        {
            this.resource = resource;
            MagicAPI.Allocate(resource);
        }

        ~APIObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); //деструктор не будет вызываться
        }

        protected virtual void Dispose(bool fromDisposeMethod)
        {
            if (!isDisposed)
            {
                try
                {
                    MagicAPI.Free(resource);
                    isDisposed = true;
                }
                catch (ArgumentException)
                {
                }
            }
        }
    }
}
