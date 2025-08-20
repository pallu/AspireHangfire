using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireHangfire.JobLibrary
{
    public class PrintingJobs(ILogger<PrintingJobs> logger) : IPrintingJobs
    {
        public Task PrintMessageAsync(string message)
        {
            logger.LogInformation("Hangfire Job Executing: {Message}", message);
            // In a real-world scenario, you would have your actual job logic here.
            return Task.CompletedTask;
        }
    }
}
