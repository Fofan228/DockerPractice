namespace Data.Repository.Interfaces;

public interface IRepositoryLink
{
    Task<Link?> GetLinkByIdAsync(long id);
    Task InsertAsync(Link link);
    Task<int> SaveAsync();
    void Delete(Link link);
    Task<Link?> GetFirstInInvertLinksAsync();
}