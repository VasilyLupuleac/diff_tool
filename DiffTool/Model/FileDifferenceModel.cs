using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Linq;
using Logic;


namespace Model
{
    public class FileDifferenceModel : INotifyPropertyChanged
    {
        private static void Main()
        {
        }

        private string _path1;
        private string _path2;
        public string Path1
        {
            get => _path1;
            set
            {
                _path1 = value;
                CalculateDifference();
            }
        }
        public string Path2 {
            get => _path2;
            set
            {
                _path2 = value;
                CalculateDifference();
            }
        }

        public List<ChangeBlock> ChangeBlocks { get; private set; }


        public void CalculateDifference()
        {
            if (_path1 is null || _path2 is null)
                ChangeBlocks = new List<ChangeBlock>();
            else
            {
                var lcs = FileOperations.GetLCS(_path1, _path2);
                ChangeBlocks = LcsToChangeBlocks(_path1, _path2, lcs);
            }
            OnPropertyChanged("ChangeBlocks");
        }

        public FileDifferenceModel(string path1=null, string path2=null)
        {
            _path1 = path1;
            _path2 = path2;
            CalculateDifference();
        }

        public static List<ChangeBlock> LcsToChangeBlocks(string path1, string path2, IEnumerable<FileOperations.Pos> lcs)
        {
            var len1 = File.ReadLines(path1).Count();
            var len2 = File.ReadLines(path2).Count();
            var changeBlocks = new List<ChangeBlock>();
            var lcsWithEof = new List<FileOperations.Pos>(lcs) { new FileOperations.Pos(len1, len2) }; // Consider ends of files as fictive equal lines
            var end1 = -1;
            var end2 = -1;

            using (var lines1 = new OneWayReader(path1))
                using (var lines2 = new OneWayReader(path2))
                    foreach (var pos in lcsWithEof)
                    {
                        var start1 = end1;
                        var start2 = end2;
                        end1 = pos.Pos1;
                        end2 = pos.Pos2;
                        if (end1 - start1 > 1 || end2 - start2 > 1)  // Equal lines are not consecutive so there are changes between them
                            changeBlocks.Add(new ChangeBlock(
                                start2 + 1,
                                end2 - 1,
                                lines1.LineRange(start1 + 1, end1),
                                lines2.LineRange(start2 + 1, end2)
                            ));
                    }

            return changeBlocks;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

