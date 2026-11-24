using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Users;
using PersonManager.Core.Modules.Identity.Users.Dtos;
using PersonManager.Domain.Modules.Identity.Users;
using PersonManager.Infrastructure.Common.Extensions;
using PersonManager.Infrastructure.Data;

namespace PersonManager.Infrastructure.Modules.Identity.Users;

public class UsersRepository(AppDbContext _db, IGridifyMapper<User> _mapper) : IUsersRepository
{
	/// <summary>
	/// SaveChanges 한 번으로 INSERT 를 배치 실행 (트랜잭션 하나라서 하나라도 실패하면 전체 롤백)
	/// </summary>
	public async Task<List<User>> CreateUsers(List<User> users)
	{
		_db.Users.AddRange(users);
		await _db.SaveChangesAsync().TranslateDbExceptions();

		// 응답에 Country 를 담기 위해 쿼리 한 번으로 로드 (추적 중인 User 의 내비게이션 속성에 자동 연결)
		List<Guid> countryIds = [.. users.Select(temp => temp.CountryId).OfType<Guid>().Distinct()];
		if (countryIds.Count > 0)
			await _db.Countries.Where(temp => countryIds.Contains(temp.Id)).LoadAsync();

		return users;
	}

	public async Task<PagedResult<User>> GetUsers(ListQuery query)
	{
		var gridifyQuery = query.ToGridifyQuery();
		gridifyQuery.EnsureValid(_mapper);

		// COUNT 쿼리 + (JOIN + WHERE + ORDER BY + OFFSET/FETCH) 쿼리 실행
		var paging = await _db.Users.AsNoTracking().Include(temp => temp.Country).GridifyAsync(gridifyQuery, _mapper);

		return paging.ToPagedResult();
	}

	/// <summary>
	/// filter 에 맞는 행을 엔티티 로드 없이 UPDATE 한 번으로 수정하고 영향받은 행 수 반환
	/// </summary>
	public async Task<int> UpdateUsers(string filter, UserUpdate values)
	{
		// null 이 아닌 값만 SET
		return await _db
			.Users.ApplyBulkFiltering(filter, _mapper)
			.ExecuteUpdateAsync(setters =>
			{
				if (values.Name is not null)
					setters.SetProperty(temp => temp.Name, values.Name);
				// 여러 행이 대상이면 유니크 인덱스 위반으로 UPDATE 전체가 실패
				if (values.Email is not null)
					setters.SetProperty(temp => temp.Email, values.Email);
				if (values.DateOfBirth is not null)
					setters.SetProperty(temp => temp.DateOfBirth, values.DateOfBirth);
				if (values.Gender is not null)
					setters.SetProperty(temp => temp.Gender, values.Gender);
				if (values.CountryId is not null)
					setters.SetProperty(temp => temp.CountryId, values.CountryId);
				if (values.Address is not null)
					setters.SetProperty(temp => temp.Address, values.Address);
				if (values.ReceiveNewsLetters is bool receiveNewsLetters)
					setters.SetProperty(temp => temp.ReceiveNewsLetters, receiveNewsLetters);
			})
			.TranslateDbExceptions();
	}

	/// <summary>
	/// filter 에 맞는 행을 엔티티 로드 없이 DELETE 한 번으로 삭제하고 영향받은 행 수 반환
	/// </summary>
	public async Task<int> DeleteUsers(string filter)
	{
		return await _db.Users.ApplyBulkFiltering(filter, _mapper).ExecuteDeleteAsync().TranslateDbExceptions();
	}
}
