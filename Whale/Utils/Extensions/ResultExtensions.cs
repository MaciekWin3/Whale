namespace Whale.Utils.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> MapFail<T>(this Result result)
        {
            if (result is null)
            {
                throw new InvalidOperationException();
            }

            return Result.Fail<T>(result.Error);
        }
    }
}
