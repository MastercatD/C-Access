using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Globalization;


namespace Screen
{

		internal class Program
		{
				//Чтение XML файла
				static public List<Order> ReadXML(string filePath)
				{
						IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
						List<Order> orders = new List<Order>();
						//Открытие XML
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(filePath);
						XmlElement elmRoot = xmlDocument.DocumentElement;
						foreach (XmlNode orderNode in elmRoot)
						{
								Order order = new Order();
								foreach (XmlNode orderInfoNode in orderNode.ChildNodes)
								{
										switch (orderInfoNode.Name)
										{
												case "no":
														order.orderId = Convert.ToInt32(orderInfoNode.InnerText);
														break;
												case "reg_date":
														order.date = Convert.ToDateTime(orderInfoNode.InnerText);
														break;
												case "sum":
														order.total = Double.Parse(orderInfoNode.InnerText, formatter);
														break;
												case "product":
														Product product = new Product(orderInfoNode);
														order.AddProduct(product);
														break;
												case "user":
														User user = new User(orderInfoNode);
														order.user = user;
														break;
										}
								}
								orders.Add(order);
						}
						return orders;
				}

				//Загрузка данных заказа в БД
				static public bool LoadOrderIntoDb(Order order)	
				{
						OleDbConnection connection;
						try
						{
								//Подключение к БД
								string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=../../Base//Shop.mdb";
								connection = new OleDbConnection(connectionString);
								connection.Open();
						}
						catch (Exception ex)
						{
								Console.WriteLine(ex.Message);
								return false;
								
						}


						string querry = $"SELECT order_id FROM Orders WHERE order_id = {order.orderId}";
						OleDbCommand command = new OleDbCommand(querry, connection);
						OleDbDataReader reader = command.ExecuteReader();
						if (!reader.HasRows)
						{
								//Создание записи заказа при остутствии заказа с таким же id
								querry = $"INSERT INTO Orders VALUES (\'{order.orderId}\', \'{order.user.GetUserIdInDb(connection)}\', \'{order.date}\', \'{order.total}\', \'{order.address}\', \'{order.status}\')";
								command = new OleDbCommand(querry, connection);
								command.ExecuteNonQuery();
						}
;
						//Создание записей с содержимым заказа
						foreach (Product product in order.cart)
						{
								querry = $"SELECT order_id FROM OrderList WHERE order_id = {order.orderId} AND product_id = {product.GetProductIdInDb(connection)}";
								command = new OleDbCommand(querry, connection);
								reader = command.ExecuteReader();
								if (reader.HasRows)
								{
										//Обновление при существовании предмета в корзине
										querry = $"UPDATE OrderList SET OrderList.count = {product.count} WHERE order_id = {order.orderId} AND product_id = {product.GetProductIdInDb(connection)}";
								}
								else
								{
										//Добавление предмета в корзину
										querry = $"INSERT INTO OrderList VALUES (\'{order.orderId}\', \'{product.GetProductIdInDb(connection)}\', \'{product.count}\')";
								}
								command = new OleDbCommand(querry, connection);
								command.ExecuteNonQuery();
						}
						connection.Close();
						return true;
				}

				static void Main(string[] args)
				{
						//Путь до XML файла
						string filePath = "../../Base/XMLrequests/addOrders.xml";
						if (File.Exists(filePath))
						{
								//Чтение
								List<Order> orders = ReadXML(filePath);
								//Запись в БД
								foreach (Order order in orders)
								{
										LoadOrderIntoDb(order);
								}
						}
						else
						{
								Console.WriteLine("Не удалость найти XML файл");
						}

				}

	
		}
}
