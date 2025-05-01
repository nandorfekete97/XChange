using XChange.Data.Entities;

namespace XChange.Data.Repositories.BookKeeping;

public interface IBookKeepingRepository
{
    public Task Create(BookKeepingEntity bookKeeping);
    public Task<BookKeepingEntity> GetById(int id);
    public Task DeleteById(int id);
}