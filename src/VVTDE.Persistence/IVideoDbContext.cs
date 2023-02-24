using Microsoft.EntityFrameworkCore;
using VVTDE.Domain;

namespace VVTDE.Persistence;

public interface IVideoDbContext
{
    DbSet<Video> Videos { get; set; }
    int SaveChanges();
}