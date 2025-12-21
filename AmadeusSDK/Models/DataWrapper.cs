using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Models;

internal class DataWrapper<T>
{
    public DataWrapper(T data)
    {
        this.Data = data;
    }

    public T Data { get; set; }
}
