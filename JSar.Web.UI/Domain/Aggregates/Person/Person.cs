﻿using System;
using JSar.Web.UI;
using JSar.Web.UI.Extensions;

namespace JSar.Web.UI.Domain.Aggregates.Person
{
    public class Person : AggregateRoot
    {
        private string _firstName;
        private string _lastName;
        private string _primaryEmail;
        private string _primaryPhone;

        private Person()
        {
            // Parameterless constructor required for Entity Framework
        }

        // Minimal required person
        public Person(string firstName, string lastName, Guid id) : this(firstName, lastName, "", "", id)
        {
        }
        
        public Person(string firstName, string lastName, string primaryEmail, string primaryPhone, Guid id) : base(id)
        {
            _firstName = firstName.IsNullOrWhiteSpace()
                ? throw new ArgumentException("Person.FirstName cannot be null or white space. EID: CD250A91.", nameof(firstName))
                : firstName.Trim();

            _lastName = lastName.IsNullOrWhiteSpace()
                ? throw new ArgumentException("Person.LastName cannot be null or white space. EID: 514FF925", nameof(lastName))
                : lastName.Trim();

            _primaryEmail = primaryEmail.Trim();
            _primaryPhone = primaryPhone.Trim();

            _domainEventsQueue.Add(
                new PersonCreatedDomainEvent(
                    Guid.NewGuid(),
                    this));
        }


        // Properties

        public string FirstName
        {
            get { return _firstName; }
        }

        public string LastName => _lastName;

        public string Name => FullName;

        public string FullName => _firstName + " " + _lastName;

        public string PrimaryEmail => _primaryEmail;

        public string PrimaryPhone => _primaryPhone;

        // Behaviors

        public DomainErrorList UpdateName(string firstName, string lastName)
        {
            // Validate 

            var errors = new DomainErrorList();

            if (firstName.IsNullOrWhiteSpace())
                errors.Add("FirstName cannot be empty.");

            if (lastName.IsNullOrWhiteSpace())
                errors.Add("LastName cannot be empty");

            if (errors)
                return errors;

            // Execute

            _firstName = firstName.Trim();
            _lastName = lastName.Trim();
            
            // Notify

            _domainEventsQueue.Add(
                new PersonNameUpdatedDomainEvent(
                    Guid.NewGuid(),
                    Id,
                    Name,
                    FirstName,
                    LastName));

            return errors;
        }

        public DomainErrorList UpdatePrimaryEmail(string email)
        {
            // Validate

            var errors = new DomainErrorList();

            if (email.IsNullOrWhiteSpace())
                errors.Add("Email address cannot be empty.");

            if (errors) return errors;

            // Execute

            _primaryEmail = email.Trim();

            // Notify

            _domainEventsQueue.Add(
                new PersonPrimaryEmailUpdatedDomainEvent(
                    Guid.NewGuid(),
                    Id,
                    Name,
                    _primaryEmail));

            return errors;
        }

        public void UpdatePrimaryPhone(string phone)
        {
            // Execute

            _primaryPhone = phone.Trim();

            // Notify

            _domainEventsQueue.Add(
                new PersonPrimaryPhoneUpdatedDomainEvent(
                    Guid.NewGuid(),
                    Id,
                    Name,
                    _primaryPhone));
        }
    }
}
