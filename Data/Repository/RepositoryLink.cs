using Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository;

public class RepositoryLink : IRepositoryLink
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryLink(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Link?> GetLinkByIdAsync(long id)
    {
        return await _dbContext.Links.FindAsync(id);
    }

    public async Task InsertAsync(Link link)
    {
        await _dbContext.Links.AddAsync(link);
    }

    public void Delete(Link link)
    {
        _dbContext.Links.Remove(link);
    }

    public async Task<Link?> GetFirstInInvertLinksAsync()
    {
        return await _dbContext.Links.OrderByDescending(l => l.Id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}