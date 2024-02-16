namespace QualityStation.API.Services.Common
{
	public enum ResultType
	{
		Ok,
		NotFound,
		Conflict,
		BadRequest,
	}; 

	public class ServiceResult<T>
	{
		public T? Value { get; set; }
		public ResultType Type { get; set; } = ResultType.Ok;
		public string? Error { get; set; }
	}
}
