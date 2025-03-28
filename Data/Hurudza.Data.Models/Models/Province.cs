﻿using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Province : BaseEntity
{
    public required string Name { get; set; }

    public virtual ICollection<District>? Districts { get; set; }
    public virtual ICollection<Ward> Wards { get; set; }
    public virtual ICollection<Farm> Farms { get; set; }
}