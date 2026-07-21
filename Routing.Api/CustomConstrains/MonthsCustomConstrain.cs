using System.Text.RegularExpressions;

namespace Routing.Api.CustomConstrains;

public partial class MonthsCustomConstrain : IRouteConstraint
{
	[GeneratedRegex("^(apr|jul|oct|jan)$")]
	private static partial Regex MonthRegex();

	public bool Match(
		HttpContext? httpContext,
		IRouter? route,
		string routeKey,
		RouteValueDictionary values,
		RouteDirection routeDirection
	)
	{
		if (!values.TryGetValue(routeKey, out var value))
		{
			return false;
		}

		string? monthValue = Convert.ToString(value);

		return monthValue is not null && MonthRegex().IsMatch(monthValue);
	}
}
