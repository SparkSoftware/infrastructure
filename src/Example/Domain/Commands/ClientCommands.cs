﻿using System;
using System.Runtime.Serialization;
using Spark.Infrastructure;
using Spark.Infrastructure.Commanding;

namespace Example.Domain.Commands
{
    [DataContract]
    public sealed class RegisterClient : Command
    {
        [DataMember(Name = "n")]
        public String Name { get; private set; }

        public RegisterClient(String name)
        {
            Verify.NotNullOrWhiteSpace(name, "name");

            Name = name;
        }
    }
}
