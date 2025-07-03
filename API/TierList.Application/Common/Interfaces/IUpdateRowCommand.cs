using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Domain.Entities;

namespace TierList.Application.Common.Interfaces;

public interface IUpdateRowCommand
{
    int Id { get; set; }

    int ListId { get; set; }

    void UpdateRow(TierRowEntity rowEntity);
}
