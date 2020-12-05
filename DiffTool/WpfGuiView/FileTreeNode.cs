using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Model;

namespace WpfGuiView.ViewModel
{
    public enum NodeType
    {
		ChangedDir,
		ChangedFile,
		AddedDir,
		AddedFile,
		DeletedDir,
		DeletedFile,
		UnchangedDir,
		UnchangedFile
    }

	public class FileTreeNode
	{
		public FileTreeNode Parent;
		public ObservableCollection<FileTreeNode> Children;
		public string Name;
		public string Path1;
		public string Path2;
		private string _path => Path1 ?? Path2;
		public readonly NodeType Type;

		private IEnumerable<string> getDirNames()
        {
			var dir = new DirectoryInfo(_path);
			return dir.EnumerateDirectories().Select(di => di.Name);

		}

		private IEnumerable<string> getFileNames()
		{
			var dir = new DirectoryInfo(_path);
			return dir.EnumerateFiles().Select(di => di.Name);

		}

		public FileTreeNode(DirectoryDifferenceModel diff, FileTreeNode parent)
        {
			Parent = parent;
			Type = NodeType.ChangedDir;
			Name = diff.Name;
			var children = new List<FileTreeNode>();
			children.AddRange(diff.ChangedDirectories.Select(d => new FileTreeNode(d, this)));
			children.AddRange(diff.DeletedDirectories.Select(d => new FileTreeNode(d, this, NodeType.DeletedDir)));
			children.AddRange(diff.AddedDirectories.Select(d => new FileTreeNode(d, this, NodeType.AddedDir)));
			children.AddRange(diff.UnchangedDirectories.Select(d => new FileTreeNode(d, this, NodeType.UnchangedDir)));
			children.AddRange(diff.ChangedFiles.Select(d => new FileTreeNode(d, this)));
			children.AddRange(diff.DeletedFiles.Select(d => new FileTreeNode(d, this, NodeType.DeletedFile)));
			children.AddRange(diff.AddedFiles.Select(d => new FileTreeNode(d, this, NodeType.AddedFile)));
			children.AddRange(diff.UnchangedFiles.Select(d => new FileTreeNode(d, this, NodeType.UnchangedFile)));
			Children = new ObservableCollection<FileTreeNode>(children);
		}
		public FileTreeNode(FileDifferenceModel diff, FileTreeNode parent)
		{
			Path1 = diff.Path1;
			Path2 = diff.Path2;
			Name = Path1.Substring(Path1.LastIndexOf('/') + 1);
			Type = NodeType.ChangedFile;
			Children = new ObservableCollection<FileTreeNode>();
		}

		public FileTreeNode(string name, FileTreeNode parent, NodeType type)
        {
			Parent = parent;
			Type = type;
			Name = name;
			var children = new List<FileTreeNode>();

			if (type == NodeType.AddedFile ||
				type == NodeType.DeletedFile ||
				type == NodeType.UnchangedFile)
				return;

			NodeType fileType;
			if (type == NodeType.AddedDir)
			{
				Path1 = null;
				Path2 = Path.Join(parent.Path2, name);
				fileType = NodeType.AddedFile;
			}
			else if (type == NodeType.DeletedDir)
			{
				Path1 = Path.Join(parent.Path1, name);
				Path2 = Path.Join(parent.Path2, name);
				fileType = NodeType.DeletedFile;
			}
			else
			{
				Path1 = Path.Join(parent.Path1, name);
				Path2 = null;
				fileType = NodeType.UnchangedFile;
			}

			children.AddRange(
						getDirNames().Select(
							d => new FileTreeNode(d, this, type)));
			children.AddRange(
						getFileNames().Select(
							d => new FileTreeNode(d, this, fileType)));
			Children = new ObservableCollection<FileTreeNode>(children);
		}
	}
}
