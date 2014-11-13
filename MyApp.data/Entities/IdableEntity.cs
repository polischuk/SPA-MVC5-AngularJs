using System;
using System.ComponentModel.DataAnnotations;

namespace MyApp.data.Entities
{
    public abstract class IdableEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        protected IdableEntity()
        {
            Id = Guid.NewGuid();
        }
    }
 
}
