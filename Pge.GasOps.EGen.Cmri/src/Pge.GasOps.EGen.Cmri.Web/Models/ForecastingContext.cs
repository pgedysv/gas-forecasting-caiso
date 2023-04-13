using System;
using System.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Pge.GasOps.EGen.Cmri.Web.Models
{
    public partial class ForecastingContext : DbContext
    {
        public ForecastingContext(ConnectionStringSettings connectionString)
            : base("name=Forecasting")
        {
            this.Database.Connection.ConnectionString = connectionString.ConnectionString;
        }

        public virtual DbSet<CMRI> CMRIs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
