using Whale.Utils.Extensions;

namespace Whale.Utils
{
    public class Result<T> : Result
    {
        public T? Value { get; set; }

        protected internal Result(T? value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static implicit operator Result<T>(string error) => error.ToResult<T>();

        public static implicit operator Result<T>(T value) => Ok(value);

        public T GetValue()
        {
            if (IsSuccess && Value != null)
            {
                return Value;
            }
            else
            {
                throw new ArgumentNullException(nameof(Value));
            }
        }
    }
}
