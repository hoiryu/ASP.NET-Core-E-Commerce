using System.Collections;
using PersonManager.Api.Endpoints.Common.Dtos;
using Serilog;

namespace PersonManager.Api.Endpoints.Common.EndpointFilters;

/// <summary>
/// 생성·수정·삭제(CUD) 엔드포인트 공통 필터
/// 핸들러 반환값에서 영향받은 행 수를 꺼내 요청 완료 로그에 AffectedCount 로 추가
/// </summary>
public sealed class MutationEndpointFilter(IDiagnosticContext diagnosticContext) : IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var result = await next(context);

		int? count = result switch
		{
			MutationResponse response => response.Count, // 수정·삭제
			ICollection collection => collection.Count, // 생성
			_ => null,
		};

		if (count is not null)
		{
			diagnosticContext.Set("AffectedCount", count);
		}

		return result;
	}
}
