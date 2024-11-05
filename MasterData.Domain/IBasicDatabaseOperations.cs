using System.Threading;

using Microsoft.EntityFrameworkCore;

namespace MasterData.Domain
{
    public interface IDbFacade { }

    public interface IBasicDatabaseOperations : IDbFacade
    {
        DatabaseContext Db { get; }

        void Add<T>(T item) where T : class => Db.Add(item);

        Task<T[]> Get<T>(CancellationToken cancellationToken) where T : class => Db.Set<T>().ToArrayAsync(cancellationToken);

        ValueTask<T?> Get<T>(int id, CancellationToken cancellationToken) where T : class => Db.Set<T>().FindAsync([id], cancellationToken);

        Task Save(CancellationToken cancellationToken) => Db.SaveChangesAsync(cancellationToken);
    }
}
