using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exam.Validations
{
    public class StrongerPassAttribute : ValidationAttribute
    {
        string Digits = "0123456789";
        string Special = "!#$%&'()*+,-./:;<=>?@[]^_`{|}~";
        string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool ContainsDigit = false;
            bool ContainsSpecial = false;
            bool ContainsLetter = false;
            foreach(var c in (string)value)
            {
                if(Digits.Contains(c))
                {
                    ContainsDigit = true;
                }
                if(Special.Contains(c))
                {
                    ContainsSpecial = true;
                }
                if(Alphabet.Contains(c))
                {
                    ContainsLetter = true;
                }
            }
            if(ContainsDigit && ContainsLetter && ContainsSpecial)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Password must contain at least one of each of the following: letter, number, and special character");
            }
        }
    }
}