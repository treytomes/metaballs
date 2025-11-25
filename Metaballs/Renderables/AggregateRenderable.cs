using System.Collections;
using RetroTK.Gfx;

namespace Metaballs.Renderables;

class AggregateRenderable : BaseRenderable, IList<IRenderable>
{
	#region Fields

	private IList<IRenderable> _children = new List<IRenderable>();

	#endregion

	#region Properties

	public int Count => _children.Count;

	public bool IsReadOnly => _children.IsReadOnly;

	public IRenderable this[int index]
	{
		get => _children[index];
		set => _children[index] = value;
	}

	#endregion

	#region Methods

	public override void Render(IRenderingContext rc)
	{
		if (!IsVisible) return;

		Parallel.ForEach(_children, x => x.Render(rc));
	}

	public void Add(IRenderable item)
	{
		if (_children.Contains(item))
		{
			return;
		}
		item.Parent = this;
		_children.Add(item);
	}

	public void Clear()
	{
		foreach (var child in _children)
		{
			child.Parent = null;
		}
		_children.Clear();
	}

	public bool Contains(IRenderable item)
	{
		return _children.Contains(item);
	}

	public void CopyTo(IRenderable[] array, int arrayIndex)
	{
		_children.Select(x => x.Clone()).ToList().CopyTo(array, arrayIndex);
	}

	public IEnumerator<IRenderable> GetEnumerator()
	{
		return _children.GetEnumerator();
	}

	public int IndexOf(IRenderable item)
	{
		return _children.IndexOf(item);
	}

	public void Insert(int index, IRenderable item)
	{
		_children.Insert(index, item);
	}

	public bool Remove(IRenderable item)
	{
		item.Parent = null;
		return _children.Remove(item);
	}

	public void RemoveAt(int index)
	{
		_children.RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_children).GetEnumerator();
	}

	public override IRenderable Clone()
	{
		return new AggregateRenderable()
		{
			Position = Position,
			_children = _children.Select(x => x.Clone()).ToList(),
		};
	}

	#endregion
}
