namespace Services;

public class CitiesService
{
	private readonly List<string> _cities;

	public CitiesService()
	{
		_cities =
		[
			"London",
			"Paris",
			"New York",
			"Rome",
			"Sydney",
			"Los Angeles",
			"San Francisco",
			"Chicago",
			"Barcelona",
			"Amsterdam",
		];
	}

	public List<string> GetCities()
	{
		return _cities;
	}
}
