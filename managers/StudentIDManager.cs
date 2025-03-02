using System;
using System.Collections.Generic;

/// <summary>
/// Overall, This Codebase Will Manage Students ID
/// It Uses Singleton Design Pattern
/// It Also Uses HashSet For Efficient ID Management
/// </summary>

namespace SMIS.managers
{
    public sealed class StudentIDManager
    {
        private static readonly Lazy<StudentIDManager> _instance = new(() => new StudentIDManager());
        public static StudentIDManager Instance => _instance.Value;
        private StudentIDManager() { }

        private readonly HashSet<int> ids = new();

        public bool TakenID(int id) => ids.Contains(id);
        public bool AddID(int id) => ids.Add(id);
        public bool RemoveID(int id) => ids.Remove(id);
        public void ClearAllID() => ids.Clear();

        public void InitializeIDs(IEnumerable<int> idsCollection)
        {
            ids.Clear();
            foreach (var id in idsCollection) 
                ids.Add(id);
        }
    }
}