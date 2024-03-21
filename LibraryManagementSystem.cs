using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class User
{
    public string CardNumber { get; set; }
    public string Password { get; set; }
    public List<string> BorrowedBooks { get; set; }
}

class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int CopiesAvailable { get; set; }
}

class Program
{
    private static List<User> users = new List<User>();
    private static List<Book> books = new List<Book>();
    private static string usersFilePath = "users.txt";
    private static string booksFilePath = "books.txt";
    private static string adminPassword = "admin"; // Secret admin password

    static void Main(string[] args)
    {
        LoadTestData(); // Load data from text files on startup

        while (true)
        {
            Console.WriteLine("Library Management System");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Search Books");
            Console.WriteLine("3. Admin Mode");
            Console.WriteLine("4. Quit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Login();
                    break;
                case "2":
                    SearchBooks();
                    break;

                case "3":
                    AdminMode();
                    break;
                case "4":
                    SaveData();
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void Login()
    {
        Console.Write("Enter your library card number: ");
        string libraryCardNumber = Console.ReadLine();
        Console.Write("Enter your password: ");
        string password = Console.ReadLine();

        User user = users.FirstOrDefault(u => u.CardNumber == libraryCardNumber && u.Password == password);

        if (user != null)
        {
            Console.WriteLine("Login successful.");
            UserMenu(user);
        }
        else
        {
            Console.WriteLine("Invalid library card number or password. Please try again.");
        }
    }

    static void UserMenu(User user)
    {
        while (true)
        {
            Console.WriteLine("User Menu");
            Console.WriteLine("1. Search Books");
            Console.WriteLine("2. Borrow Book");
            Console.WriteLine("3. Return Book");
            Console.WriteLine("4. Logout");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearchBooks();
                    break;
                case "2":
                    BorrowBook(user);
                    break;
                case "3":
                    ReturnBook(user);
                    break;
                case "4":
                    SaveData();
                    Console.WriteLine("Logging out...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void SearchBooks()
    {
        Console.WriteLine("Search Books");
        Console.Write("Enter search query (title, author, genre): ");
        string query = Console.ReadLine().ToLower();

        var results = books.Where(b =>
            b.Title.ToLower().Contains(query) ||
            b.Author.ToLower().Contains(query) ||
            b.Genre.ToLower().Contains(query)
        ).ToList();

        if (results.Count == 0)
        {
            Console.WriteLine("No matching books found.");
        }
        else
        {
            Console.WriteLine("Matching Books:");
            foreach (var book in results)
            {
                Console.WriteLine($"{book.Title} by {book.Author} ({book.Genre}) - Copies available: {book.CopiesAvailable}");
            }
        }
    }

    static void BorrowBook(User user)
    {
        Console.WriteLine("Borrow Book");
        Console.Write("Enter the title of the book you want to borrow: ");
        string title = Console.ReadLine();

        Book book = books.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (book != null && book.CopiesAvailable > 0)
        {
            user.BorrowedBooks.Add(book.Title);
            book.CopiesAvailable--;
            Console.WriteLine($"You have successfully borrowed '{book.Title}'.");
        }
        else
        {
            Console.WriteLine("Book not found or no copies available.");
        }
    }

    static void ReturnBook(User user)
    {
        Console.WriteLine("Return Book");
        Console.Write("Enter the title of the book you want to return: ");
        string title = Console.ReadLine();

        Book book = books.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (book != null && user.BorrowedBooks.Contains(book.Title))
        {
            user.BorrowedBooks.Remove(book.Title);
            book.CopiesAvailable++;
            Console.WriteLine($"You have successfully returned '{book.Title}'.");
        }
        else
        {
            Console.WriteLine("Book not found in your borrowed list.");
        }
    }

    static void AdminMode()
    {
        Console.Write("Enter admin password: ");
        string adminPasswordInput = Console.ReadLine();

        if (adminPasswordInput == adminPassword)
        {
            while (true)
            {
                Console.WriteLine("Admin Menu");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. Remove Book");
                Console.WriteLine("3. Add User");
                Console.WriteLine("4. View Book Inventory");
                Console.WriteLine("5. View User Borrowings");
                Console.WriteLine("6. Exit Admin Mode");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        RemoveBook();
                        break;
                    case "3":
                        AddUser();
                        break;
                    case "4":
                        ViewBookInventory();
                        break;
                    case "5":
                        ViewUserBorrowings();
                        break;
                    case "6":
                        SaveData();
                        Console.WriteLine("Exiting Admin Mode...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        else
        {
            Console.WriteLine("Incorrect admin password.");
        }
    }

    static void AddBook()
    {
        Console.WriteLine("Add Book");
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();
        Console.Write("Enter author: ");
        string author = Console.ReadLine();
        Console.Write("Enter genre: ");
        string genre = Console.ReadLine();
        Console.Write("Enter the number of copies available: ");
        int copiesAvailable = int.Parse(Console.ReadLine());

        Book newBook = new Book
        {
            Title = title,
            Author = author,
            Genre = genre,
            CopiesAvailable = copiesAvailable
        };

        books.Add(newBook);
        Console.WriteLine($"Book '{newBook.Title}' added successfully.");
    }

    static void RemoveBook()
    {
        Console.WriteLine("Remove Book");
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();

        Book bookToRemove = books.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (bookToRemove != null)
        {
            books.Remove(bookToRemove);
            Console.WriteLine($"Book '{bookToRemove.Title}' removed successfully.");
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
    }

    static void AddUser()
    {
        Console.WriteLine("Add User");
        Console.Write("Enter library card number: ");
        string cardNumber = Console.ReadLine();
        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        User newUser = new User
        {
            CardNumber = cardNumber,
            Password = password,
            BorrowedBooks = new List<string>()
        };

        users.Add(newUser);
        Console.WriteLine($"User with card number '{newUser.CardNumber}' added successfully.");
    }

    static void ViewBookInventory()
    {
        Console.WriteLine("Book Inventory");
        Console.WriteLine("Title | Author | Genre | Copies Available");
        foreach (var book in books)
        {
            Console.WriteLine($"{book.Title} | {book.Author} | {book.Genre} | {book.CopiesAvailable}");
        }
    }

    static void ViewUserBorrowings()
    {
        Console.WriteLine("User Borrowings");
        Console.WriteLine("Library Card Number | Borrowed Books Count");
        foreach (var user in users)
        {
            Console.WriteLine($"{user.CardNumber} | {user.BorrowedBooks.Count}");
        }
    }

    static void LoadTestData()
    {
        if (File.Exists(usersFilePath) && File.Exists(booksFilePath))
        {
            users = LoadDataFromFile<User>(usersFilePath);
            books = LoadDataFromFile<Book>(booksFilePath);
        }
        else
        {
            InitializeTestData();
        }
    }

    static void InitializeTestData()
    {
        users = new List<User>
        {
            new User { CardNumber = "984011", Password = "password1", BorrowedBooks = new List<string> { "The Old Man and the Sea", "Animal Farm", "Of Mice and Men" } },
            new User { CardNumber = "984022", Password = "password2", BorrowedBooks = new List<string> { "The Road", "The Outsiders" } },
            new User { CardNumber = "984033", Password = "password3", BorrowedBooks = new List<string> { "Lord of the Flies" } },
            new User { CardNumber = "984044", Password = "password4", BorrowedBooks = new List<string>() },
            new User { CardNumber = "984055", Password = "password5", BorrowedBooks = new List<string>() }
        };

        books = new List<Book>
        {
            new Book { Title = "The Old Man and the Sea", Author = "Ernest Hemingway", Genre = "Adventure", CopiesAvailable = 5 },
            new Book { Title = "Animal Farm", Author = "George Orwell", Genre = "Fiction", CopiesAvailable = 3 },
            new Book { Title = "Of Mice and Men", Author = "John Steinbeck", Genre = "Fiction", CopiesAvailable = 2 },
            new Book { Title = "The Road", Author = "Cormac McCarthy", Genre = "Fiction", CopiesAvailable = 1 },
            new Book { Title = "The Outsiders", Author = "S.E. Hinton", Genre = "Adult", CopiesAvailable = 4 },
            new Book { Title = "Lord of the Flies", Author = "William Golding", Genre = "Fiction", CopiesAvailable = 0 },
            new Book { Title = "The Giver", Author = "Lois Lowry", Genre = "Adult", CopiesAvailable = 0 },
            new Book { Title = "The Call of the Wild", Author = "Jack London", Genre = "Adventure", CopiesAvailable = 2 },
            new Book { Title = "The Pearl", Author = "John Steinbeck", Genre = "Novel", CopiesAvailable = 3 },
            new Book { Title = "Rich Dad Poor Dad ", Author = "Robert T. Kiyosaki", Genre = "Self Help", CopiesAvailable = 1 },
        };
    }

    static void SaveData()
    {
        SaveDataToFile(users, usersFilePath);
        SaveDataToFile(books, booksFilePath);
    }

    static List<T> LoadDataFromFile<T>(string filePath)
    {
        List<T> data = new List<T>();
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                T obj = ParseData<T>(line);
                if (obj != null)
                {
                    data.Add(obj);
                }
            }
        }
        return data;
    }

    static void SaveDataToFile<T>(List<T> data, string filePath)
    {
        List<string> lines = new List<string>();
        foreach (T obj in data)
        {
            string line = SerializeData(obj);
            lines.Add(line);
        }
        File.WriteAllLines(filePath, lines);
    }

    static T ParseData<T>(string line)
    {
        try
        {
            if (typeof(T) == typeof(User))
            {
                string[] parts = line.Split('|');
                if (parts.Length == 3)
                {
                    return (T)(object)new User
                    {
                        CardNumber = parts[0],
                        Password = parts[1],
                        BorrowedBooks = parts[2].Split(',').ToList()
                    };
                }
            }
            else if (typeof(T) == typeof(Book))
            {
                string[] parts = line.Split('|');
                if (parts.Length == 4)
                {
                    return (T)(object)new Book
                    {
                        Title = parts[0],
                        Author = parts[1],
                        Genre = parts[2],
                        CopiesAvailable = int.Parse(parts[3])
                    };
                }
            }
            return default(T);
        }
        catch (Exception)
        {
            return default(T);
        }
    }

    static string SerializeData<T>(T obj)
    {
        if (typeof(T) == typeof(User))
        {
            User user = obj as User;
            return $"{user.CardNumber}|{user.Password}|{string.Join(",", user.BorrowedBooks)}";
        }
        else if (typeof(T) == typeof(Book))
        {
            Book book = obj as Book;
            return $"{book.Title}|{book.Author}|{book.Genre}|{book.CopiesAvailable}";
        }
        return string.Empty;
    }
}
