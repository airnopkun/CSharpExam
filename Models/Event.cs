using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Exam.Validations;

namespace Exam.Models
{
    public class Event
    {
        [Key]
        public int EventId {get; set;}
        [Required]
        public string Title {get; set;}
        [Required]
        [DataType(DataType.DateTime)]
        [Future]
        public DateTime StartDate {get; set;}
        [Required]
        public int Duration {get; set;}
        [Required]
        public string DurationUnit {get; set;}
        [Required]
        public string Description {get; set;}
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public List <RSVP> Participants {get; set;}
        [Required]
        public int UserId {get; set;}
        public User Creator {get; set;}
    }
}