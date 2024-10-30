using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equin.ApplicationFramework
{
    public interface IMyObjectView
    {
        object Object { get; }
        Type ObjectType { get; }
    }
}
