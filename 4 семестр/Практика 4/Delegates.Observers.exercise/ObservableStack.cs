using System;
using System.Text;
using System.Collections.Generic;

namespace Delegates.Observers
{
    public class TestHandler
    {
        public StringBuilder Log = new StringBuilder();

        public void Initialize<T>(ObservableStack<T> stack)
        {
            stack.ObjectMoved += (sender, args) => Log.Append(args.ToString());
        }

        public string GetLog()
        {
            return Log.ToString();
        }
    }

    public delegate void StackEventHandler<T>(object sender, StackEventData<T> args);

    public class ObservableStack<T>
    {
        public event StackEventHandler<T> ObjectMoved;

        private readonly List<T> data = new List<T>();

        public void Push(T obj)
        {
            data.Add(obj);
            ObjectMoved(this, new StackEventData<T> { IsPushed = true, Value = obj });
        }

        public T Pop()
        {
            if (data.Count == 0)
                throw new InvalidOperationException();
            var result = data[data.Count - 1];
            ObjectMoved(this, new StackEventData<T> { IsPushed = false, Value = result });
            return result;
        }
    }
}