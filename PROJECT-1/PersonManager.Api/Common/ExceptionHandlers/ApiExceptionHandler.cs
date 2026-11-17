using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;
using PersonManager.Exceptions;

namespace PersonManager.Api.Common.ExceptionHandlers;

/// <summary>
/// 클라이언트 원인 예외를 4xx ProblemDetails 로 변환
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
		(int statusCode, string detail)? error = exception switch
		{
			// 서비스 검증(빈 배열, 빈 filter, 없는 country id 등)과 Gridify 필드·문법 오류
			InvalidRequestException => (StatusCodes.Status400BadRequest, exception.Message),

			// 요청 바인딩 실패 (빈 body, 잘못된 JSON 등). Development 에서만 예외로 던져지고, 그 외 환경은 예외 없이 400
			BadHttpRequestException badRequest => (badRequest.StatusCode, exception.Message),

			// DB 제약 위반 (UseExceptionProcessor 가 변환. 원본 메시지에 SQL 정보가 있어 detail 은 고정 문구)
			UniqueConstraintException => (StatusCodes.Status409Conflict, "The request conflicts with existing data."),
			ReferenceConstraintException => (StatusCodes.Status409Conflict, "The request conflicts with related data."),
			CannotInsertNullException => (StatusCodes.Status400BadRequest, "A required value is missing."),
			MaxLengthExceededException => (StatusCodes.Status400BadRequest, "A value exceeds the maximum length."),
			NumericOverflowException => (StatusCodes.Status400BadRequest, "A numeric value is out of range."),
			_ => null,
		};

		if (error is not var (statusCode, detail))
			return false;

		httpContext.Response.StatusCode = statusCode;

		return await problemDetailsService.TryWriteAsync(
			new ProblemDetailsContext
			{
				HttpContext = httpContext,
				Exception = exception,
				ProblemDetails = { Detail = detail },
			}
		);
	}
}
