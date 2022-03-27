using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public interface IJobQueue
    {
        void Push(Action job);
    }

    class JobQueue : IJobQueue
    {
        Queue<Action> _jobQueue = new Queue<Action>();
        object _jobQueueLock = new object();

        public void Push(Action job)
        {
            lock(_jobQueueLock)
                _jobQueue.Enqueue(job);
        }

        Action Pop()
        {
            lock(_jobQueueLock)
            {
                if (_jobQueue.Count == 0)
                    return null;
                return _jobQueue.Dequeue();
            }
        }
    }
}
