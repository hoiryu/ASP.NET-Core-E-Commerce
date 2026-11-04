using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using PersonManager.Entities.Data;
using PersonManager.Entities.Modules.Identity.Countries;
using PersonManager.Repositories.Common.Extensions;
using PersonManager.RepositoryContracts.Modules.Identity.Countries;
using PersonManager.RepositoryContracts.Modules.Identity.Countries.Dtos;

namespace PersonManager.Repositories.Modules.Identity.Countries;

public class CountriesRepository(AppDbContext _db) : ICountriesRepository
{
	/// <summary>
	/// 필터·정렬에 허용할 필드 목록 (등록되지 않은 필드는 검증에서 거부)
	/// </summary>
	private static readonly IGridifyMapper<Country> _mapper = new GridifyMapper<Country>()
		.AddMap("id", temp => temp.Id)
		.AddMap("name", temp => temp.Name);

	/// <summary>
	/// SaveChanges 한 번으로 INSERT 를 배치 실행 (트랜잭션 하나라서 하나라도 실패하면 전체 롤백)
	/// </summary>
	public async Task<List<Country>> CreateCountries(List<Country> countries)
	{
		_db.Countries.AddRange(countries);
		await _db.SaveChangesAsync();

		return countries;
	}

	public async Task<Paging<Country>> GetCountries(GridifyQuery query)
	{
		query.EnsureValid(_mapper);
		query.EnsureOrdering();

		// COUNT 쿼리 + (WHERE + ORDER BY + OFFSET/FETCH) 쿼리 실행
		return await _db.Countries.AsNoTracking().GridifyAsync(query, _mapper);
	}

	/// <summary>
	/// 엔티티를 불러오지 않고 EXISTS 쿼리 한 번으로 확인
	/// </summary>
	public async Task<bool> ExistsCountryById(Guid countryId)
	{
		return await _db.Countries.AnyAsync(temp => temp.Id == countryId);
	}

	/// <summary>
	/// 모든 id 가 존재하는지 COUNT 쿼리 한 번으로 확인 (ids 는 중복 없이 전달)
	/// </summary>
	public async Task<bool> ExistsCountriesByIds(List<Guid> countryIds)
	{
		return await _db.Countries.CountAsync(temp => countryIds.Contains(temp.Id)) == countryIds.Count;
	}

	/// <summary>
	/// 이름 중 하나라도 존재하는지 EXISTS 쿼리 한 번으로 확인
	/// 대소문자 구분은 DB collation 을 따름 (기본 *_CI_AS 는 구분하지 않음)
	/// </summary>
	public async Task<bool> ExistsAnyCountryByNames(List<string> names)
	{
		return await _db.Countries.AnyAsync(temp => names.Contains(temp.Name!));
	}

	/// <summary>
	/// filter 에 맞는 행을 엔티티 로드 없이 UPDATE 한 번으로 수정하고 영향받은 행 수 반환
	/// </summary>
	public async Task<int> UpdateCountries(string filter, CountryUpdate values)
	{
		// null 이 아닌 값만 SET
		return await _db
			.Countries.ApplyBulkFiltering(filter, _mapper)
			.ExecuteUpdateAsync(setters =>
			{
				if (values.Name is not null)
					setters.SetProperty(temp => temp.Name, values.Name);
			});
	}

	/// <summary>
	/// filter 에 맞는 행을 엔티티 로드 없이 DELETE 한 번으로 삭제하고 영향받은 행 수 반환
	/// </summary>
	public async Task<int> DeleteCountries(string filter)
	{
		// 참조하는 User 의 CountryId 는 DB FK(ON DELETE SET NULL) 가 null 로 변경
		return await _db.Countries.ApplyBulkFiltering(filter, _mapper).ExecuteDeleteAsync();
	}
}
