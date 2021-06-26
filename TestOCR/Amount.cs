using System;
using System.Collections.Generic;
using System.Text;

namespace TestOCR
{
    public class Amount : EntityToFind, IEntityToFind
    {
        public float Value;
        private const string Token = "TOTALE CONTO TELEFONICO";

        public new void CheckValue(string textExtracted)
        {
            if (!this.Found)
            {
                this.MustCheck = this.MustCheck || textExtracted.Equals(Token, StringComparison.InvariantCultureIgnoreCase);
                if (this.MustCheck)
                {
                    if (float.TryParse(textExtracted, out this.Value))
                    {
                        this.Found = true;
                        this.TextValue = textExtracted;
                    }
                }
            }
        }

    }
}
