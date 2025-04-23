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
    
    public async Task Create(BookKeepingEntity bookKeepingEntity)
    {
        _xChangeContext.BookKeepings.Add(bookKeepingEntity);
        await _xChangeContext.SaveChangesAsync();
    }

    public async Task<BookKeepingEntity> GetById(int id)
    {
        return await _xChangeContext.BookKeepings.FirstOrDefaultAsync(keeping => keeping.Id == id);
    }

    public async Task DeleteById(int bookKeepingId)
    {
        var bookKeepingToDelete = await _xChangeContext.BookKeepings.FirstOrDefaultAsync(entity => entity.Id == bookKeepingId);

        if (bookKeepingToDelete is not null)
        {
            _xChangeContext.BookKeepings.Remove(bookKeepingToDelete);
            await _xChangeContext.SaveChangesAsync();
        }
    }
}