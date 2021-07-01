using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TestOCR
{
    public class EmissionDate : EntityToFind, IEntityToFind
    {
        public DateTime Value;
        private const string Token = "Data emissione fattura";

        public new void CheckValue(string textExtracted)
        {
            if (!this.Found)
            {
                this.MustCheck = this.MustCheck || textExtracted.Equals(Token, StringComparison.InvariantCultureIgnoreCase);
                if (this.MustCheck)
                {
                    var culture = CultureInfo.CreateSpecificCulture("it-IT");
                    if (DateTime.TryParse(textExtracted, culture, DateTimeStyles.None, out this.Value))
                    {
                        this.Found = true;
                        this.TextValue = textExtracted;
                    }
                }
            }
        }

        public new string ValueToString()
        {
            return this.Value.ToString("dd/MM/yyyy");
        }

    }
}
