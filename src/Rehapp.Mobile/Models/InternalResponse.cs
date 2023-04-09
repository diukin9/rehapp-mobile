namespace Rehapp.Mobile.Models;

public class InternalResponse
{
    public bool IsSuccess { get; protected set; }
    public Exception Exception { get; protected set; }

    public InternalResponse Success()
    {
        IsSuccess = true;
        return this;
    }

    public InternalResponse Failure(Exception exception = null)
    {
        Exception = exception;
        IsSuccess = false;
        return this;
    }
}

public class InternalResponse<T> : InternalResponse
{
    public T Data { get; protected set; }

    public InternalResponse<T> Success(T data)
    {
        Data = data;
        IsSuccess = true;
        return this;
    }

    public new InternalResponse<T> Failure(Exception exception = null)
    {
        Exception = exception;
        IsSuccess = false;
        return this;
    }
}