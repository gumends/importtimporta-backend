using System;

namespace Domain.Entities
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName)
            : base($"{entityName} não encontrado.")
        {
        }
    }
}
