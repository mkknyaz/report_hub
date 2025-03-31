using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Exadel.ReportHub.SDK.DTOs.User;

public class CreateUserDTO
{
    public string Email { get; set; }

    public string FullName { get; set; }

    public SecureString Password { get; set; }
}
