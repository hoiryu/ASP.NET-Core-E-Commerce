using Entities.Data;
using Entities.Modules.Countries;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts.Modules.Countries;

namespace Repositories.Modules.Countries;

public class CountriesRepository(AppDbContext _db) : ICountriesRepository
{
	/// <summary>
	/// 필터·정렬에 허용할 필드 목록 (등록되지 않은 필드는 검증에서 거부)
	/// </summary>
	private static readonly IGridifyMapper<Country> _mapper = new GridifyMapper<Country>()
		.AddMap("id", temp => temp.Id)
		.AddMap("name", temp => temp.Name);

	public async Task<Country> CreateCountry(Country country)
	{
		_db.Countries.Add(country);
		await _db.SaveChangesAsync();

		return country;
	}

	public async Task<Paging<Country>> GetCountries(GridifyQuery query)
	{
		if (!query.IsValid(out var errors, _mapper))
			throw new ArgumentException(string.Join(", ", errors));

		// COUNT 쿼리 + (WHERE + ORDER BY + OFFSET/FETCH) 쿼리 실행
		return await _db.Countries.GridifyAsync(query, _mapper);
	}

	/// <summary>
	/// 같은 DbContext 에서 조회해 추적 중인 엔티티를 받아 변경 사항만 저장 (재조회하지 않음)
	/// </summary>
	public async Task<Country> UpdateCountry(Country country)
	{
		await _db.SaveChangesAsync();

		return country;
	}

	public async Task<Country?> DeleteCountry(Guid countryId)
	{
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

		return matchingCountry;
	}
}
