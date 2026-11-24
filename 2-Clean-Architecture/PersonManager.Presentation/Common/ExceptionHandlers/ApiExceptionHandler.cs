using Microsoft.AspNetCore.Diagnostics;
using PersonManager.Core.Common.Exceptions;

namespace PersonManager.Presentation.Common.ExceptionHandlers;

/// <summary>
/// 클라이언트 원인 예외를 4xx ProblemDetails 로 변환
/// DB 제약 위반은 Infrastructure 가 Core 예외로 변환해 던지므로 여기서는 Core 예외만 다룬다
/// 매핑하지 않은 예외는 처리하지 않아 기본 500 응답으로 넘어간다
/// </summary>
public sealed class ApiExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken
	)
	{
		// title 은 지정하지 않아 상태 코드 이름(Bad Request, Conflict)으로 자동 설정된다 (RFC 9457)
		int? statusCode = exception switch
		{
			// 서비스 검증(빈 배열, 빈 filter, 없는 country id 등), Gridify 필드·문법 오류, DB 값 오류(null·길이·범위)
			InvalidRequestException => StatusCodes.Status400BadRequest,

			// DB 제약 위반 (유니크 값 중복, 참조 중인 데이터)
			ConflictException => StatusCodes.Status409Conflict,

			// 요청 바인딩 실패 (빈 body, 잘못된 JSON 등). Development 에서만 예외로 던져지고, 그 외 환경은 예외 없이 400
			BadHttpRequestException badRequest => badRequest.StatusCode,
			_ => null,
		};

		if (statusCode is not int status)
			return false;

		httpContext.Response.StatusCode = status;

		return await problemDetailsService.TryWriteAsync(
			new ProblemDetailsContext
			{
				HttpContext = httpContext,
				Exception = exception,
				ProblemDetails = { Detail = exception.Message },
			}
		);
	}
}
