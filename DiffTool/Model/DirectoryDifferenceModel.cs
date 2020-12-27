using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class DirectoryDifferenceModel
    {
        public string Path1 { get; set; }
        public string Path2 { get; set; }

        private DirectoryInfo dir1;
        private DirectoryInfo dir2;

        public readonly bool IsEmpty;
        public List<FileDifferenceModel> ChangedFiles { get; private set; }
        public List<DirectoryDifferenceModel> ChangedDirectories { get; private set; }
        public IEnumerable<string> AddedDirectories { get; private set; }
        public IEnumerable<string> DeletedDirectories { get; private set; }
        public IEnumerable<string> AddedFiles { get; private set; }
        public IEnumerable<string> DeletedFiles { get; private set; }
        public List<string> UnchangedFiles{ get; private set; }
        public List<string> UnchangedDirectories { get; private set; }
        public string Name => dir2.Name;

        public void CalculateDifference()
        {
            var dirNames1 = new List<string>(dir1.EnumerateDirectories().Select(di => di.Name));
            var dirNames2 = new List<string>(dir2.EnumerateDirectories().Select(di => di.Name));
            var fileNames1 = new List<string>(dir1.EnumerateFiles().Select(fi => fi.Name));
            var fileNames2 = new List<string>(dir2.EnumerateFiles().Select(fi => fi.Name));

            UnchangedFiles = new List<string>();
            UnchangedDirectories = new List<string>();

            AddedDirectories = dirNames2.Except(dirNames1);
            DeletedDirectories = dirNames1.Except(dirNames2);

            AddedFiles = fileNames2.Except(fileNames1);
            DeletedFiles = fileNames1.Except(fileNames2);
            
            var sameFiles = fileNames2.Intersect(fileNames1);
            ChangedFiles = new List<FileDifferenceModel>();

            foreach (var name in sameFiles)
            {
                var diff = new FileDifferenceModel(Path.Join(Path1, name),
                                                   Path.Join(Path2, name));
                if (diff.IsEmpty)
                    UnchangedFiles.Add(name);
                else
                    ChangedFiles.Add(diff);
            }


            var sameDirectories = dirNames2.Intersect(dirNames1);
            ChangedDirectories = new List<DirectoryDifferenceModel>();

            foreach (var dirName in sameDirectories)
            {
                var diff = new DirectoryDifferenceModel(Path.Join(Path1, dirName),
                                                        Path.Join(Path2, dirName));
                if (diff.IsEmpty)
                    UnchangedDirectories.Add(dirName);
                else
                    ChangedDirectories.Add(diff);
            }
        }

        public DirectoryDifferenceModel(string path1, string path2)
        {
            Path1 = path1;
            Path2 = path2;

            dir1 = new DirectoryInfo(path1);
            dir2 = new DirectoryInfo(path2);

            CalculateDifference();

            IsEmpty = AddedDirectories.Count() == 0
                   && DeletedDirectories.Count() == 0
                   && ChangedDirectories.Count() == 0
                   && AddedFiles.Count() == 0
                   && DeletedFiles.Count() == 0
                   && ChangedFiles.Count() == 0;
        }

        public DirectoryDifferenceModel()
        {
            Path1 = null;
            Path2 = null;

            AddedDirectories = new List<string>();
            DeletedDirectories = new List<string>();

            AddedFiles = new List<string>();
            DeletedFiles = new List<string>();

            UnchangedFiles = new List<string>();
            UnchangedDirectories = new List<string>();

            ChangedFiles = new List<FileDifferenceModel>();
            ChangedDirectories = new List<DirectoryDifferenceModel>();
            IsEmpty = true;
        }
    }
}