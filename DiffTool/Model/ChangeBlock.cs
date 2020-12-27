using System;

namespace Model
{
    public class ChangeBlockLoadException: Exception { }

    public class ChangeBlock
    {
        public int StartPos1;
        public int EndPos1;
        public int StartPos2;
        public int EndPos2;
        public string[] Delete;
        public string[] Insert;

        public ChangeBlock(int start1, int end1, int start2, int end2, string[] del, string[] ins)
        {
            StartPos1 = start1;
            EndPos1 = end1;
            StartPos2 = start2;
            EndPos2 = end2;
            Delete = del;
            Insert = ins;
        }
        public ChangeBlock(System.IO.StreamReader stream)
        {
            var positions = stream.ReadLine().Split(' ');
            try
            {
                StartPos1 = int.Parse(positions[0]);
                EndPos1 = int.Parse(positions[1]);
                StartPos2 = int.Parse(positions[2]);
                EndPos2 = int.Parse(positions[3]);

                Delete = new string[EndPos1 - StartPos1];
                Insert = new string[EndPos2 - StartPos2];
            }
            catch
            {
                throw new ChangeBlockLoadException();
            }


            for (int i = 0; i < StartPos1 - EndPos1; i++)
                Delete[i] = stream.ReadLine();
            for (int i = 0; i < StartPos2 - EndPos2; i++)
                Insert[i] = stream.ReadLine();
        }
        public void Save(System.IO.StreamWriter stream)
        {
            stream.WriteLine($"{StartPos1} {EndPos1} {StartPos2} {EndPos2}");
            foreach (var del in Delete)
                stream.WriteLine(del);
            foreach (var ins in Insert)
                stream.WriteLine(ins);
        }
    }
}