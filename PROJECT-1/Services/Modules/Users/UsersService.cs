using System.Linq.Expressions;
using Entities.Data;
using Entities.Modules.Users;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.Common.Enums;
using ServiceContracts.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;
using ServiceContracts.Modules.Users.Enums;
using Services.Common.Helpers;

namespace Services.Modules.Users;

public class UsersService(AppDbContext _db) : IUsersService
{
	public UserResponse CreateUser(UserCreateRequest? userCreateRequest)
	{
		ArgumentNullException.ThrowIfNull(userCreateRequest);

		ValidationHelper.ModelValidation(userCreateRequest);

		if (userCreateRequest.Country?.Id is Guid countryId && !_db.Countries.Any(temp => temp.Id == countryId))
			throw new ArgumentException("Given country id doesn't exist");

		User user = userCreateRequest.ToEntity();

		_db.Users.Add(user);
		_db.SaveChanges();

		// 응답에 Country 를 담기 위해 내비게이션 속성 로드
		_db.Entry(user).Reference(temp => temp.Country).Load();

		return user.ToResponse();
	}

	public List<UserResponse> GetUsers(UserFilter filter, UserSort sort)
	{
		IQueryable<User> users = _db.Users.Include(temp => temp.Country);

		users = FilterUsers(users, filter);
		users = SortUsers(users, sort);

		// 여기서 SQL 실행 (JOIN + WHERE + ORDER BY 포함)
		return [.. users.Select(user => user.ToResponse())];
	}

	private static IQueryable<User> FilterUsers(IQueryable<User> users, UserFilter filter)
	{
		// 값이 있는 조건만 Where 로 이어 붙임 (AND)
		if (filter.Id is Guid userId)
			users = users.Where(temp => temp.Id == userId);

		if (!string.IsNullOrEmpty(filter.Name))
			users = users.Where(temp => temp.Name != null && temp.Name.Contains(filter.Name));

		if (!string.IsNullOrEmpty(filter.Email))
			users = users.Where(temp => temp.Email != null && temp.Email.Contains(filter.Email));

		if (filter.DateOfBirth is DateTime dateOfBirth)
			users = users.Where(temp => temp.DateOfBirth != null && temp.DateOfBirth.Value.Date == dateOfBirth.Date);

		if (filter.Gender is GenderOptions gender)
		{
			string genderString = gender.ToString();
			users = users.Where(temp => temp.Gender == genderString);
		}

		if (filter.Country?.Id is Guid countryId)
			users = users.Where(temp => temp.CountryId == countryId);

		if (!string.IsNullOrEmpty(filter.Country?.Name))
		{
			string countryName = filter.Country.Name;
			users = users.Where(temp =>
				temp.Country != null && temp.Country.Name != null && temp.Country.Name.Contains(countryName)
			);
		}

		if (!string.IsNullOrEmpty(filter.Address))
			users = users.Where(temp => temp.Address != null && temp.Address.Contains(filter.Address));

		if (filter.ReceiveNewsLetters is bool receiveNewsLetters)
			users = users.Where(temp => temp.ReceiveNewsLetters == receiveNewsLetters);

		return users;
	}

	private static IQueryable<User> SortUsers(IQueryable<User> users, UserSort sort)
	{
		// 값이 있는 조건만 첫 번째는 OrderBy, 이후는 ThenBy 로 이어 붙임
		IOrderedQueryable<User>? ordered = null;

		ordered = ApplySort(users, ordered, sort.Id, temp => temp.Id);
		ordered = ApplySort(users, ordered, sort.Name, temp => temp.Name);
		ordered = ApplySort(users, ordered, sort.Email, temp => temp.Email);
		ordered = ApplySort(users, ordered, sort.DateOfBirth, temp => temp.DateOfBirth);
		ordered = ApplySort(users, ordered, sort.Gender, temp => temp.Gender);
		ordered = ApplySort(users, ordered, sort.Country?.Id, temp => temp.CountryId);
		ordered = ApplySort(users, ordered, sort.Country?.Name, temp => temp.Country!.Name);
		ordered = ApplySort(users, ordered, sort.Address, temp => temp.Address);
		ordered = ApplySort(users, ordered, sort.ReceiveNewsLetters, temp => temp.ReceiveNewsLetters);

		return ordered ?? users;
	}

	private static IOrderedQueryable<User>? ApplySort<T>(
		IQueryable<User> users,
		IOrderedQueryable<User>? ordered,
		SortOrderOptions? sortOrder,
		Expression<Func<User, T>> keySelector
	)
	{
		if (sortOrder is null)
			return ordered;

		bool descending = sortOrder == SortOrderOptions.DESC;

		if (ordered is null)
			return descending ? users.OrderByDescending(keySelector) : users.OrderBy(keySelector);

		return descending ? ordered.ThenByDescending(keySelector) : ordered.ThenBy(keySelector);
	}

	public UserResponse? UpdateUser(UserUpdateRequest? userUpdateRequest)
	{
		ArgumentNullException.ThrowIfNull(userUpdateRequest);

		ValidationHelper.ModelValidation(userUpdateRequest);

		if (userUpdateRequest.Country?.Id is Guid countryId && !_db.Countries.Any(temp => temp.Id == countryId))
			throw new ArgumentException("Given country id doesn't exist");

		User? matchingUser = _db
			.Users.Include(temp => temp.Country)
			.FirstOrDefault(temp => temp.Id == userUpdateRequest.Id);

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
		_db.SaveChanges();

		// CountryId 가 바뀌었을 수 있으므로 Country 다시 로드
		_db.Entry(matchingUser).Reference(temp => temp.Country).Load();

		return matchingUser.ToResponse();
	}

	public UserResponse? DeleteUser(Guid? userId)
	{
		if (!userId.HasValue)
		{
			throw new ArgumentNullException(nameof(userId));
		}

		User? matchingUser = _db.Users.Include(temp => temp.Country).FirstOrDefault(temp => temp.Id == userId);

		if (matchingUser == null)
			return null;

		_db.Users.Remove(matchingUser);
		_db.SaveChanges();

		return matchingUser.ToResponse();
	}
}
