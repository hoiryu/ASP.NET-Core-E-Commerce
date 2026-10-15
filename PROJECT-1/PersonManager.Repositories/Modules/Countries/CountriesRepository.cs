using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using PersonManager.Entities.Data;
using PersonManager.Entities.Modules.Countries;
using PersonManager.RepositoryContracts.Modules.Countries;

namespace PersonManager.Repositories.Modules.Countries;

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
		return await _db.Countries.AsNoTracking().GridifyAsync(query, _mapper);
	}

	public async Task<Country?> GetCountryById(Guid countryId)
	{
		return await _db.Countries.FirstOrDefaultAsync(temp => temp.Id == countryId);
	}

	/// <summary>
	/// 엔티티를 불러오지 않고 EXISTS 쿼리 한 번으로 확인
	/// </summary>
	public async Task<bool> ExistsCountryById(Guid countryId)
	{
		return await _db.Countries.AnyAsync(temp => temp.Id == countryId);
	}

	/// <summary>
	/// 대소문자 구분은 DB collation 을 따름 (기본 *_CI_AS 는 구분하지 않음)
	/// </summary>
	public async Task<bool> ExistsCountryByName(string name)
	{
		return await _db.Countries.AnyAsync(temp => temp.Name == name);
	}

	/// <summary>
	/// 같은 DbContext 에서 조회해 추적 중인 엔티티를 받아 변경 사항만 저장 (재조회하지 않음)
	/// </summary>
	public async Task<Country> UpdateCountry(Country country)
	{
		await _db.SaveChangesAsync();

		return country;
	}

	/// <summary>
	/// 같은 DbContext 에서 조회해 추적 중인 엔티티를 받아 삭제 (재조회하지 않음)
	/// </summary>
	public async Task<Country> DeleteCountry(Country country)
	{
		// 참조하는 User 의 CountryId 는 DB FK(ON DELETE SET NULL) 가 null 로 변경
		_db.Countries.Remove(country);
		await _db.SaveChangesAsync();

		return country;
	}
}
