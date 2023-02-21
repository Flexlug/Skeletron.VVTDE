using Microsoft.EntityFrameworkCore;
using VVTDE.Domain;

namespace VVTDE.Persistence;

public interface IVideoDbContext
{
    DbSet<Video> Videos { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}