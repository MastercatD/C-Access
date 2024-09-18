using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Screen
{
		class Product
		{
				private int _count;
				private string _name;
				private double _price;

				public int count { get { return _count; } }
				public Product(XmlNode productNode)
				{
						IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
						foreach (XmlNode productAttr in productNode.ChildNodes)
						{
								switch (productAttr.Name)
								{
										case "quantity":
												_count = Convert.ToInt32(productAttr.InnerText);
												break;
										case "name":
												_name = productAttr.InnerText;
												break;
										case "price":
												_price = Double.Parse(productAttr.InnerText, formatter);
												break;
								}
						}
				}
				//Получение id продукта в таблице
				public int GetProductIdInDb(OleDbConnection connection)
				{
						string querry = $"SELECT product_id FROM Products WHERE name = \'{this._name}\'";
						OleDbCommand command = new OleDbCommand(querry, connection);
						OleDbDataReader reader = command.ExecuteReader();
						if (reader.HasRows)
						{
								reader.Read();
								//Если товар найден в БД, возвращается его id
								return Convert.ToInt32(reader["product_id"]);
						}
						//Если товар не найден в БД, создаётся запись с ним
						querry = $"INSERT INTO Products (name, price) VALUES (\'{this._name}\', \'{this._price}\')";
						command = new OleDbCommand(querry, connection);
						command.ExecuteNonQuery();
						return GetProductIdInDb(connection);
				}
		}
}
