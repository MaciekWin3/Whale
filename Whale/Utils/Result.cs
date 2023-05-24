using Whale.Utils.Extensions;

namespace Whale.Utils
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string error)
        {
            if (error is null)
            {
                throw new InvalidOperationException();
            }

            if (isSuccess && !string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException();
            }

            if (!isSuccess && string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Fail(string error)
        {
            return new Result(false, error);
        }

        public static Result<T> Fail<T>(string error)
        {
            return new Result<T>(default, false, error);
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public static implicit operator Result(string error) => error.ToResult();
    }
}
