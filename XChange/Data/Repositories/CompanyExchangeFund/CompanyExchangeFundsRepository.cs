using Microsoft.EntityFrameworkCore;
using XChange.Data.context;
using XChange.Data.Entities;

namespace XChange.Data.Repositories.CompanyExchangeFunds;

public class CompanyExchangeFundsRepository : ICompanyExchangeFundsRepository
{
    private XChangeContext _xChangeContext;

    public CompanyExchangeFundsRepository(XChangeContext xChangeContext)
    {
        _xChangeContext = xChangeContext;
    }

    public async Task<CompanyExchangeFundEntity> Get()
    {
        return await _xChangeContext.CompanyExchangeFunds.FirstAsync();
    }

    public async Task<CompanyExchangeFundEntity> GetByIdAsync(int id)
    {
        return await _xChangeContext.CompanyExchangeFunds.FirstOrDefaultAsync(funds => funds.Id == id);
    }
}