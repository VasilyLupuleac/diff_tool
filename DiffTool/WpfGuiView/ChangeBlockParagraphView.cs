using System;
using Model;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfGuiView
{
	public class ChangeBlockParagraphView
	{
		private readonly int _startPos1, _startPos2, _endPos1, _endPos2;
		public int StartPos1 => _startPos1;
		public int StartPos2 => _startPos2 + 1;
		public int EndPos1 => _endPos1;
		public int EndPos2 => _endPos2;

		private Paragraph _insert;
		private Paragraph _delete;

		public Paragraph Delete => _delete;

		public Paragraph Insert => _insert;

		public ChangeBlockParagraphView(ChangeBlock changeBlock)
		{
			var insertText = String.Join('\n', changeBlock.Insert);
			if (insertText == "")
				_insert = new Paragraph();
			else
			{
				_insert = new Paragraph(new Run(insertText));
				_insert.Background = Brushes.LightGreen;
			}

			var deleteText = String.Join('\n', changeBlock.Delete);
			if (deleteText == "")
				_delete = new Paragraph();
			else
			{
				_delete = new Paragraph(new Run(deleteText));
				_delete.Background = Brushes.PaleVioletRed;
			}

			_startPos1 = changeBlock.StartPos1;
			_startPos2 = changeBlock.StartPos2;
			_endPos1 = changeBlock.EndPos1;
			_endPos2 = changeBlock.EndPos2;
	}
	}
}