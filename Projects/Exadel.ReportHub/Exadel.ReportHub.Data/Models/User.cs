using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exadel.ReportHub.Data.Models;

public class User : IDocument
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }

    public string Password { get; set; }

    public bool IsActive { get; set; } = true;
}
