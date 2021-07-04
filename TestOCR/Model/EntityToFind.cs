using System;
using System.Collections.Generic;
using System.Text;

namespace TestOCR.Model
{
    public abstract class EntityToFind: IEntityToFind
    {
        public bool MustCheck = false;
        public bool Found = false;
        public string TextValue = null;

        public abstract void CheckValue(string textExtracted);

        public abstract string ValueToString();
    }
}
