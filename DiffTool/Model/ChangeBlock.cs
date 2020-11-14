namespace Model
{
    public class ChangeBlock
    {
        public int StartPos2; // Position in the second (changed) file
        public int EndPos2; //
        public string[] Delete;
        public string[] Insert;

        public ChangeBlock(int start, int end, string[] del, string[] ins)
        {
            StartPos2 = start;
            EndPos2 = end;
            Delete = del;
            Insert = ins;
        }
    }
}