namespace Results;

public class ResultOperation<T>
{
    public bool Success;
    public string Message;

    public string title;

    public T Data;

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

public class ResultOperation
{
    public bool Success;
    public string Message;

    public string title;

    public static ResultOperation Ok(string message = null)
        {
            return new ResultOperation
            {
                Success = true,
                Message = message
            };
        }

        public static ResultOperation Fail(string message)
        {
            return new ResultOperation
            {
                Success = false,
                Message = message
            };
        }
}