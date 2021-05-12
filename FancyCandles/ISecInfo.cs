using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    public interface ISecInfo
    {
        string ClassCode { get; }
        string SecCode { get; }
        string FullName { get; }
    }
}
