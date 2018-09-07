using System;

namespace hashes
{
	public class GhostKey
	{
        private int hashChanger = 0;
		public string Name { get; private set; }

		public GhostKey(string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			Name = name;
		}

		public void DoSomething()
		{
            hashChanger++;
		}

        public override bool Equals(object obj)
        {
            var key = obj as GhostKey;
            return key.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + hashChanger;
        }
    }
}