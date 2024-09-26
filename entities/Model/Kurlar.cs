using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Model
{
	public class Kurlar
	{
		public int kurId { get; set; }
		public string Birim { get; set; }
		public string Dovizcins { get; set; }
		public string Dovizalis { get; set; }
		public string Dovizsatis { get; set; }
		public string Efektifalis { get; set; }
		public string Efektifsatis { get; set; }
		public Nullable<int> TarihId { get; set; }

		public virtual Tarih Tarih { get; set; }
	}
}
