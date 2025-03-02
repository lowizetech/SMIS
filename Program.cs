using System;
using System.Threading.Tasks;

using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using SMIS.models;
using SMIS.managers;
using SMIS.utilities;

/// <remarks>
/// ---- whoami ----
/// Developer/Programmer: Louise Czedrian B. Laping
/// GitHub: https://github.com/lowizetech
/// Email: lowize.tech@gmail.com
/// ---------------
/// </remarks>

/// <summary>
/// STUDENT MANAGEMENT INFORMATION SYSTEM
/// Console Based CRUD Application
/// Developed: February 23, 2025 - March 2, 2025

/// It Uses JSON As A Database
/// It Read/Write/Update/Remove/Clear (CRUD) Asynchronously
/// It Has A Feature That Will Generate Printable PDF Containing The List Of Students (database.json)

/// THIS IS THE MAIN ENTRY POINT OF THE SYSTEM.
/// THIS CODE BASE WILL HANDLE MOST OF THE SYSTEM LOGIC
/// </summary>

namespace SMIS
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Settings.License = LicenseType.Community;

            StaticUtil.Message("student management information system", true, ConsoleColor.Green);
            StaticUtil.Message("====================================================", true, ConsoleColor.Green);

            await StudentInfoManager.Instance.LoadDataAsync();

            while (true)
            {
                Console.WriteLine("1. View Students");
                Console.WriteLine("2. Add Student");
                Console.WriteLine("3. Update Student");
                Console.WriteLine("4. Delete Student");
                Console.WriteLine("5. Clear Student List");
                Console.WriteLine("6. Generate PDF");
                Console.WriteLine("7. Exit");

                Console.Write("\nEnter [#] >> ");
                if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice))
                {
                    Console.Write("\nInvalid Input. Please Enter A Valid Choice.\n\n");
                    continue;
                }

                switch (choice) 
                {
                    case 1:
                        await ViewStudents();
                        break;
                    case 2:
                        await AddStudent();
                        break;
                    case 3:
                        await UpdateStudent();
                        break;
                    case 4:
                        await DeleteStudent();
                        break;
                    case 5:
                        await ClearAllStudents();
                        break;
                    case 6:
                        await GetDocument();
                        break;
                    case 7:
                        Console.WriteLine("\nStartnig Exiting System...");
                        StaticUtil.Message("successfully exited system", true, ConsoleColor.Green);
                        return;
                    default:
                        Console.WriteLine("\nInvalid Choice. Try again.\n");
                        break;
                }
            }
        }

// ---------------------------------------------------------------------------------------------

        private static async Task GetDocument()
        {
            var students = await StudentInfoManager.Instance.GetAllStudentsAsync();

            StaticUtil.Message("generating printable pdf", true, ConsoleColor.Green);
            Console.WriteLine("\nGenerate PDF With The Current Student List? Enter To Cancel\n");

            while (true)
            {
                Console.Write("(Y\\N): ");
                string? input = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(input) || input?.Equals("N", StringComparison.OrdinalIgnoreCase) == true)
                {
                    StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                    return;
                }
                else if (!input?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true)
                {
                    Console.WriteLine("\nInvalid Input. Please Enter A Valid Choice (Y\\N)\n");
                    continue;
                }
                break;
            }

            if (students.Count == 0)
            {
                StaticUtil.Message("no students found in database. enter to cancel");
                while (true)
                {
                    Console.WriteLine("\nThere Are No Students In The List. Would You Generate Anyway?\n");
                    Console.Write("(Y\\N): ");
                    string? input = Console.ReadLine()?.Trim();

                    if (string.IsNullOrWhiteSpace(input) || input?.Equals("N", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                        return;
                    }
                    else if (!input?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        Console.WriteLine("\nInvalid Input. Please Enter A Valid Choice (Y\\N)\n");
                        continue;
                    }
                    break;
                }
            }

            Console.WriteLine("\nGenerating PDF...");
            
            var doc = new GeneratePDF(students);
            
            Document.Create(container =>
                doc.Compose(container)).GeneratePdf("STUDENT LIST (READY TO PRINT).pdf");

            Console.WriteLine("\nGenerated Student List PDF at (/SMIS/database/STUDENT LIST (READY TO PRINT).pdf)\n");
            StaticUtil.Message("successfully generated printable pdf", true, ConsoleColor.Green);
        }

// ---------------------------------------------------------------------------------------------

        private static async Task ViewStudents()
        {
            var students = await StudentInfoManager.Instance.GetAllStudentsAsync();
            if (students.Count == 0)
            {
                Console.WriteLine("\nNo Students Found In The Database.\n");
                return;
            }

            StaticUtil.Message("student list", true, ConsoleColor.Green);
            StaticUtil.Message("====================================================", true, ConsoleColor.Green);

            Console.WriteLine("--------------------------------------------------------\n");
            foreach (var student in students) 
            {
                Console.WriteLine($"# ID: {student.id}\nName: {student.name}\nGender: {student.sex}\nAge: {student.age}\nAddress: {student.address}\nStrand: {student.strand}\n");
                Console.WriteLine("--------------------------------------------------------\n");
            }
            StaticUtil.Message("====================================================\n", true, ConsoleColor.Green);
        }

// ---------------------------------------------------------------------------------------------

        private static async Task AddStudent()
        {
            StaticUtil.Message("fill up student details. press enter to cancel", true, ConsoleColor.Yellow);
            string? _name, _address;
            int _id = default, _age = default;

            // GET ID
            while (true)
            {
                Console.Write("\nID: ");
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                    return;
                }

                if (!int.TryParse(input, out _id))
                {
                    Console.WriteLine("\nInvalid Input. Please Enter A Valid Numeric ID.");
                    continue;
                }
                else if (_id < 1_000)
                {
                    Console.WriteLine("\nInvalid Input. Please Enter ID Up To 1,000.");
                    continue;
                }
                else if (StudentIDManager.Instance.TakenID(_id))
                {
                    Console.WriteLine("\nInvalid Input. This ID Is Already Taken.");
                    continue;
                }
                break;
            }

            // GET NAME
            while (true)
            {
                Console.Write("\nName: ");
                string? input = Console.ReadLine()?.Trim().ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(input))
                {
                    StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                    return;
                }
                _name = input;
                break;
            }

            // GET SEX
            Sex _sex = TInput.GetEnumInput<Sex>("Sex (M/F)", new() {{"M", Sex.MALE.ToString()},{"F", Sex.FEMALE.ToString()}});
            
            // GET AGE
            while (true)
            {
                Console.Write("\nAge: ");
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                    return;
                }
                if (!int.TryParse(input, out _age))
                {
                    Console.WriteLine("\nInvalid Input. Please Enter A Valid Numeric Age.");
                    continue;
                }
                break;
            }

            // GET ADDRESS
            while (true)
            {
                Console.Write("\nAddress: ");
                string? input = Console.ReadLine()?.Trim().ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(input))
                {
                    StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                    return;
                }
                _address = input;
                break;
            }

            // GET STRAND
            Strand _strand = TInput.GetEnumInput<Strand>("Strand");

            Student _student = new Student
            {
                id = _id,
                name = _name,
                sex = _sex,
                age = _age,
                address = _address,
                strand = _strand,
            };

            bool added = await StudentInfoManager.Instance.AddStudentAsync(_student);

            if (added == true)
                StaticUtil.Message("student added successfully", false, ConsoleColor.Green);
            else
                StaticUtil.Message("failed to add student. ID might be already taken", false, ConsoleColor.Red);
        }

// ---------------------------------------------------------------------------------------------

        private static async Task UpdateStudent()
        {
            int _origID, _id, _age;
            string? _name, _address;

            StaticUtil.Message("fill up student details. enter to cancel", true, ConsoleColor.Yellow);

            while (true)
            {
                Console.Write("\nEnter ID of student to update: ");
                string? input = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                    return;
                }
                if (int.TryParse(input, out _origID))
                    break;

                Console.WriteLine("Invalid Input. Please Enter A Valid Numeric ID.");
            }
            
            var student = await StudentInfoManager.Instance.GetStudentByIDAsync(_origID);
            if (student == null)
            {
                Console.WriteLine("\nStudent Not Found.\n");
                return;
            }

            StaticUtil.Message("fill in new student details. enter to keep current value", true, ConsoleColor.Yellow);

            while (true)
            {
                Console.Write($"\nID ({student.id}): ");
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                    break;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (int.TryParse(input, out _id) && _id != student.id)
                    {
                        if (StudentIDManager.Instance.TakenID(_id))
                        {
                            Console.WriteLine("\nInvalid Input. This ID Is Already Taken.\n");
                            continue;
                        }
                        else
                        {
                            StudentIDManager.Instance.RemoveID(student.id);
                            StudentIDManager.Instance.AddID(_id);
                            student.id = _id;    
                        }
                    }
                }
                break;
            }

            Console.Write($"\nName ({student.name}): ");
            _name = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(_name))
                student.name = _name;

            student.sex = TInput.GetEnumInput<Sex>(
                "Sex (M/F)",
                new() {{"M", Sex.MALE.ToString()},{"F", Sex.FEMALE.ToString()}},
                true,
                currentValue: student.sex);

            while (true)
            {
                Console.Write($"\nAge ({student.age}): ");
                string? input = Console.ReadLine()?.Trim();    

                if (string.IsNullOrWhiteSpace(input))
                    break;
                else if (!int.TryParse(input, out _age))
                {
                    Console.WriteLine("\nInvalid Input. Please Enter A Valid Numeric ID.");
                    continue;
                }
                student.age = _age;
                break;
            }

            Console.Write($"\nAddress ({student.address}): ");
            _address = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(_address))
                student.address = _address;

            student.strand = TInput.GetEnumInput<Strand>(
                "Strand",
                null,
                true,
                currentValue: student.strand);

            bool updated = await StudentInfoManager.Instance.UpdateStudentAsync(student);

            if (updated == true)
                StaticUtil.Message("student updated successfully", false, ConsoleColor.Green);
            else
                StaticUtil.Message("failed to update student", false, ConsoleColor.Red);
        }

// ---------------------------------------------------------------------------------------------

        private static async Task DeleteStudent()
        {
            StaticUtil.Message("fill up student details. enter to cancel", true, ConsoleColor.Yellow);
            
            int _id;
            while (true)
            {
                Console.Write("\nEnter ID of student to delete: ");
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                    return;
                }

                if (!int.TryParse(input, out _id))
                {
                    Console.WriteLine("\nInvalid Input. Please Enter A Valid Numeric ID.");
                    continue;
                }

                var student = await StudentInfoManager.Instance.GetStudentByIDAsync(_id);
                if (student == null)
                {
                    Console.WriteLine("\nStudent Not Found.\n");
                    return;
                }

                bool deleted = await StudentInfoManager.Instance.DeleteStudentAsync(_id);

                if (deleted == true)
                    StaticUtil.Message("student removed successfully", false, ConsoleColor.Green);
                else
                    StaticUtil.Message("failed to remove student.", false, ConsoleColor.Red);

                break;
            }
        }

// ---------------------------------------------------------------------------------------------    

        private static async Task ClearAllStudents()
        {
            StaticUtil.Message("clearing all students in database", true, ConsoleColor.Yellow);
            
            while (true)
            {
                Console.WriteLine("\nClear All Students In The List And Database? Enter To Cancel\n");
                Console.Write("(Y\\N): ");

                string? input = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(input) || input?.Equals("N", StringComparison.OrdinalIgnoreCase) == true)
                {
                    StaticUtil.Message("operation canceled", true, ConsoleColor.Yellow);
                    return;
                }
                if (input?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true)
                {
                    Console.WriteLine("\nClearing Student Database...\n");
                    bool cleared = await StudentInfoManager.Instance.ClearAllStudentsAsync();
                    if (cleared)
                        StaticUtil.Message("student data cleared successfully", false, ConsoleColor.Green);
                    else
                        StaticUtil.Message("Failed to clear student data", false, ConsoleColor.Red);
                    break;
                }
                else
                {
                    Console.WriteLine("\nInvalid Input. Please Enter A Valid Choice (Y\\N)\n");
                    continue;
                }
            }
        }
    }
}