using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using BoardingHouseSys.Models;

namespace BoardingHouseSys.Data
{
    public class PaymentRepository
    {
        private DatabaseHelper _dbHelper;

        public PaymentRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public DataTable GetAllPayments()
        {
            string query = @"
                SELECT 
                    p.Id, 
                    b.FullName AS BoarderName, 
                    r.RoomNumber, 
                    p.Amount, 
                    p.MonthPaid, 
                    p.YearPaid, 
                    p.PaymentDate, 
                    p.Status, 
                    p.Notes,
                    p.BoarderId
                FROM Payments p
                JOIN Boarders b ON p.BoarderId = b.Id
                LEFT JOIN Rooms r ON b.RoomId = r.Id
                ORDER BY p.PaymentDate DESC";
            
            return _dbHelper.ExecuteQuery(query);
        }

        public DataTable SearchPayments(string keyword)
        {
            string query = @"
                SELECT 
                    p.Id, 
                    b.FullName AS BoarderName, 
                    r.RoomNumber, 
                    p.Amount, 
                    p.MonthPaid, 
                    p.YearPaid, 
                    p.PaymentDate, 
                    p.Status, 
                    p.Notes,
                    p.BoarderId
                FROM Payments p
                JOIN Boarders b ON p.BoarderId = b.Id
                LEFT JOIN Rooms r ON b.RoomId = r.Id
                WHERE (b.FullName LIKE @Keyword OR r.RoomNumber LIKE @Keyword OR p.Status LIKE @Keyword OR p.MonthPaid LIKE @Keyword OR CAST(p.Amount AS CHAR) LIKE @Keyword)
                ORDER BY p.PaymentDate DESC";

            MySqlParameter[] parameters = {
                new MySqlParameter("@Keyword", "%" + keyword + "%")
            };

            return _dbHelper.ExecuteQuery(query, parameters);
        }

        public DataTable GetPaymentsByBoarderId(int boarderId)
        {
            string query = @"
                SELECT 
                    p.Id, 
                    p.BoarderId,
                    p.Amount, 
                    p.MonthPaid, 
                    p.YearPaid, 
                    p.PaymentDate, 
                    p.Status, 
                    p.Notes 
                FROM Payments p
                WHERE p.BoarderId = @BoarderId
                ORDER BY p.PaymentDate DESC";
            
            MySqlParameter[] parameters = {
                new MySqlParameter("@BoarderId", boarderId)
            };

            return _dbHelper.ExecuteQuery(query, parameters);
        }

        public void AddPayment(Payment payment)
        {
            string query = @"
                INSERT INTO Payments (BoarderId, Amount, PaymentDate, MonthPaid, YearPaid, Status, Notes)
                VALUES (@BoarderId, @Amount, @PaymentDate, @MonthPaid, @YearPaid, @Status, @Notes)";

            MySqlParameter[] parameters = {
                new MySqlParameter("@BoarderId", payment.BoarderId),
                new MySqlParameter("@Amount", payment.Amount),
                new MySqlParameter("@PaymentDate", payment.PaymentDate),
                new MySqlParameter("@MonthPaid", payment.MonthPaid),
                new MySqlParameter("@YearPaid", payment.YearPaid),
                new MySqlParameter("@Status", payment.Status),
                new MySqlParameter("@Notes", payment.Notes ?? (object)DBNull.Value)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void UpdatePayment(Payment payment)
        {
            string query = @"
                UPDATE Payments 
                SET BoarderId = @BoarderId, 
                    Amount = @Amount, 
                    MonthPaid = @MonthPaid, 
                    YearPaid = @YearPaid, 
                    Status = @Status, 
                    Notes = @Notes 
                WHERE Id = @Id";

            MySqlParameter[] parameters = {
                new MySqlParameter("@Id", payment.Id),
                new MySqlParameter("@BoarderId", payment.BoarderId),
                new MySqlParameter("@Amount", payment.Amount),
                new MySqlParameter("@MonthPaid", payment.MonthPaid),
                new MySqlParameter("@YearPaid", payment.YearPaid),
                new MySqlParameter("@Status", payment.Status),
                new MySqlParameter("@Notes", payment.Notes ?? (object)DBNull.Value)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void DeletePayment(int id)
        {
            string query = "DELETE FROM Payments WHERE Id = @Id";
            MySqlParameter[] parameters = {
                new MySqlParameter("@Id", id)
            };
            _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
