using System;
using System.Runtime.Serialization;

namespace NHibernate_Deep_Dive.Entities
{
    [Serializable]
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public byte[] DbVersion { get; set; }
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
    }
}