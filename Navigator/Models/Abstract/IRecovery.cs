﻿using System;

namespace Navigator.Models.Abstract
{
    public interface IRecovery
    {
        int RecoveryId { get; set; }
        int UserId { get; set; }
        string Token { get; set; }
        DateTime ValidThrough { get; set; }
    }
}
