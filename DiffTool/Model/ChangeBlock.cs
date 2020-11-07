namespace Model
{
    public class ChangeBlock
    {
        public int StartPos; // Position in the second (changed) file
        public int EndPos; //
        public string[] Delete;
        public string[] Insert;

        public ChangeBlock(int start, int end, string[] del, string[] ins)
        {
            StartPos = start;
            EndPos = end;
            Delete = del;
            Insert = ins;
        }
    }
}