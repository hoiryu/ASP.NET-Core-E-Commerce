using Entities.Data;
using Entities.Modules.Countries;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Countries;
using ServiceContracts.Modules.Countries.Dtos;
using Services.Common.Extensions;
using Services.Common.Helpers;
using Services.Modules.Countries.Extensions;

namespace Services.Modules.Countries;

public class CountriesService(AppDbContext _db) : ICountriesService
{
	public async Task<CountryResponse> CreateCountry(CountryCreateRequest? countryCreateRequest)
	{
		if (countryCreateRequest == null || countryCreateRequest.Name == null)
		{
			throw new ArgumentException("countryCreateRequest and countryCreateRequest.Name must not be null");
		}

		if (await _db.Countries.AnyAsync(temp => temp.Name == countryCreateRequest.Name))
		{
			throw new ArgumentException("Given country name already exists");
		}

		Country country = countryCreateRequest.ToEntity();

		_db.Countries.Add(country);
		await _db.SaveChangesAsync();

		return country.ToResponse();
	}

	public async Task<List<CountryResponse>> GetCountries(CountryFilter filter, CountryOrder order, Paging paging)
	{
		IQueryable<Country> countries = _db.Countries;

		countries = countries.ApplyFiltering(filter).ApplyOrdering(order).ApplyPaging(paging);

		// 여기서 SQL 실행 (WHERE + ORDER BY + OFFSET/FETCH 포함)
		return await countries.Select(country => country.ToResponse()).ToListAsync();
	}

	public async Task<CountryResponse?> UpdateCountry(CountryUpdateRequest? countryUpdateRequest)
	{
		ArgumentNullException.ThrowIfNull(countryUpdateRequest);

		ValidationHelper.ModelValidation(countryUpdateRequest);

		IQueryable<Country> countries = _db.Countries;

		Country? matchingCountry = await countries.FirstOrDefaultAsync(temp => temp.Id == countryUpdateRequest.Id);

		if (matchingCountry == null)
			return null;

		matchingCountry.Name = countryUpdateRequest.Name ?? matchingCountry.Name;

		await _db.SaveChangesAsync();

		return matchingCountry.ToResponse();
	}

	public async Task<CountryResponse?> DeleteCountry(Guid? countryId)
	{
		if (!countryId.HasValue)
			throw new ArgumentNullException(nameof(countryId));

		Country? matchingCountry = await _db.Countries.FirstOrDefaultAsync(temp => temp.Id == countryId);

		if (matchingCountry == null)
			return null;

		// UPDATE users 와 DELETE countries 를 하나의 트랜잭션으로 묶음
		await using var transaction = await _db.Database.BeginTransactionAsync();

		// 이 Country 를 참조하는 User 의 CountryId 를 null 로 초기화 (UPDATE 한 번으로 실행)
		await _db.Users.Where(temp => temp.CountryId == countryId)
			.ExecuteUpdateAsync(setters => setters.SetProperty(temp => temp.CountryId, (Guid?)null));

		_db.Countries.Remove(matchingCountry);
		await _db.SaveChangesAsync();

		await transaction.CommitAsync();

		return matchingCountry.ToResponse();
	}
}
