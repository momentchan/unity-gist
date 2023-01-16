namespace mj.gist
{
	public interface IInitialize
	{
		bool Initialized { get; set; }
		void Initialize();
	}
}