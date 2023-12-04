using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nile.Stores
{
   public class Sql: ProductDatabase
   {
        public Sql(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override Product AddCore(Product product)
        {

            using var conn = OpenConnection();
            var cmd = new SqlCommand("AddProduct", conn) { CommandType = CommandType.StoredProcedure };
            //var parmName= cmd.Parameters.Add("@name",SqlDbType.VarChar);
            // parmName.Value= movie.Title;
            cmd.Parameters.AddWithValue("@name", product.Name);
            cmd.Parameters.AddWithValue("@id", product.Id);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@isDiscontinued", product.IsDiscontinued);
           

            product.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return product;
        }

        protected override IEnumerable<Product> GetAllCore()
        {
            using var conn = OpenConnection();
            var ds = new DataSet();
            var da = new SqlDataAdapter()
            {
                SelectCommand = new SqlCommand("GetMovies", conn)
                {
                    CommandType = CommandType.StoredProcedure
                }
            };
            da.Fill(ds);

            //enumerate the dataset
            var products = new List<Product>();

            var table = ds.Tables.OfType<DataTable>().FirstOrDefault();
            if (table != null)
            {
                foreach (var row in table.Rows.OfType<DataRow>())
                {

                    products.Add(new Product()
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = Convert.ToString(row["Name"]),
                        Price = row.Field<Decimal>("Price"),
                        IsDiscontinued = row.Field<bool>("isDiscontinued"),
                        Description = Convert.ToString(row["Description"]),
                    });
                }
            }
            return products;
        }
         
        protected override Product GetCore(int id)
        {
            using var conn = OpenConnection();
            var cmd = new SqlCommand("GetProduct", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                return new Product()
                {

                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                   Price = reader.GetDecimal("Price"),
                    IsDiscontinued = reader.GetBoolean("IsDiscontinued"),
                    Description= reader.GetString("Description"),


                };
            }

            return null;
        }

        protected override void RemoveCore(int id)
        {
            using var conn = OpenConnection();
            var cmd = new SqlCommand("RemoveProduct", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        protected override Product UpdateCore(Product existing, Product newItem)
        {
            using var conn = OpenConnection();
            var cmd = new SqlCommand("UpdateProduct", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@id", existing);
            cmd.Parameters.AddWithValue("@name", newItem.Name);
            cmd.Parameters.AddWithValue("@description", newItem.Description);
            cmd.Parameters.AddWithValue("@price", newItem.Price);
            cmd.Parameters.AddWithValue("@isDiscontinued", newItem.IsDiscontinued);



            cmd.ExecuteNonQuery();

        }

        private SqlConnection OpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();

            return conn;
        }

        private readonly string _connectionString;
   }
}
