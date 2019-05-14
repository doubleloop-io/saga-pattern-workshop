using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SagaPattern.Infrastructure.Scheduler
{
    public class ThreadBasedScheduler : IScheduler
    {
        private readonly ConcurrentQueue<ScheduledTask> _pending = new ConcurrentQueue<ScheduledTask>();
        private readonly PairingHeap<ScheduledTask> _tasks = new PairingHeap<ScheduledTask>((x, y) => x.DueTime < y.DueTime);

        private readonly ITimeProvider _timeProvider;

        private readonly Thread _timerThread;
        private volatile bool _stop;

        public ThreadBasedScheduler(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;

            _timerThread = new Thread(DoTiming);
            _timerThread.IsBackground = true;
            _timerThread.Name = "Scheduler";
            _timerThread.Start();
        }

        public void Stop()
        {
            Dispose();
        }

        public void Schedule(TimeSpan after, Action<IScheduler, object> callback, object state)
        {
            _pending.Enqueue(new ScheduledTask(_timeProvider.Now.Add(after), callback, state));
        }

        private void DoTiming()
        {
            while (!_stop)
            {
                while (_pending.TryDequeue(out var task))
                {
                    _tasks.Add(task);
                }

                var processed = 0;

                while (_tasks.Count > 0 && _tasks.FindMin().DueTime <= _timeProvider.Now)
                {
                    processed += 1;
                    var scheduledTask = _tasks.DeleteMin();
                    scheduledTask.Action(this, scheduledTask.State);
                }

                if (processed == 0)
                {
                    Thread.Sleep(1);
                }
            }
        }

        public void Dispose()
        {
            _stop = true;
        }

        private struct ScheduledTask
        {
            public readonly DateTime DueTime;
            public readonly Action<IScheduler, object> Action;
            public readonly object State;

            public ScheduledTask(DateTime dueTime, Action<IScheduler, object> action, object state)
            {
                DueTime = dueTime;
                Action = action;
                State = state;
            }
        }

        private class SchedulePendingTasks
        {
        }

        private class ExecuteScheduledTasks
        {
        }
    }
}