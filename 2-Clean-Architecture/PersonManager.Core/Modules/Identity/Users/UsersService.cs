using PersonManager.Core.Common.Exceptions;
using PersonManager.Core.Common.Helpers;
using PersonManager.Core.Common.Logging;
using PersonManager.Core.Common.Queries;
using PersonManager.Core.Modules.Identity.Countries;
using PersonManager.Core.Modules.Identity.Users.Dtos;
using PersonManager.Domain.Modules.Identity.Users;

namespace PersonManager.Core.Modules.Identity.Users;

public class UsersService(
	IUsersRepository _usersRepository,
	ICountriesRepository _countriesRepository,
	TimeProvider _timeProvider
) : IUsersService
{
	[LogExecutionTime(nameof(userCreateRequests))]
	public async Task<List<UserResponse>> CreateUsers(List<UserCreateRequest> userCreateRequests)
	{
		// 저장 전에 전체를 검증 (하나라도 실패하면 아무것도 저장하지 않음)
		ValidationHelper.ModelValidation(userCreateRequests);

		List<Guid> countryIds = [.. userCreateRequests.Select(temp => temp.Country?.Id).OfType<Guid>().Distinct()];
		await EnsureCountriesExist(countryIds);

		List<User> users = await _usersRepository.CreateUsers([.. userCreateRequests.Select(temp => temp.ToEntity())]);

		DateTime now = Now();
		return [.. users.Select(user => user.ToResponse(now))];
	}

	[LogExecutionTime(nameof(query))]
	public async Task<PagedResult<UserResponse>> GetUsers(ListQuery query)
	{
		PagedResult<User> users = await _usersRepository.GetUsers(query);

		DateTime now = Now();
		return new PagedResult<UserResponse>(users.Count, users.Data.Select(user => user.ToResponse(now)));
	}

	[LogExecutionTime(nameof(filter))]
	public async Task<int> UpdateUsers(string? filter, UserUpdateRequest userUpdateRequest)
	{
		ValidationHelper.FilterValidation(filter);

		ValidationHelper.ModelValidation(userUpdateRequest);

		if (userUpdateRequest.Country?.Id is Guid countryId)
			await EnsureCountriesExist([countryId]);

		return await _usersRepository.UpdateUsers(filter, userUpdateRequest.ToUpdate());
	}

	[LogExecutionTime(nameof(filter))]
	public async Task<int> DeleteUsers(string? filter)
	{
		ValidationHelper.FilterValidation(filter);

		return await _usersRepository.DeleteUsers(filter);
	}

	/// <summary>
	/// 참조할 country id 가 모두 있는지 쿼리 한 번으로 확인 (ids 는 중복 없이 전달)
	/// </summary>
	private async Task EnsureCountriesExist(List<Guid> countryIds)
	{
		if (countryIds.Count > 0 && !await _countriesRepository.ExistsCountriesByIds(countryIds))
			throw new InvalidRequestException("Given country id doesn't exist");
	}

	// 나이 계산 기준 시각 (기존 DateTime.Now 와 같은 로컬 시각)
	private DateTime Now() => _timeProvider.GetLocalNow().DateTime;
}
