using XChange.Data.Entities;

namespace XChange.Data.Repositories.BookKeeping;

public interface IBookKeepingRepository
{
    public Task CreateAsync(BookKeepingEntity bookKeeping);
    public Task<BookKeepingEntity> GetByIdAsync(int id);
    public Task DeleteAsync(BookKeepingEntity bookKeeping);
}