﻿using System;
using System.Collections.Generic;
using Model;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Media;

namespace WpfGuiView
{
	public class FileParagraphView
	{
		private FlowDocument _doc;
		public FlowDocument Document => _doc;
		public FileParagraphView(FileDifferenceModel diff)
		{
			var changeBlockViews = diff.ChangeBlocks.Select(cb => new ChangeBlockParagraphView(cb));
			var curPos = 0;
			_doc = new FlowDocument();
			var p = new Paragraph();
			using (var reader = new OneWayReader(diff.Path1))
			{
				foreach (var cb in changeBlockViews)
                {
					if (cb.StartPos1 > curPos)
					{
						var buffer = reader.LineRange(curPos, cb.StartPos1);
						var text = String.Join('\n', buffer);
						_doc.Blocks.Add(new Paragraph(new Run(text)));
					}
					if (cb.Insert.Inlines.Any())
						_doc.Blocks.Add(cb.Insert);
					if (cb.Delete.Inlines.Any())
						_doc.Blocks.Add(cb.Delete);
					curPos = cb.EndPos1;
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