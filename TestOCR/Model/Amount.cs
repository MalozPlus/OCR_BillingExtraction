using System;
using System.Collections.Generic;
using System.Text;

namespace TestOCR.Model
{
    public class Amount : EntityToFind, IEntityToFind
    {
        public float Value;
        private const string Token = "TOTALE CONTO TELEFONICO";

        public override void CheckValue(string textExtracted)
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

        public override string ValueToString()
        {
            return this.Value.ToString("N2");
        }

    }
}
