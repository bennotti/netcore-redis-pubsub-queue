using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleRedisPub.Queue.Interface
{
    public interface IEmailTaskQueue
    {
        void Adicionar(EmailJobDTO job);

        Task<EmailJobDTO> ObterAsync(CancellationToken cancellationToken);
    }
}
