using System.Threading;

using MasterData.Domain;

using Microsoft.EntityFrameworkCore;

namespace MasterData.Host.Endpoints
{
    public interface IDbFacade { }

    public interface IBasicOperations : IDbFacade
    {
        DatabaseContext Db { get; }

        void Add<T>(T item) where T : class => Db.Add(item);

        Task<T[]> Get<T>(CancellationToken cancellationToken) where T : class => Db.Set<T>().ToArrayAsync(cancellationToken);

        ValueTask<T?> Get<T>(int id, CancellationToken cancellationToken) where T : class => Db.Set<T>().FindAsync([id], cancellationToken);

        Task Save(CancellationToken cancellationToken) => Db.SaveChangesAsync(cancellationToken);
    }
}
