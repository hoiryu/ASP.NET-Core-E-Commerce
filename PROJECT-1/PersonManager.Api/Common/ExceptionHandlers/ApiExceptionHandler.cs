using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PersonManager.Api.Common.ExceptionHandlers;

/// <summary>
/// 클라이언트 원인 예외를 4xx ProblemDetails 로 변환
/// 매핑하지 않은 예외는 처리하지 않아 기본 500 응답으로 넘어간다
/// </summary>
public sealed class ApiExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
	// SQL Server 오류 번호
	private const int UniqueIndexViolation = 2601; // 유니크 인덱스 중복 (HasIndex().IsUnique())
	private const int UniqueConstraintViolation = 2627; // PK·UNIQUE 제약 중복

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken
	)
	{
		// title 은 지정하지 않아 상태 코드 이름(Bad Request, Conflict)으로 자동 설정된다 (RFC 9457)
		(int statusCode, string detail)? error = exception switch
		{
			// 서비스 검증(빈 배열, 빈 filter, 중복 등)과 Gridify 파싱 오류
			ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),

			// 요청 바인딩 실패 (빈 body, 잘못된 JSON 등). Development 에서만 예외로 던져지고, 그 외 환경은 예외 없이 400
			BadHttpRequestException badRequest => (badRequest.StatusCode, exception.Message),

			// 유니크 인덱스 위반 (메시지에 SQL 정보가 있어 detail 은 고정 문구)
			// SaveChanges 는 DbUpdateException 으로 감싸고, ExecuteUpdate/Delete 는 SqlException 을 그대로 던진다
			DbUpdateException
			{
				InnerException: SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation }
			}
			or SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation } => (
				StatusCodes.Status409Conflict,
				"The request conflicts with existing data."
			),
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
