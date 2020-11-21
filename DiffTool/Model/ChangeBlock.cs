namespace Model
{
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
    }
}