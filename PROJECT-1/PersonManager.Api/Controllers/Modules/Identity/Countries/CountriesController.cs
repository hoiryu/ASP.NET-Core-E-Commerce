using Gridify;
using Microsoft.AspNetCore.Mvc;
using PersonManager.ServiceContracts.Modules.Identity.Countries;
using PersonManager.ServiceContracts.Modules.Identity.Countries.Dtos;
using Serilog;

namespace PersonManager.Api.Controllers.Modules.Identity.Countries
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController(ICountriesService _countriesService, IDiagnosticContext _diagnosticContext)
		: ControllerBase
	{
		/// <summary>
		/// 국가 일괄 생성 (한 건도 배열로 전달)
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<List<CountryResponse>>> CreateCountries(
			[FromBody] List<CountryCreateRequest> countryCreateRequests
		)
		{
			var countries = await _countriesService.CreateCountries(countryCreateRequests);
			_diagnosticContext.Set("AffectedCount", countries.Count); // 요청 완료 로그에 생성된 행 수 추가

			return countries;
		}

		[HttpGet]
		public async Task<ActionResult<Paging<CountryResponse>>> GetCountries([FromQuery] GridifyQuery query)
		{
			return await _countriesService.GetCountries(query);
		}

		/// <summary>
		/// filter 에 맞는 국가 일괄 수정 (orderBy·page·pageSize 는 무시)
		/// </summary>
		[HttpPatch]
		public async Task<ActionResult<object>> UpdateCountries(
			[FromQuery] GridifyQuery query,
			[FromBody] CountryUpdateRequest countryUpdateRequest
		)
		{
			var count = await _countriesService.UpdateCountries(query.Filter, countryUpdateRequest);
			_diagnosticContext.Set("AffectedCount", count); // 요청 완료 로그에 영향받은 행 수 추가

			return new { count };
		}

		/// <summary>
		/// filter 에 맞는 국가 일괄 삭제 (orderBy·page·pageSize 는 무시)
		/// </summary>
		[HttpDelete]
		public async Task<ActionResult<object>> DeleteCountries([FromQuery] GridifyQuery query)
		{
			var count = await _countriesService.DeleteCountries(query.Filter);
			_diagnosticContext.Set("AffectedCount", count); // 요청 완료 로그에 영향받은 행 수 추가

			return new { count };
		}
	}
}
