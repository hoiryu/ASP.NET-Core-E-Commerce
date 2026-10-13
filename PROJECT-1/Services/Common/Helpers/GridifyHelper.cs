using System.Text.RegularExpressions;

namespace Services.Common.Helpers;

public static partial class GridifyHelper
{
	/// <summary>
	/// Gridify 필터 특수문자 ( , | ( ) \ /i ) 앞에 \ 를 붙여 값으로 취급되게 함
	/// </summary>
	internal static string Escape(string value) => SpecialCharacters().Replace(value, "\\$1");

	[GeneratedRegex(@"([(),|\\]|/i)")]
	private static partial Regex SpecialCharacters();
}
