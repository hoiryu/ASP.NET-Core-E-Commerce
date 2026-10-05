using Entities.Modules.Users;
using ServiceContracts.Modules.Users.Dtos;
using ServiceContracts.Modules.Users.Enums;
using Services.Common.Extensions;

namespace Services.Modules.Users.Extensions;

public static class UserQueryExtensions
{
	internal static IQueryable<User> ApplyFiltering(this IQueryable<User> users, UserFilter filter)
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

	internal static IQueryable<User> ApplyOrdering(this IQueryable<User> users, UserOrder order)
	{
		// 값이 있는 조건만 첫 번째는 OrderBy, 이후는 ThenBy 로 이어 붙임
		IOrderedQueryable<User>? ordered = null;

		ordered = users.OrderByOption(ordered, order.Id, temp => temp.Id);
		ordered = users.OrderByOption(ordered, order.Name, temp => temp.Name);
		ordered = users.OrderByOption(ordered, order.Email, temp => temp.Email);
		ordered = users.OrderByOption(ordered, order.DateOfBirth, temp => temp.DateOfBirth);
		ordered = users.OrderByOption(ordered, order.Gender, temp => temp.Gender);
		ordered = users.OrderByOption(ordered, order.Country?.Id, temp => temp.CountryId);
		ordered = users.OrderByOption(ordered, order.Country?.Name, temp => temp.Country!.Name);
		ordered = users.OrderByOption(ordered, order.Address, temp => temp.Address);
		ordered = users.OrderByOption(ordered, order.ReceiveNewsLetters, temp => temp.ReceiveNewsLetters);

		return ordered ?? users;
	}
}
