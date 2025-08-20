using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireHangfire.JobLibrary
{
    public interface IPrintingJobs
    {
        Task PrintMessageAsync(string message);
    }
}
