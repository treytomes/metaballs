namespace RetroTK.States;

public interface IGameComponent
{
	void Load();
	void Unload();
	void Render(GameTime gameTime);
	void Update(GameTime gameTime);
}
