using ExampleAOPInterface;
using System;

namespace ExampleAOPBusiness
{
    public class OneLoadedBusiness : IOneLoadedInterface
    {
        public string GetTextFrom()
        {
            return "Esse é um texto de dentro da business";
        }
    }
}