using PersonManager.Entities.Data;
using PersonManager.Entities.Modules.Users;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using PersonManager.RepositoryContracts.Modules.Users;

namespace PersonManager.Repositories.Modules.Users;

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

	public async Task<User?> GetUserById(Guid userId)
	{
		return await _db.Users.Include(temp => temp.Country).FirstOrDefaultAsync(temp => temp.Id == userId);
	}

	/// <summary>
	/// 같은 DbContext 에서 조회해 추적 중인 엔티티를 받아 변경 사항만 저장 (재조회하지 않음)
	/// </summary>
	public async Task<User> UpdateUser(User user)
	{
		// 변경 감지된 컬럼만 UPDATE
		await _db.SaveChangesAsync();

		// CountryId 가 바뀌었을 수 있으므로 Country 다시 로드
		await _db.Entry(user).Reference(temp => temp.Country).LoadAsync();

		return user;
	}

	/// <summary>
	/// 같은 DbContext 에서 조회해 추적 중인 엔티티를 받아 삭제 (재조회하지 않음)
	/// </summary>
	public async Task<User> DeleteUser(User user)
	{
		_db.Users.Remove(user);
		await _db.SaveChangesAsync();

		return user;
	}
}
