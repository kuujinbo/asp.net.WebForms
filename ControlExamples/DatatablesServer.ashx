<%@ WebHandler Language="C#" Class="DatatablesServer" %>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using kuujinbo.asp.net.WebForms;
using Newtonsoft.Json;
using System.Threading;

public class DatatablesServer : IHttpHandler, IRequiresSessionState {
    private JqDataTablesHelper helper = new JqDataTablesHelper(7);

    public void ProcessRequest(HttpContext context)
    {
        HttpResponse Response = context.Response;
        HttpRequest Request = context.Request;
        Response.ContentType = "application/json";
        Thread.Sleep(500);    
        _getTestData(Request, Response);

    }
    public bool IsReusable
    {
        get { return false; }
    }

    private void _getTestData(HttpRequest Request, HttpResponse Response)
    {
        helper.GetClientParams(Request);

        string dataFile = "~/app_data/arrays.json".MapPath();
        string json = File.ReadAllText(dataFile);
        var dataFromFile = JsonConvert.DeserializeObject<string[][]>(json);

        if (helper.HasSearchValue)
        {
            var contains = dataFromFile
                .Where(s => s[helper.SearchColumn]
                .IndexOf(helper.SearchValue, StringComparison.OrdinalIgnoreCase) != -1);

            helper.RecordsTotal = contains.Count();
            var data = helper.SortAcsending
                ? contains.OrderBy(x => x[helper.SortColumn])
                    .Skip(helper.start)
                    .Take(helper.length)
                : contains.OrderByDescending(x => x[helper.SortColumn])
                    .Skip(helper.start)
                    .Take(helper.length)
            ;            
            Response.Write(helper.GetJson(data));
        }
        else
        {
            helper.RecordsTotal = dataFromFile.Length;
            var data = helper.SortAcsending
              ? dataFromFile.OrderBy(x => x[helper.SortColumn])
                .Skip(helper.start)
                .Take(helper.length)
              : dataFromFile.OrderByDescending(x => x[helper.SortColumn])
                .Skip(helper.start)
                .Take(helper.length)
            ;
            Response.Write(helper.GetJson(data));
        }
    }


    public class JqDataTablesHelper
    {
        // URL-encoded parameters __ALWAYS__ sent to server
        const string DRAW = "draw";
        const string START = "start";
        const string LENGTH = "length";

        const string ORDER_COLUMN = "order[0][column]";
        const string ORDER_DIR = "order[0][dir]";
        const string ORDER_ASC = "asc";
        const string ORDER_DESC = "desc";

        const string SEARCH_VALUE = "search[value]";
        const string SEARCH_REGEX = "search[regex]";
        const string COLUMNS_DATA = "columns[{0}][data]";
        const string COLUMNS_NAME = "columns[{0}][name]";
        const string COLUMNS_SEARCHABLE = "columns[{0}][searchable]";
        const string COLUMNS_ORDERABLE = "columns[{0}][orderable]";
        const string COLUMNS_SEARCH_VALUE = "columns[{0}][search][value]";
        const string COLUMNS_SEARCH_REGEX = "columns[{0}][search][regex]";

        public int NumberOfColumns { get; set; }

        private int _draw, _start, _length, _sortColumn;
        public int draw { get { return _draw; } }
        public int start { get { return _start; } }
        public int length { get { return _length; } }
        public int RecordsTotal { get; set; }

        public int SortColumn { get { return _sortColumn; } }
        public bool SortAcsending { get; private set; }
        public bool HasSearchValue { get; private set; }
        public int SearchColumn { get; private set; }
        public string SearchValue { get; private set; }

        public JqDataTablesHelper(int numberOfColumns)
        {
            NumberOfColumns = numberOfColumns;
        }

        public void GetClientParams(HttpRequest Request)
        {
            Int32.TryParse(Request.Params[DRAW], out _draw);
            Int32.TryParse(Request.Params[START], out _start);
            Int32.TryParse(Request.Params[LENGTH], out _length);
            Int32.TryParse(Request.Params[ORDER_COLUMN], out _sortColumn);
            string sortOrder = Request.Params[ORDER_DIR];
            SortAcsending = !string.IsNullOrEmpty(sortOrder) 
                && sortOrder == ORDER_ASC ? true : false;

            for (int i = 0; i < NumberOfColumns; ++i)
            {
                string paramName = string.Format(COLUMNS_SEARCH_VALUE, i);
                string search = Request.Params[paramName];
                if (!string.IsNullOrEmpty(search))
                {
                    HasSearchValue = true;
                    SearchColumn = i;
                    SearchValue = search;
                    break;
                }
            }
        }

        public string GetJson<T>(IEnumerable<T> data)
        {
            var result = new
            {
                draw = this.draw,
                recordsTotal = this.RecordsTotal,
                recordsFiltered = this.RecordsTotal,
                data = data
            };
            return JsonConvert.SerializeObject(result);
        }
    }
}