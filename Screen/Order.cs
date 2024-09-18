using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen
{
		class Order
		{
				private int _orderId;
				private DateTime _date;
				private double _total;
				private string _address;
				private string _status;
				private List<Product> _cart;
				private User _user;
				public Order()
				{
						_cart = new List<Product>();
						_status = "package";
						_address = "unknown";
				}
				public int orderId { get { return _orderId; } set { _orderId = value; } }
				public User user { get { return _user; } set { _user = value; } }
				public DateTime date { get { return _date; } set { _date = value; } }
				public double total { get { return _total; } set { _total = value; } }
				public string address { get { return _address; } set { _address = value; } }
				public string status { get { return _status; } set { _status = value; } }
				public List<Product> cart { get { return _cart; } }
				public void AddProduct(Product product)
				{
						_cart.Add(product);
				}

		}
}
