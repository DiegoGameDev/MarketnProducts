namespace ResultOperation;

public class ResultOperation<T>
{
    public bool Success;
    public string Message;

    public string title;

    public T Data;

    public ResultOperation<T> Result(T obj, string message, string error = "")
    {
        Data = obj;
        this.Message = message;
        title = error;

        return this;
    }

    public static ResultOperation<T> Ok(T data, string message = null)
        {
            return new ResultOperation<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ResultOperation<T> Fail(string message)
        {
            return new ResultOperation<T>
            {
                Success = false,
                Message = message
            };
        }
}