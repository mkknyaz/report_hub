using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exadel.ReportHub.Data.Models;

public class Customer : IDocument
{
    public Guid Id { get; set; }

    public string Country { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }
}
