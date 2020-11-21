using System;
using System.Collections.Generic;
using Model;

namespace WpfGuiView
{
	public class ChangeBlockView
	{
		private readonly ChangeBlock _changeBlock;
		public readonly int ID;
		public int StartPos1 => _changeBlock.StartPos1 + 1;
		public int StartPos2 => _changeBlock.StartPos2 + 1;
		public int EndPos1 => _changeBlock.EndPos1;
		public int EndPos2 => _changeBlock.EndPos2;

		public string Delete
        {
            get
            {
				var formattedDelete = new List<String>();
				
				for (var i = 0; i < _changeBlock.Delete.Length; i++)
					formattedDelete.Add($"- {StartPos1 + i}:  {_changeBlock.Delete[i]}");
				return String.Join("\n", formattedDelete);
            }
        }

		public string Insert
		{
			get
			{
				var formattedInsert = new List<String>();

				for (var i = 0; i < _changeBlock.Insert.Length; i++)
					formattedInsert.Add($"+ {StartPos2 + i}:  {_changeBlock.Insert[i]}");
				return String.Join("\n", formattedInsert);
			}
		}

		public override string ToString()
		{
			if (EndPos2 - StartPos2 < 1)
				return $"Line {StartPos2}";
			return $"Lines {StartPos2} - {EndPos2}";
		}

		

		public ChangeBlockView(ChangeBlock changeBlock, int id)
		{
			_changeBlock = changeBlock;
			ID = id;
		}
	}
}