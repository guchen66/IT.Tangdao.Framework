using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.DaoTasks
{
    public class TimeWheel<T>
    {
        private int secondSlot = 0;

        private DateTime wheelTime
        { get { return new DateTime(1, 1, 1, 0, 0, secondSlot); } }

        private Dictionary<int, ConcurrentQueue<WheelTask<T>>> secondTaskQueue;

        public void Start()
        {
            //new Timer(Callback, null, 0, 1000);
            secondTaskQueue = new Dictionary<int, ConcurrentQueue<WheelTask<T>>>();
            Enumerable.Range(0, 60).ToList().ForEach(x =>
            {
                secondTaskQueue.Add(x, new ConcurrentQueue<WheelTask<T>>());
            });
        }

        public async Task AddTaskAsync(int second, T data, Func<T, Task> handler)
        {
            var handTime = wheelTime.AddSeconds(second);
            if (handTime.Second != wheelTime.Second)
                secondTaskQueue[handTime.Second].Enqueue(new WheelTask<T>(data, handler));
            else
                await handler(data);
        }

        private async void Callback(object o)
        {
            if (secondSlot != 59)
                secondSlot++;
            else
            {
                secondSlot = 0;
            }
            if (secondTaskQueue[secondSlot].IsEmpty)
                await ExecuteTask();
        }

        private async Task ExecuteTask()
        {
            if (secondTaskQueue[secondSlot].IsEmpty)
                while (secondTaskQueue[secondSlot].IsEmpty)
                    if (secondTaskQueue[secondSlot].TryDequeue(out WheelTask<T> task))
                        await task.Handler(task.Data);
        }
    }
}