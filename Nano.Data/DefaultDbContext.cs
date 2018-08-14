using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nano.Eventing.Attributes;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Z.EntityFramework.Plus;

namespace Nano.Data
{
    /// <inheritdoc />
    public class DefaultDbContext : BaseDbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contextOptions">The <see cref="DbContextOptions"/>.</param>
        /// <param name="dataOptions">The <see cref="DataOptions"/>.</param>
        public DefaultDbContext(DbContextOptions contextOptions, DataOptions dataOptions)
            : base(contextOptions, dataOptions)
        {

        }

        /// <inheritdoc />
        public override int SaveChanges()
        {
            return this.SaveChanges(true);
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return this.SaveChangesAsync(acceptAllChangesOnSuccess).Result;
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await this.SaveChangesAsync(true, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var pendingEvents = this.GetPendingEntityEvents();

            this.SaveSoftDeletion();
            this.SaveAudit();
            
            return await base
                .SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
                .ContinueWith(async x =>
                {
                    if (x.IsFaulted)
                        return await x;

                    if (x.IsCanceled)
                        return await x;

                    var eventing = this.GetService<IEventing>();

                    if (eventing == null)
                        return await x;

                    foreach (var @event in pendingEvents)
                    {
                        await eventing
                            .PublishAsync(@event, @event.Type)
                            .ConfigureAwait(false);
                    }

                    return await x;
                }, cancellationToken)
                .Result;
        }

        private void SaveAudit()
        {
            if (!this.Options.UseAudit)
                return;

            var audit = new Audit();
            audit.PreSaveChanges(this);
            audit.Configuration.AutoSavePreAction?.Invoke(this, audit);
            audit.PostSaveChanges();
        }
        private void SaveSoftDeletion()
        {
            if (!this.Options.UseSoftDeletetion)
                return;

            this.ChangeTracker
                .Entries<IEntityDeletableSoft>()
                .Where(x => x.State == EntityState.Deleted)
                .ToList()
                .ForEach(x =>
                {
                    x.State = EntityState.Modified;
                    x.Entity.IsDeleted = DateTimeOffset.UtcNow.GetEpochTime();
                });
        }
        private IEnumerable<EntityEvent> GetPendingEntityEvents()
        {
            return this.ChangeTracker
                .Entries<IEntity>()
                .Where(x =>
                    x.Entity.GetType().IsTypeDef(typeof(IEntityIdentity<>)) && 
                    x.Entity.GetType().GetCustomAttributes<PublishAttribute>().Any() &&
                    (x.State == EntityState.Added || x.State == EntityState.Deleted))
                .Select(x =>
                {
                    var name = x.Entity.GetType().Name.Replace("Proxy", "");
                    var state = x.State.ToString();

                    switch (x.Entity)
                    {
                        case IEntityIdentity<Guid> guid:
                            return new EntityEvent(guid.Id, name, state);

                        case IEntityIdentity<dynamic> dynamic:
                            return new EntityEvent(dynamic.Id, name, state);

                        default:
                            return null;
                    }
                })
                .Where(x => x != null)
                .ToArray();
        }
    }
}