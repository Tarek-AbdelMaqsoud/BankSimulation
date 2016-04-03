using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Users;
using Data.Roles;
using Data.UsersQueues;
using Data.Queues;

namespace Data.Context
{
    public class BankSimulationContext : DbContext
    {
        public DbSet<UsersDb> Users { get; set; }
        public DbSet<RoleDb> Roles { get; set; }
        public DbSet<UsersQueuesDb> UsersQueues { get; set; }
        public DbSet<QueueDb> Queues { get; set; }

        public BankSimulationContext()
            : base("BankSimulationContextDatabaseConnectionName")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BankSimulationContext>());

            //Database.SetInitializer<BankSimulationContext>(null);
        }

        public BankSimulationContext(string connectionStringName)
            : base(connectionStringName)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<BankSimulationContext>());
            //Database.SetInitializer<BankSimulationContext>(null);
        }

        public static void SetDataContext(string databaseConnectionName)
        {
            System.Web.HttpContext.Current.Items["BankSimulationContextDatabaseConnectionName"] = databaseConnectionName;
        }

        // Automatically add the times the entity got created/modified
        public override int SaveChanges()
        {
            string tempInfo = String.Empty;
            try
            {
                try
                {
                    var entries = ChangeTracker.Entries().ToList();
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (entries[i].State == EntityState.Unchanged || entries[i].State == EntityState.Detached || entries[i].State == EntityState.Deleted) continue;

                        var hasInterfaceInheritDb = entries[i].Entity as InheritDb;
                        if (hasInterfaceInheritDb == null) continue;

                        if (entries[i].State == EntityState.Added)
                        {
                            var created = entries[i].Property("Created");
                            if (created != null)
                            {
                                created.CurrentValue = DateTime.Now;
                            }
                        }
                        if (entries[i].State == EntityState.Modified)
                        {
                            var modified = entries[i].Property("LastModified");
                            if (modified != null)
                            {
                                modified.CurrentValue = DateTime.Now;
                            }
                        }
                    }
                }
                catch (System.Exception dbEx)
                {
                }
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string additionalInfo = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        additionalInfo += "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage + "";
                    }
                }
                return 0;
            }

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }


    }
}
