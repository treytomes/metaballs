namespace Metaballs.Brushes;

interface IMetaballsBrush
{
	void Draw(MetaballsBuffer buffer, int sx, int sy, int size);
}
