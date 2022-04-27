using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace weddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}

        [Required]
        public string WedderOne {get;set;}
        [Required]
        public string WedderTwo {get;set;}
        [Required]
        public DateTime WeddingDate {get;set;}
        [Required]
        public string WeddingAddress {get;set;}

        // One to many connection
        public int UserId {get;set;}
        public User Wedder {get;set;}

        // Many to many connection
        public List<Guest> WeddingGuests {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}