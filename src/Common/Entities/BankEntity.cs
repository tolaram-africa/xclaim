﻿using System.ComponentModel.DataAnnotations;
using XClaim.Common.Base;

namespace XClaim.Common.Entities;

public class BankEntity : BaseEntity {
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;
}