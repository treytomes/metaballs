namespace RetroTK.IO;

public interface ICloneable<T> : ICloneable
{
	new T Clone();
}