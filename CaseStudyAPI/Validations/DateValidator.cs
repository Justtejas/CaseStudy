﻿using CaseStudyAPI.DTO;
using System.ComponentModel.DataAnnotations;

namespace CaseStudyAPI.Validations
{

    public static class DateValidator
    {
        public static ValidationResult ValidateDateOfBirth(DateTime dateOfBirth, ValidationContext context)
        {
            var today = DateTime.Now;

            if (dateOfBirth > today)
            {
                return new ValidationResult("Date of Birth cannot be in the future.");
            }

            if (dateOfBirth < new DateTime(1900, 1, 1))
            {
                return new ValidationResult("Date of Birth cannot be before 1900.");
            }

            var age = today.Year - dateOfBirth.Year;
            if (age < 18)
            {
                return new ValidationResult("Age must be 18 or older.");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var instance = (RegisterJobSeekerDTO)context.ObjectInstance;

            if (endDate < instance.StartDate)
            {
                return new ValidationResult("End Date cannot be earlier than Start Date.");
            }

            if (endDate > DateTime.Now)
            {
                return new ValidationResult("End Date cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}