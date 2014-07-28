using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyJQGrid
{
    public class JQGridCol
    {
        public string ColName { get; set; }
        public int Width { get; set; }
        public bool Hidden { get; set; }

    }
    public class JQGridColParams
    {
        public List<JQGridCol> Cols { get; set; }
        public JQGridColParams()
        {
            Cols = new List<JQGridCol>();
        }

        public void AddCol(string colName, int width)
        {
            AddCol(colName, width, false);
        }
        public void AddCol(string colName, int width, bool isHidden)
        {
            Cols.Add(new JQGridCol() { ColName = colName, Width = width, Hidden = isHidden });
            
        }
    }
}
