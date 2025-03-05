using XChange.Data.Entities;

namespace XChange.Data.Repositories.CompanyExchangeFunds;

public interface ICompanyExchangeFundsRepository
{
    public Task<CompanyExchangeFundEntity> Get();
    public Task<CompanyExchangeFundEntity> GetByIdAsync(int id);
}