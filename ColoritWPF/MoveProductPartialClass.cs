namespace ColoritWPF
{
    public partial class MoveProduct
    {
        public string Name
        {
            get { return Product.Name; }
        }

        public string Article
        {
            get { return this.Product.Article; }
        }

        public decimal Cost
        {
            get { return Product.Cost; }
        }
    }
}
