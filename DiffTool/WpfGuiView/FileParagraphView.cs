using System;
using System.Collections.Generic;
using Model;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Media;
using System.Windows;

namespace WpfGuiView
{
	public class FileParagraphView
	{
		private FlowDocument _doc;
		public FlowDocument Document => _doc;

		private List<int> _changeBlocksPositions;

		public List<int> ChangeBlocksPositions => _changeBlocksPositions;
		public FileParagraphView(FileDifferenceModel diff)
		{
			var changeBlockViews = diff.ChangeBlocks.Select(cb => new ChangeBlockParagraphView(cb));
			var curPos = 0;
			_doc = new FlowDocument();
			_doc.FontFamily = new FontFamily("Pericles Light");
			_doc.FontWeight = FontWeights.Light;
			_changeBlocksPositions = new List<int>();
			var curParagraphNumber = 0;
			using (var reader = new OneWayReader(diff.Path2))
			{
				foreach (var cb in changeBlockViews)
                {
					var isMarked = false;

					if (cb.StartPos2 > 0)
					{
						var buffer = reader.LineRange(curPos, cb.StartPos2 - 1);
						for (var i = 0; i < cb.StartPos2 - curPos - 1; i++)
							buffer[i] = $"{i + 1 + curPos}.    {buffer[i]}";
						var text = String.Join('\n', buffer);
						_doc.Blocks.Add(new Paragraph(new Run(text)));
						curParagraphNumber++;
					}

					if (cb.Insert.Inlines.Any())
					{
						_doc.Blocks.Add(cb.Insert);
						_changeBlocksPositions.Add(curParagraphNumber++);
						isMarked = true;
					}
					if (cb.Delete.Inlines.Any())
					{
						_doc.Blocks.Add(cb.Delete);
						if (!isMarked)
							_changeBlocksPositions.Add(curParagraphNumber);
						curParagraphNumber++;
					}
					curPos = cb.EndPos2;
                }
				var endLines = reader.ReadToEnd(changeBlockViews.Last().EndPos1);
				if (endLines.Length > 0)
                {
					var endText = String.Join('\n', endLines);
					_doc.Blocks.Add(new Paragraph(new Run(endText)));
				}
			}
		}
	}
}