using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiPad.Utils
{
    internal class BackoffManager
    {
        private readonly TimeSpan _backoff;
        private AsyncManualResetEvent mre = new AsyncManualResetEvent(false);
        int allocId = 0;
        public BackoffManager(TimeSpan delay)
        {
            _backoff = delay;
        }
        public void ExecuteTask(Func<ValueTask> a)
        {
            Task.Run(async () =>
            {
                int cid = allocId++;
                mre.Set();
                mre.Reset();
                await Task.WhenAny(Task.Delay(_backoff), mre.WaitAsync());
                if (allocId != cid + 1) return;
                await a();
            });
        }
    }
}
