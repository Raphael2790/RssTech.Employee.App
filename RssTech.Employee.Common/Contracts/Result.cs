namespace RssTech.Employee.Common.Contracts;


public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public static Result Success() 
        => new() { IsSuccess = true };
    public static Result Error(string message) 
        => new() { IsSuccess = false, Message = message };
}


public sealed class Result<T> : Result
{
    public T Data { get; set; }

    public static Result<T> Success(T data) 
        => new() { IsSuccess = true, Data = data };

    public new static Result<T> Error(string message) 
        => new() { IsSuccess = false, Message = message };
}
