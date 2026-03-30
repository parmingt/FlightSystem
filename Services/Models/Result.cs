using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Services.Models;

public record Result<T>(T? Data, bool Success, string? ErrorMessage = null)
{
    public static Result<T> SuccessResult(T data) => new Result<T>(data, true);
    public static Result<T> FailureResult(string errorMessage) => new Result<T>(default, false, errorMessage);

    public static implicit operator Result<T>(string error) => new Result<T>(error);

    public static implicit operator Result<T>(Failure f)
    {
        return Result<T>.FailureResult(f.Message ?? "");
    }
}

public record Failure(string? Message);