using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exadel.ReportHub.SDK.DTOs.Client;

public class ClientDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public IList<Guid> CustomerIds { get; set; }
}
