using HeliumParty.RadixDLT.Identity;
using HeliumParty.RadixDLT.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeliumParty.RadixDLT.Actions
{
    public enum TokenSupplyType
    {
        Fixed,
        Mutable
    }


    public class CreateTokenAction : IAction
    {
        public string Name { get; protected set; }
        public RRI RRI { get; protected set; }
        public string Description { get; protected set; }
        public string IconUrl { get; set; }
        public BigDecimal InitialSupply { get; protected set; }
        public BigDecimal Granularity { get; protected set; }
        public TokenSupplyType TokenSupplyType { get; protected set; }

        public CreateTokenAction(string name, RRI rRI, string description,string iconUrl, BigDecimal initialSupply, BigDecimal granularity, TokenSupplyType tokenSupplyType)
        {
            if (initialSupply < 0)
                throw new ArgumentException(nameof(initialSupply) + " cannot be less then 0");

            if(tokenSupplyType == TokenSupplyType.Fixed && initialSupply == 0)
                throw new ArgumentException(nameof(initialSupply) + " Must be greater then 0");

            Name = name;
            RRI = rRI;
            IconUrl = iconUrl;
            InitialSupply = initialSupply;
            Description = description;
            Granularity = granularity;
            TokenSupplyType = tokenSupplyType;
        }

        public override string ToString()
        {
            return $"Create Token {RRI} {TokenSupplyType}";
        }
    }
}
