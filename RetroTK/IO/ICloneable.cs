namespace RetroTK.IO;

interface ICloneable<T> : ICloneable
{
	new T Clone();
}