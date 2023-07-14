namespace Hurudza.Data.Models.Models
{
    public class ExcelColumn
    {
        public string Key { get; set; }

        public string Header { get; set; }

        public Type ColumnType { get; set; }

        public int Order { get; set; }

        public string Options { get; set; }

        public ExcelColumn(string key, string header, Type columnType, int order)
        {
            Key = key;
            Header = header;
            ColumnType = columnType;
            Order = order;
        }
    }
}