using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.Domain.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; }
        public BaseEntity(Guid? id)
        {
            Id = id ?? Guid.NewGuid();
        }
        public BaseEntity() { }
    }
}
