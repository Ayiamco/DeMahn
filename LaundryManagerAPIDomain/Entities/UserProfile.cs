using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LaundryManagerAPIDomain.Entities
{
    public enum Gender
    {
        Male,Female,NonBinary
    }
    public class UserProfile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Laundry Laundry { get; set; }
        public Guid LaundryId { get; set; }
        public string Name { get; set; }
        public Location Address { get; set; }
        public Gender Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
