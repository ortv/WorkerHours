using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkerHours.Models;

namespace WorkerHours.Data
{
    public class WorkerHoursContext : DbContext
    {
        public WorkerHoursContext (DbContextOptions<WorkerHoursContext> options)
            : base(options)
        {
        }

        public DbSet<WorkerHours.Models.Worker> Worker { get; set; } = default!;
    }
}
