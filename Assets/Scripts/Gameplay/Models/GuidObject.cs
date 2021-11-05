using System;

namespace Gameplay.Models
{
    public class GuidObject
    {
        public readonly Guid Guid;

        public GuidObject()
        {
            Guid = Guid.NewGuid();
        }

        public GuidObject(Guid guid)
        {
            Guid = guid;
        }
    }
}