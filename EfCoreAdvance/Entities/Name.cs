﻿using CSharpFunctionalExtensions;

namespace EfCoreAdvance.Entities
{
    public class Name : ValueObject 
    {
        public string First { get; }
        public string Last { get; }
        private Name(string first, string last) 
        {
            First = first;
            Last = last;    
        }

        public static Result<Name> Create(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<Name>("First name should not be empty");
            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<Name>("Last name should not be empty");

            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (firstName.Length > 200)
                return Result.Failure<Name>("First name is too long");
            if (lastName.Length > 200)
                return Result.Failure<Name>("Last name is too long");

            return Result.Success(new Name(firstName, lastName));
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return First;
            yield return Last;
        }
    }
}
