using EntityFramework.Exceptions.Common;
using PersonManager.Core.Common.Exceptions;

namespace PersonManager.Infrastructure.Common.Extensions;

internal static class DbExceptionExtensions
{
	/// <summary>
	/// DB 쓰기 작업을 실행하고, UseExceptionProcessor 가 던진 제약 위반 예외를 Core 예외로 변환 (원본은 InnerException 으로 보존)
	/// 원본 메시지에 SQL 정보가 있어 메시지는 고정 문구를 쓴다
	/// </summary>
	public static async Task<T> TranslateDbExceptions<T>(this Task<T> task)
	{
		try
		{
			return await task;
		}
		catch (UniqueConstraintException ex)
		{
			throw new ConflictException("The request conflicts with existing data.", ex);
		}
		catch (ReferenceConstraintException ex)
		{
			throw new ConflictException("The request conflicts with related data.", ex);
		}
		catch (CannotInsertNullException ex)
		{
			throw new InvalidRequestException("A required value is missing.", ex);
		}
		catch (MaxLengthExceededException ex)
		{
			throw new InvalidRequestException("A value exceeds the maximum length.", ex);
		}
		catch (NumericOverflowException ex)
		{
			throw new InvalidRequestException("A numeric value is out of range.", ex);
		}
	}
}
