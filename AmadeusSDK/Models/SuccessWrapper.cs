using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Models;

public class SuccessWrapper<T>
{
    public SuccessWrapper(T data)
    {
        Data = data;
    }

    public SuccessWrapper(string failureReason)
    {
        Data = default(T);
        Success = false;
        FailureReason = failureReason;
    }

    public T Data { get; set; }
    public bool Success { get; private set; } = true;
    public string FailureReason { get; init; }
}
