using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Screen
{
		class User
		{
				private string _name;
				private string _secondName;
				private string _middleName;
				private string _email;
				private string _phone;

				public User(XmlNode userNode)
				{
						foreach (XmlNode userAttr in userNode.ChildNodes)
						{
								switch (userAttr.Name)
								{
										case "fio":
												string [] fio = userAttr.InnerText.Split(' ');
												_secondName = fio[0];
												_name = fio[1];
												_middleName = fio[2];
												break;
										case "email":
												_email = userAttr.InnerText;
												break;
								}
						}
				}
				public int GetUserIdInDb(OleDbConnection connection)
				{
						string querry = $"SELECT user_id FROM Users WHERE first_name = \'{this._name}\' AND second_name = \'{this._secondName}\' AND email = \'{this._email}\'";
						OleDbCommand command = new OleDbCommand(querry, connection);
						OleDbDataReader reader = command.ExecuteReader();
						if (reader.HasRows)
						{
								reader.Read();
								//Если есть запись с данными пользователя в БД, возвращается его id
								return Convert.ToInt32(reader["user_id"]);
						}
						//При отсутствии записи в БД, она добавляется запросом
						querry = $"INSERT INTO Users (first_name, second_name, email) VALUES (\'{this._name}\', \'{this._secondName}\', \'{this._email}\')";
						command = new OleDbCommand(querry, connection);
						command.ExecuteNonQuery();
						return GetUserIdInDb(connection);
				}

		}
}
