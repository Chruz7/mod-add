using SR.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mod_add.Modelos
{
    public class ReporteFolios
    {
        public ReporteFolios()
        {
            CheqDet = new List<SR_cheqdet>();
        }

        public SR_cheques Cheque { get; set; }
        public List<SR_cheqdet> CheqDet { get; set; }
    }
}
