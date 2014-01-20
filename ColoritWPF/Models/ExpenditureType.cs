namespace ColoritWPF.Models
{
    public class ExpenditureType
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public ExpenditureType(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }

    public enum ExpenditureTypeEnum
    {
        Rent,
        Salary,
        Tax,
        Expenses,
        Other
    }
}
