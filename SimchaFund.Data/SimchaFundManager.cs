using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class SimchaFundManager
    {
        private string _connectionString;

        public SimchaFundManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddSimcha(Simcha simcha)
        {
            Db(command =>
            {
                command.CommandText = "INSERT INTO Simchas (Name, Date) VALUES (@name, @date)";
                command.Parameters.AddWithValue("@name", simcha.Name);
                command.Parameters.AddWithValue("@date", simcha.Date);
                command.ExecuteNonQuery();
            });
        }

        public IEnumerable<Simcha> GetSimchas()
        {
            return Db(command =>
            {
                command.CommandText = "SELECT * FROM Simchas";
                List<Simcha> simchas = new List<Simcha>();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    simchas.Add(new Simcha
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Date = (DateTime)reader["Date"]
                    });
                }

                return simchas;
            });
        }

        public int GetTotalContributors()
        {
            return Db(command =>
            {
                command.CommandText = "SELECT Count(*) FROM Contributors";
                return (int)command.ExecuteScalar();
            });
        }

        public decimal GetContributionTotal(int simchaId)
        {
            return Db(command =>
            {
                command.CommandText = "SELECT ISNULL(Sum(amount),0) FROM Contributions WHERE SimchaId = @simchaId";
                command.Parameters.AddWithValue("@simchaId", simchaId);
                return (decimal)command.ExecuteScalar();
            });
        }

        public int GetContributorCountForSimcha(int simchaId)
        {
            return Db(command =>
            {
                command.CommandText = "SELECT COUNT(*) FROM Contributions WHERE SimchaId = @simchaId";
                command.Parameters.AddWithValue("@simchaId", simchaId);
                return (int)command.ExecuteScalar();
            });
        }

        public decimal GetContributorBalance(int contributorId)
        {
            decimal depositSum = Db(command =>
            {
                command.CommandText = "SELECT ISNULL(Sum(Amount), 0) FROM Deposits WHERE ContributorId = @id";
                command.Parameters.AddWithValue("@id", contributorId);
                return (decimal)command.ExecuteScalar();
            });

            decimal contributionsSum = Db(command =>
            {
                command.CommandText = "SELECT ISNULL(Sum(Amount), 0) FROM Contributions WHERE ContributorId = @id";
                command.Parameters.AddWithValue("@id", contributorId);
                return (decimal)command.ExecuteScalar();
            });

            return depositSum - contributionsSum;
        }

        public IEnumerable<Contributor> GetContributors()
        {
            return Db(command =>
            {
                command.CommandText = "SELECT * FROM Contributors";
                List<Contributor> contributors = new List<Contributor>();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contributor contributor = new Contributor();
                    contributor.Id = (int)reader["Id"];
                    contributor.FirstName = (string)reader["FirstName"];
                    contributor.LastName = (string)reader["LastName"];
                    contributor.CellNumber = (string)reader["CellNumber"];
                    contributor.AlwaysInclude = (bool)reader["AlwaysInclude"];
                    contributor.Date = (DateTime)reader["Date"];
                    contributors.Add(contributor);
                }

                return contributors;
            });
        }

        public decimal GetTotalBalance()
        {
            decimal depositSum = Db(command =>
            {
                command.CommandText = "SELECT ISNULL(Sum(Amount), 0) FROM Deposits";
                return (decimal)command.ExecuteScalar();
            });

            decimal contributionsSum = Db(command =>
            {
                command.CommandText = "SELECT ISNULL(Sum(Amount), 0) FROM Contributions";
                return (decimal)command.ExecuteScalar();
            });

            return depositSum - contributionsSum;
        }

        public int AddContributor(Contributor contributor)
        {
            return Db(command =>
            {
                command.CommandText =
                    "INSERT INTO Contributors (FirstName, LastName, CellNumber, Date, AlwaysInclude) VALUES (@first, @last, @cell, @date, @alwaysInclude); SELECT @@Identity";
                command.Parameters.AddWithValue("@first", contributor.FirstName);
                command.Parameters.AddWithValue("@last", contributor.LastName);
                command.Parameters.AddWithValue("@cell", contributor.CellNumber);
                command.Parameters.AddWithValue("@date", contributor.Date);
                command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                return (int)(decimal)command.ExecuteScalar();
            });
        }

        public void AddDeposit(int contributorId, decimal amount, DateTime date)
        {
            Db(command =>
            {
                command.CommandText = "INSERT INTO Deposits (ContributorId, Amount, Date) VALUES (@id, @amount, @date)";
                command.Parameters.AddWithValue("@id", contributorId);
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@date", date);
                command.ExecuteNonQuery();
            });
        }

        public void UpdateContributor(Contributor contributor)
        {
            Db(command =>
            {
                command.CommandText =
                    "UPDATE Contributors SET FirstName = @firstName, LastName = @lastName, CellNumber = @cellNumber, AlwaysInclude = @alwaysInclude, Date = @date WHERE Id = @id";
                command.Parameters.AddWithValue("@firstName", contributor.FirstName);
                command.Parameters.AddWithValue("@lastName", contributor.LastName);
                command.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
                command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                command.Parameters.AddWithValue("@date", contributor.Date);
                command.Parameters.AddWithValue("@id", contributor.Id);
                command.ExecuteNonQuery();
            });
        }

        public IEnumerable<Deposit> GetDeposits(int contributorId)
        {
            return Db(command =>
            {
                command.CommandText = "SELECT * FROM Deposits WHERE ContributorId = @id";
                command.Parameters.AddWithValue("@id", contributorId);
                List<Deposit> deposits = new List<Deposit>();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Deposit deposit = new Deposit();
                    deposit.Id = (int)reader["Id"];
                    deposit.ContributorId = (int)reader["ContributorId"];
                    deposit.Date = (DateTime)reader["Date"];
                    deposit.Amount = (decimal)reader["Amount"];
                    deposits.Add(deposit);
                }

                return deposits;
            });
        }

        public IEnumerable<Contribution> GetContributionsForContributor(int contributorId)
        {
            return Db(command =>
            {
                command.CommandText = "SELECT c.*, s.Name, s.Date FROM Contributions c JOIN Simchas s on c.SimchaId = s.Id WHERE ContributorId = @id";
                command.Parameters.AddWithValue("@id", contributorId);
                List<Contribution> contributions = new List<Contribution>();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contribution contribution = new Contribution();
                    contribution.ContributorId = (int)reader["ContributorId"];
                    contribution.SimchaName = (string)reader["Name"];
                    contribution.Amount = (decimal)reader["Amount"];
                    contribution.SimchaDate = (DateTime)reader["Date"];
                    contributions.Add(contribution);
                }

                return contributions;
            });
        }

        public IEnumerable<Contribution> GetContributionsForSimcha(int simchaId)
        {
            return Db(command =>
            {
                command.CommandText = "SELECT c.*, s.Name, s.Date FROM Contributions c JOIN Simchas s on c.SimchaId = s.Id WHERE SimchaId = @id";
                command.Parameters.AddWithValue("@id", simchaId);
                List<Contribution> contributions = new List<Contribution>();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contribution contribution = new Contribution();
                    contribution.ContributorId = (int)reader["ContributorId"];
                    contribution.SimchaName = (string)reader["Name"];
                    contribution.Amount = (decimal)reader["Amount"];
                    contribution.SimchaDate = (DateTime)reader["Date"];
                    contributions.Add(contribution);
                }

                return contributions;
            });
        }

        public Contributor GetContributor(int contributorId)
        {
            return Db(command =>
            {
                command.CommandText = "SELECT * FROM Contributors WHERE id = @id";
                command.Parameters.AddWithValue("@id", contributorId);
                var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                Contributor contributor = new Contributor();
                contributor.Id = (int)reader["Id"];
                contributor.FirstName = (string)reader["FirstName"];
                contributor.LastName = (string)reader["LastName"];
                contributor.CellNumber = (string)reader["CellNumber"];
                contributor.AlwaysInclude = (bool)reader["AlwaysInclude"];
                contributor.Date = (DateTime)reader["Date"];

                return contributor;
            });
        }

        public Simcha GetSimcha(int simchaId)
        {
            return Db(command =>
            {
                command.CommandText = "SELECT * FROM Simchas WHERE Id = @id";
                command.Parameters.AddWithValue("@id", simchaId);
                var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                Simcha simcha = new Simcha();
                simcha.Date = (DateTime) reader["Date"];
                simcha.Id = (int) reader["Id"];
                simcha.Name = (string) reader["Name"];


                return simcha;
            });
        }

        public void AddContribution(int simchaId, int contributorId, decimal amount)
        {
            Db(command =>
            {
                command.CommandText =
                    "INSERT INTO Contributions (SimchaId, ContributorId, Amount) VALUES (@simchaId, @contribId, @amount)";
                command.Parameters.AddWithValue("@simchaId", simchaId);
                command.Parameters.AddWithValue("@contribId", contributorId);
                command.Parameters.AddWithValue("@amount", amount);
                command.ExecuteNonQuery();
            });
        }

        public void DeleteContribution(int simchaId, int contributorId)
        {
            Db(command =>
            {
                command.CommandText =
                    "DELETE FROM Contributions WHERE SimchaId = @simchaId AND ContributorId = @contribId";
                command.Parameters.AddWithValue("@simchaId", simchaId);
                command.Parameters.AddWithValue("@contribId", contributorId);
                command.ExecuteNonQuery();
            });
        }

        public void UpdateContribution(int simchaId, int contributorId, decimal amount)
        {
            Db(command =>
            {
                command.CommandText =
                    "UPDATE Contributions SET SimchaId = @simchaId, ContributorId = @contribId, Amount = @amount " +
                    "WHERE SimchaId = @simchaId AND ContributorId = @contribId";
                command.Parameters.AddWithValue("@simchaId", simchaId);
                command.Parameters.AddWithValue("@contribId", contributorId);
                command.Parameters.AddWithValue("@amount", amount);
                command.ExecuteNonQuery();
            });
        }

        private void Db(Action<SqlCommand> action)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                connection.Open();
                action(cmd);
            }
        }

        private TResult Db<TResult>(Func<SqlCommand, TResult> func)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                connection.Open();
                return func(cmd);
            }
        }
    }
}
