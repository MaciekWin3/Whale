namespace Whale.Utils.Extensions
{
    public static class ErrorExtensions
    {
        public static Result ToResult(this string error)
        {
            return string.IsNullOrEmpty(error) ? Result.Ok() : Result.Fail(error);
        }

        public static Result<T> ToResult<T>(this string error) => Result.Fail<T>(error);
    }
}
