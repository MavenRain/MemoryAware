using System;

namespace Core.CSharp.Interface
{
    public interface IMemoryWatchable
    {
	    event EventHandler<object> ApplicationMemoryCriticalLimitReached;
    }
}
