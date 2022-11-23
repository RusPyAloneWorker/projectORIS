using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTPResponse.Context
{
    public class Context<T>
    {
        public T Value { get; set; }

        public string Name { get; set; }
        public Context() { }
        public Context(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
