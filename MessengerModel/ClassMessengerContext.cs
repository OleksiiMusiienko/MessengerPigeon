using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MessengerModel;
using System.IO;


namespace MessengerModel
{
    
        class MessengerContext : DbContext
        {
            static DbContextOptions<MessengerContext> _options;

            static MessengerContext()
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");
                var config = builder.Build();
                string connectionString = config.GetConnectionString("DefaultConnection");

                var optionsBuilder = new DbContextOptionsBuilder<MessengerContext>();
                _options = optionsBuilder.UseSqlServer(connectionString).Options;
            }

            public MessengerContext()
                : base(_options)
            {
                //Database.EnsureCreated(); //при выполнении миграции этот метод вызывает ошибку
            }

            public DbSet<User> Users { get; set; }
            public DbSet<Message> Messages { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // метод UseLazyLoadingProxies() делает доступной ленивую загрузку.
                optionsBuilder.UseLazyLoadingProxies();
            }
        }
}


