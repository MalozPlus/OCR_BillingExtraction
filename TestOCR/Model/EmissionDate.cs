using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TestOCR.Model
{
    public class EmissionDate : EntityToFind, IEntityToFind
    {
        public DateTime Value;
        private List<string> lstToken = new List<string>() { "Data emissione fattura", "Data di scadenza" };

        public override void CheckValue(string textExtracted)
        {
            if (!this.Found)
            {
                this.MustCheck = this.MustCheck
                        || lstToken.Exists(x => x.Equals(textExtracted, StringComparison.InvariantCultureIgnoreCase));
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

        public override string ValueToString()
        {
            return this.Value.ToString("dd/MM/yyyy");
        }

    }
}
