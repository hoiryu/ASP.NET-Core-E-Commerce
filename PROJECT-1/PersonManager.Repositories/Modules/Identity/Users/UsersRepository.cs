using PersonManager.Entities.Data;
using PersonManager.Entities.Modules.Identity.Users;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using PersonManager.RepositoryContracts.Modules.Identity.Users;
using PersonManager.RepositoryContracts.Modules.Identity.Users.Dtos;

namespace PersonManager.Repositories.Modules.Identity.Users;

public class UsersRepository(AppDbContext _db) : IUsersRepository
{
	/// <summary>
	/// 필터·정렬에 허용할 필드 목록 (등록되지 않은 필드는 검증에서 거부)
	/// </summary>
	private static readonly IGridifyMapper<User> _mapper = new GridifyMapper<User>()
		.AddMap("id", temp => temp.Id)
		.AddMap("name", temp => temp.Name)
		.AddMap("email", temp => temp.Email)
		.AddMap("dateOfBirth", temp => temp.DateOfBirth)
		.AddMap("gender", temp => temp.Gender)
		.AddMap("address", temp => temp.Address)
		.AddMap("receiveNewsLetters", temp => temp.ReceiveNewsLetters)
		.AddMap("country.id", temp => temp.CountryId)
		.AddMap("country.name", temp => temp.Country!.Name);

	public async Task<User> CreateUser(User user)
	{
		_db.Users.Add(user);
		await _db.SaveChangesAsync();

		// 응답에 Country 를 담기 위해 내비게이션 속성 로드
		await _db.Entry(user).Reference(temp => temp.Country).LoadAsync();

		return user;
	}

	public async Task<Paging<User>> GetUsers(GridifyQuery query)
	{
		if (!query.IsValid(out var errors, _mapper))
			throw new ArgumentException(string.Join(", ", errors));

		// COUNT 쿼리 + (JOIN + WHERE + ORDER BY + OFFSET/FETCH) 쿼리 실행
		return await _db.Users.AsNoTracking().Include(temp => temp.Country).GridifyAsync(query, _mapper);
	}

	/// <summary>
	/// filter 에 맞는 행을 엔티티 로드 없이 UPDATE 한 번으로 수정하고 영향받은 행 수 반환
	/// </summary>
	public async Task<int> UpdateUsers(string filter, UserUpdate values)
	{
		ValidateFilter(filter);

		// null 이 아닌 값만 SET
		return await _db
			.Users.ApplyFiltering(filter, _mapper)
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
			});
	}

	/// <summary>
	/// filter 에 맞는 행을 엔티티 로드 없이 DELETE 한 번으로 삭제하고 영향받은 행 수 반환
	/// </summary>
	public async Task<int> DeleteUsers(string filter)
	{
		ValidateFilter(filter);

		return await _db.Users.ApplyFiltering(filter, _mapper).ExecuteDeleteAsync();
	}

	/// <summary>
	/// 빈 filter 는 전체 행이 대상이 되므로 거부
	/// </summary>
	private static void ValidateFilter(string filter)
	{
		if (string.IsNullOrWhiteSpace(filter))
			throw new ArgumentException("Filter is required for bulk operations");

		if (!new GridifyQuery { Filter = filter }.IsValid(out var errors, _mapper))
			throw new ArgumentException(string.Join(", ", errors));
	}
}
