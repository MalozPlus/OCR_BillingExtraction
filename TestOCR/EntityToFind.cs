using System;
using System.Collections.Generic;
using System.Text;

namespace TestOCR
{
    public class EntityToFind: IEntityToFind
    {
        public bool MustCheck = false;
        public bool Found = false;
        public string TextValue = null;

        public void CheckValue(string textExtracted)
        {
        }

        public string ValueToString()
        {
            return "";
        }
    }
}
