﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoImpact.DataModel.Dtos
{
    public class LoginRequestDto
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
