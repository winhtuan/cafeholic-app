using System;
using System.Collections.Generic;
using CAFEHOLIC.Utils;

namespace CAFEHOLIC.Model;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? DrinkId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Drink? Drink { get; set; }

    public virtual Order? Order { get; set; }
}

public class CartItem : ObservableObject
{
    public Drink Drink { get; set; }
    public decimal TotalPrice => (Drink.Price ?? 0) * Quantity;

    private int quantity = 1;
    public int Quantity
    {
        get => quantity;
        set
        {
            if (value < 1) value = 1; 
            SetProperty(ref quantity, value);
            OnPropertyChanged(nameof(TotalPrice)); 
        }
    }
}