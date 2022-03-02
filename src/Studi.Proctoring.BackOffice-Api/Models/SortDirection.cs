using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models
{
    /// <summary>
    /// Defines the "order by" direction
    /// WARNING: the enums' names musn't be changed because they are used to define the command name in linq expression tree
    /// </summary>
    public enum SortDirection
    {
        OrderBy = 0,
        OrderByDescending = 1,
        //ThenBy = 2,
        //ThenByDescending = 3,
    }
}
