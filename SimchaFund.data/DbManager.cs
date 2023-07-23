using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.data
{
    public class DbManager
    {
        private string _connectionString;

        public DbManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributors";
            connection.Open();
            var reader = command.ExecuteReader();
            var contributors = new List<Contributor>();

            while (reader.Read())
            {
                contributors.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    PhoneNumber = (string)reader["CellPhoneNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                });
            }

            foreach(var contributor in contributors)
            {
                contributor.Balance = GetDepositsForTotal(contributor.Id);
                contributor.Balance += GetWithdrawlsForTotal(contributor.Id);
            }


            return contributors;
        }

        private int GetDepositsForTotal(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Amount FROM Deposits 
                            WHERE ContributorId = @Id";
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            int depsits = 0;

            while (reader.Read())
            {
                depsits += (int)reader["Amount"];
            }

            return depsits;
        }

        private int GetWithdrawlsForTotal(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Amount FROM Transactions 
                            WHERE ContributorId = @Id";
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            int withdrawls = 0;

            while (reader.Read())
            {
                withdrawls += (int)reader["Amount"];
            }

            return withdrawls;
        }

        public void AddContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Contributors (FirstName, LastName, CellPhoneNumber, AlwaysInclude)
                            VALUES (@firstName, @lastName, @phoneNumber, @alwaysInclude) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@firstName", c.FirstName);
            command.Parameters.AddWithValue("@lastName", c.LastName);
            command.Parameters.AddWithValue("@phoneNumber", c.PhoneNumber);
            command.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            connection.Open();

            c.Id = (int)(decimal)command.ExecuteScalar();
        }

        public void Deposit(int id, int amount, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Deposits (ContributorId, Amount, Date)
                            VALUES (@id, @amount, @date)";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@date", date);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<History> GetDepositsForHistory(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Deposits 
                            WHERE ContributorId = @Id";
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            var reader = command.ExecuteReader();

            List<History> historys = new List<History>();

            while (reader.Read())
            {
                historys.Add(new History
                {
                    Date = (DateTime)reader["Date"],
                    Amount = (int)reader["amount"]
                });
            }

            return historys;
        }

        public List<History> GetWithdrawlsForHistory(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT t.*, s.Name FROM Transactions t 
                            JOIN Simchas s
                            ON t.SimchaId = s.Id
                            WHERE ContributorId = @Id";
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            List<History> historys = new List<History>();

            while (reader.Read())
            {
                historys.Add(new History
                {
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    Amount = (int)reader["amount"]
                });
            }

            return historys;
        }

        public string GetName(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Contributors
                            WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return @$"{(string)reader["firstName"]} {(string)reader["lastName"]}";
        }

        public void Update(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Contributors SET FirstName = @firstName, LastName = @lastName, CellPhoneNumber = @cell, AlwaysInclude = @alwaysInclude
                            WHERE Id = @Id";
            command.Parameters.AddWithValue("@firstName", c.FirstName);
            command.Parameters.AddWithValue("@lastName", c.LastName);
            command.Parameters.AddWithValue("@cell", c.PhoneNumber);
            command.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            command.Parameters.AddWithValue("@Id", c.Id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void AddSimcha(SimchaForAdd s)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Simchas (Name, Date)
                            VALUES (@name, @date)";
            command.Parameters.AddWithValue("@name", s.Name);
            command.Parameters.AddWithValue("@date", s.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<SimchaForDisplay> GetSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Simchas";
            connection.Open();

            var simchas = new List<SimchaForDisplay>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                simchas.Add(new SimchaForDisplay
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["name"],
                    Date = (DateTime)reader["date"]
                });
            }
            return simchas;

        }

        public int GetTotalDonors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(FirstName) FROM Contributors";
            connection.Open();

            return (int)command.ExecuteScalar();
        }

        public int GetSimchaDonorCount(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(SimchaId) FROM Transactions
                            WHERE SimchaId = @Id";
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();

            return (int)command.ExecuteScalar();
        }

        public int GetSimchaTotal(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Transactions
                            WHERE SimchaId = @Id";
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            int total = 0;
             var reader = command.ExecuteReader();
            while (reader.Read())
            {
                total += (int)reader["amount"];
            }


            return total;
        }

        public int GetContributorsForSimcha(int simchaId, int cId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Contributors C
                            JOIN Transactions T
                            ON C.Id = T.ContributorId
                            WHERE T.SimchaId = @simchaId AND c.Id = @cId";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            command.Parameters.AddWithValue("@cId", cId);

            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return 0;
            }
            return (int)reader["amount"];
        }

        public void Donate(int simchaId, int contributorId, int amount)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Transactions (SimchaId, ContributorId, Amount, Date)
                            VALUES (@simId, @conId, @amount, @date)";
            command.Parameters.AddWithValue("@simId", simchaId);
            command.Parameters.AddWithValue("@conId", contributorId);
            command.Parameters.AddWithValue("@amount", amount * -1);
            command.Parameters.AddWithValue("@date", DateTime.Today);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void DonateMany(List<Donations> donations, int simchaId)
        {
            DeleteContributors(simchaId);

            foreach(Donations d in donations)
            {
                if (d.Include)
                {
                    Donate(simchaId, d.ContributorId, d.Amount);
                }
            }
        }

        public void DeleteContributors(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM Transactions 
                                WHERE SimchaId = @Id";
            command.Parameters.AddWithValue("@Id", simchaId);

            connection.Open();
            command.ExecuteNonQuery();

        }



    }
}
