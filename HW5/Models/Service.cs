using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;

namespace HW5.Models
{
    public class Service
    {
        // Conncetion String 
        private String ConnectionString;

         
        // Constructor
        public Service()
        {


            //Retrieve connection string from the web config tab
            //add using system.configuration in order to prevent errors
            ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        }




        // creating a method to search for a student
        public List<Student> SearchStudent(string name, string _class)
        {
            List<Student> students = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                string query = "";
                
                

                // Searching for the class only 
               if(_class != "none" || _class != "" || _class != null )
                {
                    query = "Select * from students " +
                    "Where class Like '%"+_class+"%'";
                }

                // Searching for both class AND name 
                if(name != null && _class != null && name != "" && name != "none" && _class != "" && _class != "none")
                {
                    query = "Select * from students" + " Where class Like '%"+_class+"%' AND name Like '%"+name+"%'";
                }
                if (_class == "none")
                {
                    query = "Select * from students "
                    + "Where name Like '%" + name + "%'";
                }
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                       
                        while (reader.Read())
                        {
                            Student student = new Student
                            {
                                Id = Convert.ToInt32(reader["studentId"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Class = reader["class"].ToString(),
                                Points = Convert.ToInt32(reader["point"])

                            };
                            students.Add(student);
                        }
                        
                    }
                }
                con.Close();
            }

            return students;
        }




        // Method to search for a book
        public List<Book> Search(string name,int type, int author)
        {
            string innerJoin = "";
            


            // Search for the book name only 
            if (name != null && type == 0 && author == 0)
            {
                 innerJoin =
                 " select books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  from Books " +
                 " inner join authors on books.authorId = authors.authorId " +
                 " inner join types on books.typeId = types.typeId " +
                 " where books.name LIKE '%"+name+"%'";
            }


            // Searching for the book Type and book Author
            if (type > 0 && author > 0)
            {
                innerJoin =
                " select books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  from Books " +
                " inner join authors on books.authorId = authors.authorId " +
                " inner join types on books.typeId = types.typeId " +
                " where books.typeId = " + type +" AND books.authorId = " + author;
            }

            // Searching for the book Type and book Name 
            if (type > 0 && name != null)
            {
                innerJoin =
                " select books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  from Books " +
                " inner join authors on books.authorId = authors.authorId " +
                " inner join types on books.typeId = types.typeId " +
                " where books.typeId = " + type + " AND books.name LIKE '%" + name + "%'";
            }

            // Searching for the book Name, Type and author 
            if (type > 0 && author > 0 && name != null)
            {
                innerJoin =
                " select books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  from Books " +
                " inner join authors on books.authorId = authors.authorId " +
                " inner join types on books.typeId = types.typeId " +
                " where books.typeId = " + type + " AND books.name LIKE '%" + name + "%'" + " AND  books.authorId = " + author;
            }





            // creating the list of Books 
            List<Book> books = new List<Book>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
              
                using (SqlCommand cmd = new SqlCommand(innerJoin, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                Author = reader["Author"].ToString(),
                                PageCount = Convert.ToInt32(reader["PageCount"]),
                                Points = Convert.ToInt32(reader["Points"]),
                                Types = reader["Type"].ToString()
                            };
                            books.Add(book);
                        }
                    }
                }
                con.Close();
            }

            return books;
        }





        // creating a method to get all borrowed books 
        public List<BorrowedBook> GetAllBorrowedBooks(int id = 0)
        {
            // creating a list of borrowed books
            List<BorrowedBook> borrowedBooks = new List<BorrowedBook>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                // sql inner join command
                string innerJoin =
                    " Select CONCAT( students.name,' ',students.surname) as Student, takenDate, broughtDate, borrows.bookId ,  borrows.borrowId from students " +
                    " inner join borrows on students.studentId = borrows.studentId " +
                    " inner join books on books.bookId = borrows.bookId " +
                    "where borrows.bookId = " + id;
                if(id == 0)
                {
                    innerJoin =
                    " Select CONCAT( students.name,' ',students.surname) as Student, takenDate, broughtDate, borrows.bookId ,  borrows.borrowId from students " +
                    " inner join borrows on students.studentId = borrows.studentId " +
                    " inner join books on books.bookId = borrows.bookId ";
                }

                using (SqlCommand cmd = new SqlCommand(innerJoin, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BorrowedBook book = new BorrowedBook
                            {
                               BookID = Convert.ToInt32(reader["bookId"]),
                               BorrowID = Convert.ToInt32(reader["borrowId"]),
                               BroughtDate =reader["broughtDate"].ToString(),
                               TakenDate = reader["takenDate"].ToString(),
                               StudentName = reader["student"].ToString(),
                            };
                            borrowedBooks.Add(book);
                        }
                    }
                }
                con.Close();
            }

            
            return borrowedBooks;
        }

        



        public void BorrowBook(int bookId, int studentId)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string query = "insert into borrows( studentId, bookId, takenDate) " +
                    "values(@studentId,@bookId,@takenDate) ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    
                    cmd.Parameters.Add(new SqlParameter("@studentId", studentId));
                    cmd.Parameters.Add(new SqlParameter("@bookId", bookId));
                    cmd.Parameters.Add(new SqlParameter("@takenDate", DateTime.Now));
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

            GetAllStudents().Where(s => s.Id == studentId).FirstOrDefault().Book = true;
            
        }





        public void ReturnBook(int bookId, int studentId)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string query = "update borrows set broughtDate = @broughtDate where borrows.studentId = @studentId  AND borrows.bookId = @bookId and broughtDate IS NULL";
                    ;
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    cmd.Parameters.Add(new SqlParameter("@studentId", studentId));
                    cmd.Parameters.Add(new SqlParameter("@bookId", bookId));
                    cmd.Parameters.Add(new SqlParameter("@broughtDate", DateTime.Now));
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }

        }






        // Method to GET all the book authors 
        public List<Author> GetAllAuthors()
        {
            List<Author> authors = new List<Author>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();               
                using (SqlCommand cmd = new SqlCommand("select * from Authors", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Author author = new Author
                            {
                                Id = Convert.ToInt32(reader["authorId"]),
                                Name = reader["name"].ToString(),                          
                                Surname = reader["surname"].ToString()
                            };
                            authors.Add(author);
                        }
                    }
                }
                con.Close();
            }
            return authors;

        }





        //Method to GET all of the book types
        public List<Types> GetAllTypes()
        {
            List<Types> types = new List<Types>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("select * from types", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Types type = new Types
                            {
                                ID = Convert.ToInt32(reader["typeId"]),
                                Name = reader["name"].ToString(),
                              
                            };
                            types.Add(type);
                        }
                    }
                }
                con.Close();
            }
            return types;

        }



        // Method to GET all of the students 
        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("select * from students", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Student student = new Student
                            {
                                Id = Convert.ToInt32(reader["studentId"]),
                                Name = reader["name"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Class = reader["class"].ToString(),
                                Points = Convert.ToInt32(reader["point"])

                            };
                            students.Add(student);
                        }
                    }
                }
                con.Close();
            }
            return students;

        }




        // Method to GET all the classes of the students 
        public List<Class> GetAllClasses()
        {
            List<Class> classes = new List<Class>();
            foreach(Student student in GetAllStudents())
            {
                Class cl = new Class
                {
                    Name = student.Class
                };
                if(classes.Where(n => n.Name == student.Class).Count() == 0)
                {
                    classes.Add(cl);
                }                
            }
            return classes;
        }




        // Method to GET all books 
        public List<Book> GetAllBooks()
        {
             // creating a list of Books 
             List<Book> books = new List<Book>();   
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {               
                con.Open();
                // Creating sql inner join command
                string innerJoin =
                    " select books.bookId as ID, books.pagecount as PageCount, books.point as Points, books.name as Name, types.name as Type, authors.surname as Author  from Books " +
                    " inner join authors on books.authorId = authors.authorId " +
                    " inner join types on books.typeId = types.typeId ";

                using (SqlCommand cmd = new SqlCommand(innerJoin, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                Author = reader["Author"].ToString(),
                                PageCount = Convert.ToInt32(reader["PageCount"]),
                                Points = Convert.ToInt32(reader["Points"]),
                                Types = reader["Type"].ToString(),
                            };
                            books.Add(book);
                        }
                    }
                }
                con.Close();
            }




            // status of the bbok
            foreach (var book in books)
            {
                // Get all borrowed books
                List<BorrowedBook> borrowedBooks = GetAllBorrowedBooks(book.ID);
                // check if the book is borrowed or available
                if (borrowedBooks.Where(b => b.BroughtDate == "").Count() == 1)
                {
                    book.Status = "Book Out";
                }
                else
                {
                    book.Status = "Available";
                }
            }
            return books;
        }
    }
}