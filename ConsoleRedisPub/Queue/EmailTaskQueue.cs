using ConsoleRedisPub.Queue.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleRedisPub.Queue
{
    public class EmailTaskQueue : IEmailTaskQueue
    {
        private ConcurrentQueue<EmailJobDTO> _jobs = new ConcurrentQueue<EmailJobDTO>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Adicionar(EmailJobDTO job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            _jobs.Enqueue(job);
            _signal.Release();
        }

        public async Task<EmailJobDTO> ObterAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _jobs.TryDequeue(out var job);

            return job;
        }
    }
}
