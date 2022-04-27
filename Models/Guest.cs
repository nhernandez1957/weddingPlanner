using System;
using System.ComponentModel.DataAnnotations;

namespace weddingPlanner.Models
{
    public class Guest
    {
        [Key]
        public int GuestId {get;set;}
        public int UserId {get;set;}
        public User Attendee {get;set;}
        public int WeddingId {get;set;}
        public Wedding Wedding {get;set;}
        public int GuestTotal {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}