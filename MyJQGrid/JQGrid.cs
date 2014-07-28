using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
namespace MyJQGrid
{
    [Serializable]
    public class JQColModel
    {
        public string field { get; set; }
        public string displayName { get; set;  }
      
        public int width { get; set; }
        public bool hidden { get; set; }
        
        public JQColModel(string pname, int pwidth, bool phidden)
        {
            field = pname;
            displayName = pname;
            width = pwidth;
            hidden = phidden;
        }
    }

    [Serializable]
    public class JQGrid<T> where T : class
    {
        //private JavaScriptSerializer _jsSerializer;
        
        private List<T> _data { get; set; }
        private string _datatype { get { return "local"; } }
        private List<JQColModel> _colModel { get; set; }
        private List<string> _colNames { get; set; }

        public string Data { get { return JsonConvert.SerializeObject(_data); } }
        public string ColModel { get { return JsonConvert.SerializeObject(_colModel); } }
        public string ColNames { get { return JsonConvert.SerializeObject(_colNames); } }
        public bool IsLastRecordSet { get; set; }
        private int? _customWidth;
        private int? _customHeight;
        public int Width
        {
            get
            {
                if (!_customWidth.HasValue)
                {
                    int width = 0;
                    foreach (var col in _colModel)
                    {
                        width += col.width;
                    }
                    return width;
                }
                return _customWidth.Value;
            }
            set
            {

                _customWidth = value;
            }
        }
        public int Height
        {
            get
            {
                if (!_customHeight.HasValue)
                {
                    return _data.Count * 10;
                }
                return _customHeight.Value;
            }
            set
            {

                _customHeight = value;
            }
        }
        private void Initialize(JQGridColParams colParams)
        {
            _colModel = new List<JQColModel>();
            _colNames = new List<string>();
            
            //foreach (var prop in typeof(T).GetProperties())
            //{
            //    _colModel.Add(new JQColModel(prop.Name, colParams.Cols[i].Width, colParams.Cols[i].Hidden));
            //    _colNames.Add(colParams.Cols[i].ColName);
            //    i++;
            //}
            foreach (var col in colParams.Cols)
            {
                _colModel.Add(new JQColModel(col.ColName, col.Width , col.Hidden));
                _colNames.Add(col.ColName);
            }
        }
        public JQGrid(List<T> myData, JQGridColParams colParams)
        {
            _data = myData;
            //_jsSerializer = new JavaScriptSerializer();
            IsLastRecordSet = false;
            if (_data == null)
            {
                _data = new List<T>();

            }
            Initialize(colParams);

        }


    }
}
