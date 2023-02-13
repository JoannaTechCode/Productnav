using Dapper;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DBContext
    {

        private readonly IConfiguration config;

        public DBContext(IConfiguration configuration)
        {
            config = configuration;
        }
        public Account CheckLogin(Account account)
        {
            try
            {
                using (var connection = new SqlConnection(config.GetConnectionString("Default")))
                {
                    string sql = "SELECT * FROM Users Where UserName = @Username";
                    Account user = connection.QuerySingle<Account>(sql, account);

                    if (user == null)
                    {
                        return null;
                    }

                    string passwordCheck = PasswordHasher.ConvertStringToHash(account.Password + user.Salt);
                    if (passwordCheck.Equals(user.Password))
                    {
                        return user;
                    }

                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CreateAccount(Account account)
        {
            try
            {
                using (var connection = new SqlConnection(config.GetConnectionString("Default")))
                {
                    string sql = "SELECT COUNT(*) FROM Accounts WHERE UserName = @UserName";
                    int count = connection.QuerySingle<int>(sql, account);

                    if (count > 0)
                    {
                        return false;
                    }

                    account.Salt = PasswordHasher.GenerateSalt();
                    account.Password = PasswordHasher.ConvertStringToHash(account.Password + account.Salt);

                    sql = "INSERT INTO Accounts (UserName,Password, Salt, Role) " +
                          "VALUES (@UserName, @Password, @Salt, @Role)";
                    connection.Execute(sql, account);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Product> GetProducts()
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = "SELECT * FROM Products";
                return connection.Query<Product>(sql).AsList();
            }
        }

        public List<Comment> GetCommentsForProduct(string productCode)
        {
            if (string.IsNullOrEmpty(productCode))
            { return null; }
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"SELECT * FROM Comments WHERE Comments.ProductCode like '{productCode}'";
                return connection.Query<Comment>(sql).AsList();
            }

        }

        public void AddComment(Comment comment, string sessionId)
        {
            comment.CreatedDate = DateTime.Now;
            comment.SessionId = sessionId;
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"INSERT INTO Comments (CommentText, ProductCode, SessionId, UserId, CreatedDate ) " +
                     "VALUES (@CommentText, @ProductCode, @sessionId, @UserId, @CreatedDate)";
                connection.Execute(sql, comment);
            }
        }

        public Comment GetSingleComment(int commentId)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"select * from Comments where CommentId={commentId}";
                return connection.QuerySingle<Comment>(sql);
            }
        }

        public void DeleteComment(int commentId)
        { using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"delete from Comments where CommentId={commentId}";
                connection.Execute(sql);
            
            }
        }

        public void EditComment(Comment comment)
        {
            
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = "update Comments set CommentText=@CommentText, "+
                             "ProductCode=@ProductCode, SessionId=@SessionId, UserId=@UserId, CreatedDate=@CreatedDate " +
                             "where CommentId=@CommentId ";
                connection.Execute(sql, comment);
            }


        }

        public void AddProduct(Product product)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"INSERT INTO Products (ProductCode, ProductName, ProductPrice, ProductDescription, UpdateDate ) " +
                     "VALUES (@ProductCode, @ProductName, @ProductPrice, @ProductDescription, @UpdateDate)";

                connection.Execute(sql, product);
            }
        }

        public Product GetAProduct(string productCode)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"select * from Products where Products.ProductCode like '{productCode}'";
                return connection.QuerySingle<Product>(sql);
            }

        }

        public void UpdatePrice(Product product)
        {
            product.UpdateDate = DateTime.Now;
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"update Products set ProductDescription=@ProductDescription, "+
                    "ProductName=@ProductName,ProductPrice=@ProductPrice,UpdateDate=@UpdateDate "+
                    "where ProductCode like @ProductCode";
                connection.Execute(sql, product);
            }

        }

        public List<Product> GetProductCodeCategories()
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = $"SELECT * FROM Products";
                return connection.Query<Product>(sql).AsList();
            }
        }
        public bool CheckProductCodeExist(string productCode)
        {
            
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                try
                {
                    string item;
                    string sql = $"select ProductCode from Products where Products.ProductCode like '{productCode}'";
                    item = connection.QuerySingle<string>(sql);
                 
                    if (String.IsNullOrEmpty(item))
                    { return true; }
                    return false;
                }
                catch (Exception)
                {
                    
                    return true;
                }
            }

        }

        public bool IsUserExist(string userName)
        {
            using (var connection = new SqlConnection(config.GetConnectionString("Default")))
            {
                string sql = "select * from Users";
                List<Account> users=connection.Query<Account>(sql).AsList();
                foreach (var user in users)
                {   if (user.Username == userName)
                    { return true; }
                }
                return false;

            }
        }

        public string GetPassoword(string userName)
        {
            if (IsUserExist(userName))
            {
                using (var connection = new SqlConnection(config.GetConnectionString("Default")))
                {
                    string sql = $"select Password from Users where UserName='{userName}'";
                    return connection.QuerySingle<string>(sql);
                }
            }
            return null;
        }
    }
}
