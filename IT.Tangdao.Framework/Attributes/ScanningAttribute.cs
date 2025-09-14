using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScanningAttribute : Attribute
    {
        public string ContractName { get; private set; }

        public Type ContractType { get; private set; }

        public ScanningAttribute() : this(null, null)
        {
        }

        public ScanningAttribute(Type contractType) : this(null, contractType)
        {
        }

        public ScanningAttribute(string contractName) : this(contractName, null)
        {
        }

        public ScanningAttribute(string contractName, Type contractType)
        {
            ContractName = contractName;
            ContractType = contractType;
        }
    }
}