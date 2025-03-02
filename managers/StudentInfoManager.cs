using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SMIS.models;
using SMIS.utilities;

/// <summary>
/// Overall, This Code Base Will Manage Student Infomation
/// It Contains The List Of Student
/// It Provides Simple CRUD Operations Asychronously
/// It Also Uses Singleton Design Pattern
/// </summary>

namespace SMIS.managers
{
    public sealed class StudentInfoManager
    {
        private static readonly Lazy<StudentInfoManager> _instance = new(() => new StudentInfoManager());
        public static StudentInfoManager Instance => _instance.Value;
        private StudentInfoManager() { }

        private readonly StudentIDManager ManagerID = StudentIDManager.Instance;
        private List<Student> students = new();

        public async Task LoadDataAsync()
        {
            students = await FileManager.LoadStudentsAsync();
            ManagerID.InitializeIDs(students.Select(s => s.id));
        }

        public async Task<bool> AddStudentAsync(Student student)
        {
            if (student is null) return false;
            if (ManagerID.TakenID(student.id)) return false;

            if (!ManagerID.AddID(student.id)) return false;
            students.Add(student);
            await FileManager.SaveStudentsAsync(students).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            int index = students.FindIndex(s => s.id == student.id);
            if (index < 0) return false;

            students[index] = student;
            await FileManager.SaveStudentsAsync(students);
            return true;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = students.FirstOrDefault(s => s.id == id);
            if (student == null) return false;
            if (!ManagerID.RemoveID(id)) return false;

            students.Remove(student);
            
            await FileManager.SaveStudentsAsync(students).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> ClearAllStudentsAsync()
        {
            ManagerID.ClearAllID();
            students.Clear();

            await FileManager.SaveStudentsAsync(students).ConfigureAwait(false);
            return true;
        }

        public Task<List<Student>> GetAllStudentsAsync() => Task.FromResult(students);
        public Task<Student?> GetStudentByIDAsync(int id) => Task.FromResult(students.FirstOrDefault(s => s.id == id));
    }
}