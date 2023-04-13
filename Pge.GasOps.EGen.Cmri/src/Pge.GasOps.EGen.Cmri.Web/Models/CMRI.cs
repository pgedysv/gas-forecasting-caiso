using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Pge.GasOps.EGen.Cmri.Web.Models
{
    [Table("CMRI")]
    public partial class CMRI
    {
        public DateTime StartTime { get; set; }

        [Key]
        [Column(Order = 0)]
        public DateTime EndTime { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(5)]
        public string DA { get; set; }

        public double MMCF { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }
    }
}
