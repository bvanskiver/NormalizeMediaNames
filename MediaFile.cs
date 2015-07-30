using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormalizeMediaNames
{
    public class MediaFile
    {
        public MediaFile(string originalName, DateTime date, string extention)
        {
            this.OriginalName = originalName;
            this.Date = date;
            this.Extension = extention;
        }

        public string OriginalName { get; set; }

        public string Name
        {
            get { return this.Date.ToString("yyy-MM-dd HH.mm.ss") + this.Extension; }
        }

        public DateTime Date { get; set; }

        public string Extension { get; set; }
    }
}
