using System;

namespace AccountManager.Domain.Security
{
    [Serializable]
    public class SignedEntity<TEntity>
        where TEntity: ISignableEntity, new()
    {
        public TEntity Entity { get; set; }

        public byte[] Signature { get; set; }
    }
}
