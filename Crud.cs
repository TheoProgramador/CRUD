using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.Crud
{
    public class Crud<TEntity> where TEntity : class
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["conexao"].ToString();

        public void Add(TEntity t)
        {
            var Valores = "";

            var Campos = string.Join(",", t.GetType().GetProperties().Select(x => x.Name).Where(x => x != "ID"));

            var Tabela = t.ToString().Split('.').Last();

            bool passou = false;
            foreach (var item in t.GetType().GetProperties())
            {
                if (passou)
                {
                    if (Valores != "")
                    {
                        Valores += ",";
                    }
                    if (item.PropertyType.Name == "String" || item.PropertyType.Name == "DateTime")
                    {
                        Valores += "'" + (item.GetValue(t) ?? "").ToString() + "'";
                    }
                    else
                    {
                        Valores += (item.GetValue(t) ?? "").ToString();
                    }
                }
                else
                {
                    passou = true;
                }
            }

            var query = "insert into " + Tabela.ToString() + " (" + Campos + ") values (" + Valores + ")";



            using (var sqlConnection = new MySqlConnection(ConnectionString))
            {
                var SaidaSQL = sqlConnection.Query(query);
            }
        }

        public List<TEntity> GetAll(TEntity t)
        {
            var Tabela = t.ToString().Split('.').Last();
            var query = "select * from " + Tabela;

            using (var sqlConnection = new MySqlConnection(ConnectionString))
            {
                var saida = sqlConnection.Query<TEntity>(query);
                return saida.ToList();
            }
        }

        public void Delete(TEntity t, int ID)
        {
            var Tabela = t.ToString().Split('.').Last();
            var query = "select from " + Tabela + " where ID = " + ID;


            using (var sqlConnection = new MySqlConnection(ConnectionString))
            {
                var saida = sqlConnection.Execute(query);
            }
        }

        public List<TEntity> Find(TEntity t, string where)
        {
            var Tabela = t.ToString().Split('.').Last();
            var query = "select * from " + Tabela + " where " + where;


            using (var sqlConnection = new MySqlConnection(ConnectionString))
            {
                var saida = sqlConnection.Query<TEntity>(query);
                return saida.ToList();
            }
        }

        public TEntity GetById(TEntity t, int ID)
        {
            var Tabela = t.ToString().Split('.').Last();
            var query = "select * from " + Tabela + " where ID = " + ID;

            using (var sqlConnection = new MySqlConnection(ConnectionString))
            {
                var saida = sqlConnection.Query<TEntity>(query).FirstOrDefault();
                return saida;
            }
        }

        public void Update(TEntity t)
        {
            var Valores = "";

            var ID = "";

            var Tabela = t.ToString().Split('.').Last();

            bool passou = false;
            foreach (var item in t.GetType().GetProperties())
            {
                if (passou)
                {
                    if (Valores != "")
                    {
                        Valores += ",";
                    }
                    if (item.PropertyType.Name == "String" || item.PropertyType.Name == "Single")
                    {
                        Valores += item.Name + " = '" + (item.GetValue(t) ?? "").ToString() + "' ";
                    }
                    else
                    {
                        Valores += item.Name + " = " + (item.GetValue(t) ?? "").ToString();
                    }
                }
                else
                {
                    passou = true;
                    ID = " where ID = " + (item.GetValue(t) ?? "").ToString();
                }
            }

            var query = "Update " + Tabela + " SET " + Valores + ID;

            using (var sqlConnection = new MySqlConnection(ConnectionString))
            {
                var SaidaSQL = sqlConnection.Execute(query);
            }
        }
    }
}
