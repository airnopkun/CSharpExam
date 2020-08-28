using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam.Models
{
    public class RSVP
    {
        [Key]
        public int RSVPId {get; set;}
        public int EventId {get; set;}
        public Event EventOfRSVP {get; set;}
        public int UserId {get; set;}
        public User Attendee {get; set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}