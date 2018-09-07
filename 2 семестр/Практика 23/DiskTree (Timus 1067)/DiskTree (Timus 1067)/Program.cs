using System;
using System.Collections.Generic;
using System.Linq;

namespace DiskTree
{   
    class Directory
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public List<Directory> SubDirectory { get; set; }

        public Directory (string name, int priority)
        {
            Name = name;
            Priority = priority;
            SubDirectory = new List<Directory>();
        }

        public void AddSubDirectory (Directory subDir)
        {
            SubDirectory.Add(subDir);            
        }

        public Directory GetSubDirectory(string name)
        {
            return SubDirectory.Where(dir => dir.Name == name).FirstOrDefault();
        }

        public void PrintDirectoryName()
        {
            for (int i = 0; i < Priority; i++)
                Console.Write(' ');
            Console.WriteLine(Name);
        }

        public void PrintDirectory()
        {
            if (SubDirectory.Count == 0)
            {
                PrintDirectoryName();
            }
            else
            {
                PrintDirectoryName();
                SubDirectory = SubDirectory.OrderBy(dir => dir.Name, StringComparer.Ordinal).ToList();
                foreach (var dir in SubDirectory)
                    dir.PrintDirectory();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Directory)) return false;
            var dir = obj as Directory;
            return Name == dir.Name && Priority == dir.Priority;
        }
    }

    class Disk
    {
        public List<Directory> DirTree { get; set; }

        public Disk()
        {
            DirTree = new List<Directory>();
        }

        public void AddDirectory (Directory dir)
        {
            DirTree.Add(dir);
            DirTree.OrderBy(d => d.Name).ToList();
        }

        public Directory GetDirectory(string name)
        {
            return DirTree.Where(dir => dir.Name == name).FirstOrDefault();
        }

        public void Print()
        {
            DirTree = DirTree.OrderBy(dir => dir.Name, StringComparer.Ordinal).ToList();
            foreach (var dir in DirTree)
                dir.PrintDirectory();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int pathsCount = int.Parse(Console.ReadLine());
            var disk = new Disk();
            RecoverDisk(disk, pathsCount);
            disk.Print();
            Console.ReadKey();
        }

        static public void RecoverDisk(Disk disk, int pathsCount)
        {
            for (int i = 0; i < pathsCount; i++)
            {
                var path = Console.ReadLine().Split('\\');
                var mainDir = new Directory(path[0], 0);
                var currentDir = mainDir;
                if (!disk.DirTree.Contains(mainDir))
                    disk.DirTree.Add(mainDir);
                else
                    currentDir = disk.GetDirectory(path[0]);
                for (int j = 1; j < path.Count(); j++)
                {
                    var dir = new Directory(path[j], j);
                    if (!currentDir.SubDirectory.Contains(dir))
                        currentDir.AddSubDirectory(dir);
                    currentDir = currentDir.GetSubDirectory(path[j]);
                }
            }
        }
    }
}
