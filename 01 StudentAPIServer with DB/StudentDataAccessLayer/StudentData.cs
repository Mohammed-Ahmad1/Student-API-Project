using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace StudentDataAccessLayer
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }

        public StudentDTO(int id, string name, int age, int grade)
        {
            this.Id = id;
            this.Name = name;
            this.Age = age;
            this.Grade = grade;
        }
    }

    public class StudentData
    {
        static string _connectionString = "Server=localhost;Database=StudentsDB;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        public static List<StudentDTO> GetAllStudents()
        {
            var StudentsList = new List<StudentDTO>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SP_GetAllStudents", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentsList.Add(new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))
                            ));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log or handle DB-specific errors
                throw new Exception("Error retrieving all students.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error retrieving all students.", ex);
            }

            return StudentsList;
        }

        public static List<StudentDTO> GetPassedStudents()
        {
            var StudentsList = new List<StudentDTO>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SP_GetPassedStudents", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentsList.Add(new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))
                            ));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error retrieving passed students.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error retrieving passed students.", ex);
            }

            return StudentsList;
        }

        public static double GetAverageGrade()
        {
            double averageGrade = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SP_GetAverageGrade", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    averageGrade = (result != DBNull.Value) ? Convert.ToDouble(result) : 0;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error calculating average grade.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error calculating average grade.", ex);
            }

            return averageGrade;
        }

        public static StudentDTO GetStudentById(int studentId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("SP_GetStudentById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentId", studentId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))
                            );
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error retrieving student with ID {studentId}.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error retrieving student.", ex);
            }
        }

        public static int AddStudent(StudentDTO StudentDTO)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("SP_AddStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", StudentDTO.Name);
                    command.Parameters.AddWithValue("@Age", StudentDTO.Age);
                    command.Parameters.AddWithValue("@Grade", StudentDTO.Grade);

                    var outputIdParam = new SqlParameter("@NewStudentId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    command.Parameters.Add(outputIdParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputIdParam.Value;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error adding new student.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error adding new student.", ex);
            }
        }

        public static bool UpdateStudent(StudentDTO StudentDTO)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("SP_UpdateStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@StudentId", StudentDTO.Id);
                    command.Parameters.AddWithValue("@Name", StudentDTO.Name);
                    command.Parameters.AddWithValue("@Age", StudentDTO.Age);
                    command.Parameters.AddWithValue("@Grade", StudentDTO.Grade);

                    connection.Open();
                    int rowsAffected = (int)command.ExecuteScalar();
                    return (rowsAffected == 1);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error updating student.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error updating student.", ex);
            }
        }

        public static bool DeleteStudent(int studentId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("SP_DeleteStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentId", studentId);

                    connection.Open();
                    int rowsAffected = (int)command.ExecuteScalar();
                    return (rowsAffected == 1);
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error deleting student with ID {studentId}.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error deleting student.", ex);
            }
        }
    }
}
