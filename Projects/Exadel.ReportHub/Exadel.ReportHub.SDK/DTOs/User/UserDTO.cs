﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exadel.ReportHub.SDK.DTOs.User;

public class UserDTO
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }
}
