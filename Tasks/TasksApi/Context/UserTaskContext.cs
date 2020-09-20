using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksApi.Model.Context
{
    public class UserTaskContext : DbContext
    {
        public UserTaskContext(DbContextOptions<UserTaskContext> options)
            : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.HasKey(e => e.UserId);
        //    });

        //    modelBuilder.Entity<User>()
        //        .HasMany(user => user.TaskItems)
        //        .WithOne(task => task.User)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<TaskItem>(entity =>
        //    {
        //        entity.HasKey(e => e.TaskItemId);
        //    });
        //}

        public DbSet<User> User { get; set; }
        public DbSet<TaskItem> TaskItem { get; set; }
    }
}
