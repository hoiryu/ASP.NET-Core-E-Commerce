using Entities.Data;
using Entities.Modules.Users;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.Common.Dtos;
using ServiceContracts.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;
using Services.Common.Extensions;
using Services.Common.Helpers;
using Services.Modules.Users.Extensions;

namespace Services.Modules.Users;

public class UsersService(AppDbContext _db) : IUsersService
{
	public async Task<UserResponse> CreateUser(UserCreateRequest? userCreateRequest)
	{
		ArgumentNullException.ThrowIfNull(userCreateRequest);

		ValidationHelper.ModelValidation(userCreateRequest);

		if (
			userCreateRequest.Country?.Id is Guid countryId
			&& !await _db.Countries.AnyAsync(temp => temp.Id == countryId)
		)
			throw new ArgumentException("Given country id doesn't exist");

		User user = userCreateRequest.ToEntity();

		_db.Users.Add(user);
		await _db.SaveChangesAsync();

		// 응답에 Country 를 담기 위해 내비게이션 속성 로드
		await _db.Entry(user).Reference(temp => temp.Country).LoadAsync();

		return user.ToResponse();
	}

	public async Task<List<UserResponse>> GetUsers(UserFilter filter, UserOrder order, Paging paging)
	{
		IQueryable<User> users = _db.Users.Include(temp => temp.Country);

		users = users.ApplyFiltering(filter).ApplyOrdering(order).ApplyPaging(paging);

		// 여기서 SQL 실행 (JOIN + WHERE + ORDER BY + OFFSET/FETCH 포함)
		return await users.Select(user => user.ToResponse()).ToListAsync();
	}

	public async Task<UserResponse?> UpdateUser(UserUpdateRequest? userUpdateRequest)
	{
		ArgumentNullException.ThrowIfNull(userUpdateRequest);

		ValidationHelper.ModelValidation(userUpdateRequest);

		if (
			userUpdateRequest.Country?.Id is Guid countryId
			&& !await _db.Countries.AnyAsync(temp => temp.Id == countryId)
		)
			throw new ArgumentException("Given country id doesn't exist");

		User? matchingUser = await _db
			.Users.Include(temp => temp.Country)
			.FirstOrDefaultAsync(temp => temp.Id == userUpdateRequest.Id);

		if (matchingUser == null)
			return null;

		// null 은 "보내지 않음" 으로 보고 기존 값을 유지
		matchingUser.Name = userUpdateRequest.Name ?? matchingUser.Name;
		matchingUser.Email = userUpdateRequest.Email ?? matchingUser.Email;
		matchingUser.DateOfBirth = userUpdateRequest.DateOfBirth ?? matchingUser.DateOfBirth;
		matchingUser.Gender = userUpdateRequest.Gender?.ToString() ?? matchingUser.Gender;
		matchingUser.CountryId = userUpdateRequest.Country?.Id ?? matchingUser.CountryId;
		matchingUser.Address = userUpdateRequest.Address ?? matchingUser.Address;
		matchingUser.ReceiveNewsLetters = userUpdateRequest.ReceiveNewsLetters ?? matchingUser.ReceiveNewsLetters;

		// 변경 감지된 컬럼만 UPDATE
		await _db.SaveChangesAsync();

		// CountryId 가 바뀌었을 수 있으므로 Country 다시 로드
		await _db.Entry(matchingUser).Reference(temp => temp.Country).LoadAsync();

		return matchingUser.ToResponse();
	}

	public async Task<UserResponse?> DeleteUser(Guid? userId)
	{
		if (!userId.HasValue)
		{
			throw new ArgumentNullException(nameof(userId));
		}

		User? matchingUser = await _db
			.Users.Include(temp => temp.Country)
			.FirstOrDefaultAsync(temp => temp.Id == userId);

		if (matchingUser == null)
			return null;

		_db.Users.Remove(matchingUser);
		await _db.SaveChangesAsync();

		return matchingUser.ToResponse();
	}
}
