namespace importacionmasiva.api.net.Models.DB
{
    public class TableDefinition
    {
        public List<Column> Columns { get; set; }
    }

    public class Column
    {
        public string Name { get; set; }
    }
}
