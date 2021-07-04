using System;
using System.Collections.Generic;
using System.Text;

namespace TestOCR.Model
{
    interface IEntityToFind
    {
        public  void CheckValue(string textExtracted);

        public  string ValueToString();
    }
}
