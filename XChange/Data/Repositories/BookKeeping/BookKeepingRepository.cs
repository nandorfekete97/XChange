using Microsoft.EntityFrameworkCore;
using XChange.Data.context;
using XChange.Data.Entities;

namespace XChange.Data.Repositories.BookKeeping;

public class BookKeepingRepository : IBookKeepingRepository
{
    private XChangeContext _xChangeContext;

    public BookKeepingRepository(XChangeContext xChangeContext)
    {
        _xChangeContext = xChangeContext;
    }
    
    public async Task CreateAsync(BookKeepingEntity bookKeepingEntity)
    {
        _xChangeContext.BookKeepings.Add(bookKeepingEntity);
        await _xChangeContext.SaveChangesAsync();
    }

    public async Task<BookKeepingEntity> GetByIdAsync(int id)
    {
        return await _xChangeContext.BookKeepings.FirstOrDefaultAsync(keeping => keeping.Id == id);
    }

    public async Task DeleteAsync(BookKeepingEntity bookKeeping)
    {
        _xChangeContext.BookKeepings.Remove(bookKeeping);
        await _xChangeContext.SaveChangesAsync();
    }
}