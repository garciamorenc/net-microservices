using System;

namespace Actio.Services.Activities.Domain.Models
{
    public class Category
    {
        public Guid Id { get; protected set; }
        public string Name { get; protected set; }

        protected Category() { }

        public Category(string name)
        {
            this.Id = Guid.NewGuid();
            this.Name = name.ToLowerInvariant();
        }
    }
}