using System;
using System.Collections.Generic;
using System.Text;

namespace TestOCR.Model
{
    public class Amount : EntityToFind, IEntityToFind
    {
        public float Value;
        private List<string> lstToken = new List<string>() { "TOTALE CONTO TELEFONICO", "Importo da pagare" };

        public override void CheckValue(string textExtracted)
        {
            if (!this.Found)
            {
                this.MustCheck = this.MustCheck
                        || lstToken.Exists(x => x.Equals(textExtracted, StringComparison.InvariantCultureIgnoreCase));
                if (this.MustCheck)
                {
                    if (float.TryParse(textExtracted.ToLower().Replace("euro", "").Trim(), out this.Value))
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
