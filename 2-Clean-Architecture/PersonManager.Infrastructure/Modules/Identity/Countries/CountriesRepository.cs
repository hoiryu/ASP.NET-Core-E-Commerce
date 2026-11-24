using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Countries;
using PersonManager.Core.Modules.Identity.Countries.Dtos;
using PersonManager.Domain.Modules.Identity.Countries;
using PersonManager.Infrastructure.Common.Extensions;
using PersonManager.Infrastructure.Data;

namespace PersonManager.Infrastructure.Modules.Identity.Countries;

public class CountriesRepository(AppDbContext _db, IGridifyMapper<Country> _mapper) : ICountriesRepository
{
	/// <summary>
	/// SaveChanges 한 번으로 INSERT 를 배치 실행 (트랜잭션 하나라서 하나라도 실패하면 전체 롤백)
	/// </summary>
	public async Task<List<Country>> CreateCountries(List<Country> countries)
	{
		_db.Countries.AddRange(countries);
		await _db.SaveChangesAsync().TranslateDbExceptions();

		return countries;
	}

	public async Task<PagedResult<Country>> GetCountries(ListQuery query)
	{
		var gridifyQuery = query.ToGridifyQuery();
		gridifyQuery.EnsureValid(_mapper);

		// COUNT 쿼리 + (WHERE + ORDER BY + OFFSET/FETCH) 쿼리 실행
		var paging = await _db.Countries.AsNoTracking().GridifyAsync(gridifyQuery, _mapper);

		return paging.ToPagedResult();
	}

	/// <summary>
	/// 모든 id 가 존재하는지 COUNT 쿼리 한 번으로 확인 (ids 는 중복 없이 전달)
	/// </summary>
	public async Task<bool> ExistsCountriesByIds(List<Guid> countryIds)
	{
		return await _db.Countries.CountAsync(temp => countryIds.Contains(temp.Id)) == countryIds.Count;
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
			})
			.TranslateDbExceptions();
	}

	/// <summary>
	/// filter 에 맞는 행을 엔티티 로드 없이 DELETE 한 번으로 삭제하고 영향받은 행 수 반환
	/// </summary>
	public async Task<int> DeleteCountries(string filter)
	{
		// 참조하는 User 의 CountryId 는 DB FK(ON DELETE SET NULL) 가 null 로 변경
		return await _db.Countries.ApplyBulkFiltering(filter, _mapper).ExecuteDeleteAsync().TranslateDbExceptions();
	}
}
