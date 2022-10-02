using HW5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HW5.Controllers
{
    public class HomeController : Controller
    {
        // Service 
        private Service Service = new Service();
        public ActionResult Index()
        {
            
            BooksVM booksVM = new BooksVM();
            booksVM.Books = Service.GetAllBooks();
            booksVM.Authors = Service.GetAllAuthors();
            booksVM.Types = Service.GetAllTypes();
            return View(booksVM);
        }

        public ActionResult BookDetails(int id)
        {
            // Creating a book view model
            BookDetailsVM bookDetailsVM = new BookDetailsVM();  
            bookDetailsVM.BorrowedBooks = Service.GetAllBorrowedBooks(id);
            bookDetailsVM.Book = Service.GetAllBooks().Where(b => b.ID == id).FirstOrDefault();
            return View(bookDetailsVM);
        }
        public ActionResult Search( int type = 0, int author = 0, string name = null)
        {
            // 
            BooksVM booksVM = new BooksVM();
            booksVM.Books = Service.Search(name,type, author);
            booksVM.Authors = Service.GetAllAuthors();
            booksVM.Types = Service.GetAllTypes();               
            return View("Index", booksVM);
        }

        public ActionResult Students(int bookId)
        {
            // creating a list of students 
            StudentVM studentVM = new StudentVM();
            List<Student> students = Service.GetAllStudents();
            List<BorrowedBook> books = Service.GetAllBorrowedBooks(bookId);
            foreach(var student in students)
            {
                for (int i = 0; i < books.Count(); i++)
                {
                    string name = student.Name + " " + student.Surname;
                    if (books[i].StudentName == name && (books[i].BroughtDate == "" || books[i].BroughtDate == null))
                    {
                        student.Book = true;
                    }
                    else
                    {
                        student.Book = false;
                             
                    }
                }   
            }
            studentVM.Students = students;
            studentVM.Book = Service.GetAllBooks().Where(b => b.ID == bookId).FirstOrDefault();
            studentVM.Class = Service.GetAllClasses();
            return View(studentVM);
        }

     
        public ActionResult ReturnBook(int bookId, int studentId)
        {
            Service.ReturnBook(bookId, studentId);

            BookDetailsVM bookDetailsVM = new BookDetailsVM();
            bookDetailsVM.BorrowedBooks = Service.GetAllBorrowedBooks(bookId);
            bookDetailsVM.Book = Service.GetAllBooks().Where(b => b.ID == bookId).FirstOrDefault();
            return View("BookDetails", bookDetailsVM);
            
        }

        
        public ActionResult BorrowBook(int bookId, int studentId)
        {
            Service.BorrowBook(bookId, studentId);
            BookDetailsVM bookDetailsVM = new BookDetailsVM();
            bookDetailsVM.BorrowedBooks = Service.GetAllBorrowedBooks(bookId);
            bookDetailsVM.Book = Service.GetAllBooks().Where(b => b.ID == bookId).FirstOrDefault();
            return View("BookDetails", bookDetailsVM);
        }
        
        public ActionResult StudentSearch(int bookId, string name = "none", string _class = "none")
        {
            
            StudentVM studentVM = new StudentVM
            {
                Students = Service.SearchStudent(name, _class),
                Book = Service.GetAllBooks().Where(b => b.ID == bookId).FirstOrDefault(),
                Class = Service.GetAllClasses()

            };
            return View("Students", studentVM);
        }



    }
}