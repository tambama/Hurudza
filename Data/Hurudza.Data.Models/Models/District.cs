﻿using Hurudza.Data.Models.Base;
using System.Collections;

namespace Hurudza.Data.Models.Models
{
    public class District : BaseEntity
    {
        public required string Name { get; set; }
        public required string ProvinceId { get; set; }
        
        public virtual Province? Province { get; set; }
        public virtual ICollection<Ward>? Wards { get; set; }
        public virtual ICollection<LocalAuthority> LocalAuthorities { get; set; }
        public virtual ICollection<Farm>? Farms { get; set; }
        
    }
}