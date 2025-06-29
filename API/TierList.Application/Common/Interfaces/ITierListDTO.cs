using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TierList.Application.Common.Interfaces;

public interface ITierListDTO
{
    int Id { get; init; }

    string Title { get; init; }
}
