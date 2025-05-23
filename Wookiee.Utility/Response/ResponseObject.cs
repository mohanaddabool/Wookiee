﻿namespace Wookiee.Utility.Response;

public class ResponseObject<T>
{
    public T? Result { get; set; }
    public bool IsSuccess { get; set; }
    public Exception? Exception { get; set; }
    public object? ErrorMessage { get; set; }
}