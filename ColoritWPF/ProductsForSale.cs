using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ColoritWPF
{
    public class ProductsForSale : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        long id;
        string name;
        decimal selfCost;
        decimal cost;
        double warehouse;
        double storage;
        Boolean bottled;
        double maxDiscount;

        double amount = 1;
        double currentDiscount;
        static double clientDiscount;
        decimal productSum;
        decimal productSumWithDiscount;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public long Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Id"));
            }
        }

        public double Amount
        {
            get { return amount; }
            set
            {
                if (value <= (Warehouse + Storage))
                    amount = value;
                else
                    amount = (Warehouse + Storage);

                OnPropertyChanged(new PropertyChangedEventArgs("Amount"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSum"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSumWithDiscount"));
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }

        public double CurrentDiscount
        {
            get
            {
                if (this.ClientDiscount > this.maxDiscount)
                    return this.maxDiscount;
                else
                    return this.ClientDiscount;
            }
            set
            {
                currentDiscount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentDiscount"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSumWithDiscount"));
            }
        }

        public double ClientDiscount
        {
            get { return clientDiscount; }
            set
            {
                clientDiscount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ClientDiscount"));
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentDiscount"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSumWithDiscount"));
            }
        }

        public decimal Cost
        {
            get { return cost; }
            set
            {
                cost = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Cost"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSum"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSumWithDiscount"));
            }
        }

        public decimal ProductSum
        {
            get
            {
                return this.Cost * (decimal)this.Amount;
            }
            set
            {
                productSum = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSum"));
            }
        }

        public decimal ProductSumWithDiscount
        {
            get
            {
                productSumWithDiscount = ((Convert.ToDecimal((100 - this.CurrentDiscount)) / 100) * this.Cost) * (decimal)this.Amount;
                return productSumWithDiscount;
            }
            set
            {
                productSumWithDiscount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSumWithDiscount"));
            }
        }

        public double MaxDiscount
        {
            get { return maxDiscount; }
            set
            {
                maxDiscount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MaxDiscount"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSumWithDiscount"));
            }
        }
        public Boolean Bottled
        {
            get { return bottled; }
            set
            {
                bottled = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Bottled"));
            }
        }
        public double Storage
        {
            get { return storage; }
            set
            {
                storage = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Storage"));
            }
        }
        public double Warehouse
        {
            get { return warehouse; }
            set
            {
                warehouse = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Warehouse"));
            }
        }

        public decimal SelfCost
        {
            get { return selfCost; }
            set
            {
                selfCost = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelfCost"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSum"));
                OnPropertyChanged(new PropertyChangedEventArgs("ProductSumWithDiscount"));
            }
        }
    }
}
